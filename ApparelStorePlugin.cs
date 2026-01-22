using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Abstractions.Models;
using BTCPayServer.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BTCPayServer.Plugins.ApparelStore;

public class ApparelStorePlugin : BaseBTCPayServerPlugin
{
    public override string Identifier => "BTCPayServer.Plugins.ApparelStore";
    public override string Name => "Apparel Store";
    public override string Description => "A full-featured e-commerce store for selling dropshipped apparel with product variants, image galleries, and shopping cart";
    public override Version Version => new Version(1, 0, 0);

    public override void Execute(IServiceCollection services)
    {
        services.AddSingleton<IUIExtension>(new UIExtension("ApparelStore/NavigationExtension", "header-nav"));
        services.AddHostedService<ApparelStoreHostedService>();
    }
}
