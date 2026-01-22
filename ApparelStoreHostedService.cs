using BTCPayServer.Plugins.ApparelStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BTCPayServer.Plugins.ApparelStore;

public class ApparelStoreHostedService : IHostedService
{
    private readonly ILogger<ApparelStoreHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ApparelStoreHostedService(
        ILogger<ApparelStoreHostedService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Apparel Store plugin starting...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApparelStoreDbContext>();

            // Apply migrations automatically
            await dbContext.Database.MigrateAsync(cancellationToken);

            _logger.LogInformation("Apparel Store database initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Apparel Store database");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Apparel Store plugin stopping...");
        return Task.CompletedTask;
    }
}
