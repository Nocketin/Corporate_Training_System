using Consul;
using Microsoft.Extensions.Configuration;

namespace Platform.Common.ServiceDiscovery;

public class ConsulService
{
    private readonly string _serviceId;
    private readonly string _serviceName;
    private readonly string _serviceAddress;
    private readonly int _servicePort;
    private readonly string _consulHost;

    public ConsulService(IConfiguration configuration)
    {
        var cfg = configuration.GetSection("ConsulConfig");
        _consulHost = cfg["Host"]!;
        _serviceName = cfg["ServiceName"]!;
        _serviceId = cfg["ServiceId"]!;
        _servicePort = int.Parse(cfg["ServicePort"]!);
        _serviceAddress = cfg["ServiceAddress"]!;
    }

    public async Task RegisterAsync()
    {
        try
        {
            Console.WriteLine($"[Consul] Attempting to register service '{_serviceName}' with ID '{_serviceId}'");
            Console.WriteLine($"[Consul] Consul Host: {_consulHost}");
            Console.WriteLine($"[Consul] Service Address: {_serviceAddress}:{_servicePort}");
            
            using var client = new ConsulClient(c => c.Address = new Uri(_consulHost));
            var registration = new AgentServiceRegistration
            {
                ID = _serviceId,
                Name = _serviceName,
                Address = _serviceAddress,
                Port = _servicePort,
                Check = new AgentServiceCheck
                {
                    HTTP = $"http://{_serviceAddress}:{_servicePort}/health",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
                }
            };

            await client.Agent.ServiceDeregister(_serviceId);
            Console.WriteLine($"[Consul] Deregistered old service instance (if any)");
            
            await client.Agent.ServiceRegister(registration);
            Console.WriteLine($"[Consul] Successfully registered service '{_serviceName}' with Consul!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Consul] ERROR: Failed to register service - {ex.Message}");
            Console.WriteLine($"[Consul] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task DeregisterAsync()
    {
        using var client = new ConsulClient(c => c.Address = new Uri(_consulHost));
        await client.Agent.ServiceDeregister(_serviceId);
    }
}