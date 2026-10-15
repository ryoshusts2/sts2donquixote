using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using DonQuixote.Relics;

namespace DonQuixote.RestSite;

public sealed class CraftOption : RestSiteOption
{
    public const string IdKey = "DON_QUIXOTE_REST_CRAFT";

    public CraftOption(Player owner) : base(owner) { }

    public override string OptionId => IdKey;

    public override LocString Description =>
        new LocString("rest_site_ui", IdKey + ".description");

    public override bool IsEnabled
    {
        get
        {
            var heroRelic = Owner.Relics.FirstOrDefault(r => r is DonQuixoteRelic) as DonQuixoteRelic;
            return heroRelic != null && heroRelic.HasEnoughScrap(5);
        }
    }

    public override async Task<bool> OnSelect()
    {
        var heroRelic = Owner.Relics.FirstOrDefault(r => r is DonQuixoteRelic) as DonQuixoteRelic;
        if (heroRelic == null) return false;
        if (!heroRelic.HasEnoughScrap(5)) return false;

        heroRelic.SpendScrap(5);

        var pool = new List<RelicModel>
        {
            ModelDb.Relic<RoughScrapSword>().ToMutable(),
            ModelDb.Relic<RoughScrapShield>().ToMutable(),
            ModelDb.Relic<RoughScrapHelmet>().ToMutable(),
            ModelDb.Relic<RoughScrapPlate>().ToMutable(),
            ModelDb.Relic<RoughScrapLance>().ToMutable(),
            ModelDb.Relic<RoughScrapHorse>().ToMutable(),
        };

        var chosen = Owner.PlayerRng.Rewards.NextItem(pool);
        if (chosen == null) return false;

        await RelicCmd.Obtain(chosen, Owner, Owner.Relics.Count);
        return true;
    }
}