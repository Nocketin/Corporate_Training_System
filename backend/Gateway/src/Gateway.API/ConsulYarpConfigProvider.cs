using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Gateway.API;

public class ConsulYarpConfigProvider : IProxyConfigProvider, IDisposable
{
    private readonly IConsulClient _consulClient;
    private readonly ILogger<ConsulYarpConfigProvider> _logger;
    private readonly TimeSpan _reloadInterval;
    private readonly CancellationTokenSource _reloadCancellationTokenSource = new();
    private readonly Task _reloadLoopTask;
    private volatile ConsulProxyConfig _currentConfig;

    public ConsulYarpConfigProvider(IConsulClient consulClient, ILogger<ConsulYarpConfigProvider> logger)
    {
        _consulClient = consulClient;
        _logger = logger;
        _reloadInterval = TimeSpan.FromSeconds(10);
        _currentConfig = new ConsulProxyConfig(Array.Empty<RouteConfig>(), Array.Empty<ClusterConfig>());

        ReloadAsync(_reloadCancellationTokenSource.Token).GetAwaiter().GetResult();
        _reloadLoopTask = RunReloadLoopAsync(_reloadCancellationTokenSource.Token);
    }

    public IProxyConfig GetConfig()
    {
        return _currentConfig;
    }

    private async Task RunReloadLoopAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(_reloadInterval);
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            await ReloadAsync(cancellationToken);
        }
    }

    private async Task ReloadAsync(CancellationToken cancellationToken)
    {
        try
        {
            var services = await _consulClient.Agent.Services();

            var routes = new List<RouteConfig>();
            var clusters = new List<ClusterConfig>();

            foreach (var service in services.Response.Values)
            {
                var clusterId = service.Service;
                var destinationHost = string.IsNullOrWhiteSpace(service.Address)
                    ? service.Service
                    : service.Address;

                if (string.IsNullOrWhiteSpace(clusterId) || service.Port <= 0)
                {
                    continue;
                }

                clusters.Add(new ClusterConfig
                {
                    ClusterId = clusterId,
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        {
                            $"{clusterId}/dest",
                            new DestinationConfig
                            {
                                Address = $"http://{destinationHost}:{service.Port}"
                            }
                        }
                    }
                });
                
                if (string.Equals(clusterId, "IdentityService", StringComparison.OrdinalIgnoreCase))
                {
                    routes.Add(new RouteConfig
                    {
                        RouteId = "identity_auth_route",
                        ClusterId = clusterId,
                        Match = new RouteMatch
                        {
                            Path = "/api/auth/{**catch-all}"
                        }
                    });
                }
                else if (string.Equals(clusterId, "LearningService", StringComparison.OrdinalIgnoreCase))
                {
                    routes.Add(new RouteConfig
                    {
                        RouteId = "learning_api_root_route",
                        ClusterId = clusterId,
                        Order = 10,
                        Match = new RouteMatch
                        {
                            Path = "/api/learning"
                        }
                    });
                    routes.Add(new RouteConfig
                    {
                        RouteId = "learning_route",
                        ClusterId = clusterId,
                        Order = 20,
                        Match = new RouteMatch
                        {
                            Path = "/api/learning/{**catch-all}"
                        }
                    });
                }
                else if (string.Equals(clusterId, "CourseService", StringComparison.OrdinalIgnoreCase))
                {
                    // List/create use the exact path /api/courses; catch-all does not match without a trailing segment.
                    routes.Add(new RouteConfig
                    {
                        RouteId = "courses_api_root_route",
                        ClusterId = clusterId,
                        Order = 10,
                        Match = new RouteMatch
                        {
                            Path = "/api/courses"
                        }
                    });
                    routes.Add(new RouteConfig
                    {
                        RouteId = "courses_route",
                        ClusterId = clusterId,
                        Order = 20,
                        Match = new RouteMatch
                        {
                            Path = "/api/courses/{**catch-all}"
                        }
                    });
                    routes.Add(new RouteConfig
                    {
                        RouteId = "lessons_route",
                        ClusterId = clusterId,
                        Order = 20,
                        Match = new RouteMatch
                        {
                            Path = "/api/lessons/{**catch-all}"
                        }
                    });
                }
                else
                {
                    routes.Add(new RouteConfig
                    {
                        RouteId = $"{clusterId}_route",
                        ClusterId = clusterId,
                        Match = new RouteMatch
                        {
                            Path = $"/api/{clusterId.ToLowerInvariant()}/{{**catch-all}}"
                        }
                    });
                }
            }

            var newConfig = new ConsulProxyConfig(routes, clusters);
            _currentConfig = newConfig;
            newConfig.SignalChange();

            _logger.LogInformation("Gateway routes reloaded from Consul: {RouteCount} routes, {ClusterCount} clusters", routes.Count, clusters.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to reload routes from Consul");
        }
    }

    public void Dispose()
    {
        _reloadCancellationTokenSource.Cancel();
        try
        {
            _reloadLoopTask.GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _reloadCancellationTokenSource.Dispose();
        }
    }

    private sealed class ConsulProxyConfig : IProxyConfig
    {
        private CancellationTokenSource _cts = new();

        public ConsulProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }

        public IReadOnlyList<RouteConfig> Routes { get; }
        public IReadOnlyList<ClusterConfig> Clusters { get; }
        public IChangeToken ChangeToken { get; private set; }

        public void SignalChange()
        {
            var oldCts = _cts;
            _cts = new CancellationTokenSource();
            ChangeToken = new CancellationChangeToken(_cts.Token);
            oldCts.Cancel();
        }
    }
}

