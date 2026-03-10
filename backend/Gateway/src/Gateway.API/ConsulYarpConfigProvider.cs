using Consul;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Gateway.API;

public class ConsulYarpConfigProvider : IProxyConfigProvider, IDisposable
{
    private readonly IConsulClient _consulClient;
    private readonly TimeSpan _reloadInterval;
    private Timer? _timer;
    private volatile ConsulProxyConfig _currentConfig;

    public ConsulYarpConfigProvider(IConsulClient consulClient)
    {
        _consulClient = consulClient;
        _reloadInterval = TimeSpan.FromSeconds(10);
        _currentConfig = new ConsulProxyConfig(Array.Empty<RouteConfig>(), Array.Empty<ClusterConfig>());
        
        ReloadAsync().GetAwaiter().GetResult();
        
        _timer = new Timer(async _ => await ReloadAsync(), null, _reloadInterval, _reloadInterval);
    }

    public IProxyConfig GetConfig()
    {
        Console.WriteLine($"[Gateway] GetConfig called - Routes: {_currentConfig.Routes.Count}, Clusters: {_currentConfig.Clusters.Count}");
        foreach (var route in _currentConfig.Routes)
        {
            Console.WriteLine($"[Gateway]   Route: {route.RouteId} -> {route.Match.Path} => Cluster: {route.ClusterId}");
        }
        return _currentConfig;
    }

    private async Task ReloadAsync()
    {
        try
        {
            Console.WriteLine($"[Gateway] Reloading configuration from Consul...");
            var services = await _consulClient.Agent.Services();
            Console.WriteLine($"[Gateway] Found {services.Response.Count} services in Consul");

            var routes = new List<RouteConfig>();
            var clusters = new List<ClusterConfig>();

            foreach (var service in services.Response.Values)
            {
                Console.WriteLine($"[Gateway] Processing service: {service.Service} at {service.Address}:{service.Port}");
                var clusterId = service.Service;

                clusters.Add(new ClusterConfig
                {
                    ClusterId = clusterId,
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        {
                            $"{clusterId}/dest",
                            new DestinationConfig
                            {
                                Address = $"http://{service.Address}:{service.Port}"
                            }
                        }
                    }
                });
                
                if (string.Equals(clusterId, "IdentityService", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"[Gateway] Creating IdentityService route: /api/auth/{{**catch-all}}");
                    routes.Add(new RouteConfig
                    {
                        RouteId = "identity_auth_route",
                        ClusterId = clusterId,
                        Match = new RouteMatch
                        {
                            Path = "/api/auth/{**catch-all}"
                        }
                    });
                    Console.WriteLine($"[Gateway] Route added: RouteId=identity_auth_route, Path=/api/auth/{{**catch-all}}, Cluster={clusterId}");
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
            
            Console.WriteLine($"[Gateway] Configuration reloaded: {routes.Count} routes, {clusters.Count} clusters");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Gateway] ERROR reloading config: {ex.Message}");
            Console.WriteLine($"[Gateway] Stack trace: {ex.StackTrace}");
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
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

