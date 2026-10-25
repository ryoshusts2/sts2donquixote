using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Rewards;
using DonQuixote.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Interactions.RightClick;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace DonQuixote.Relics;

[RegisterRelic(typeof(DonQuixoteRelicPool))]
[RegisterCharacterStarterRelic(typeof(DonQuixoteCharacter))]
public sealed class DonQuixoteRelic : ModRelicTemplate, IModRightClickableRelic
{
    public static readonly SavedAttachedState<DonQuixoteRelic, int> ScrapCount =
        new("ScrapCount", _ => 0);

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("ScrapCount", ScrapCount[this])
    ];

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    public int GetScrap() => ScrapCount[this];
    public bool HasEnoughScrap(int amount) => ScrapCount[this] >= amount;

    public void SpendScrap(int amount)
    {
        ScrapCount[this] = System.Math.Max(0, ScrapCount[this] - amount);
        DynamicVars["ScrapCount"].BaseValue = ScrapCount[this];
    }

    // ================= 右键交互逻辑 =================

    public bool CanHandleRightClickLocal(ModRightClickContext context)
    {
        return HasEnoughScrap(5);
    }

    public async Task OnRightClick(ModRightClickExecutionContext context)
    {
        if (!HasEnoughScrap(5)) return;

        SpendScrap(5);

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
        if (chosen != null)
        {
            await RelicCmd.Obtain(chosen, Owner, Owner.Relics.Count);
        }
    }

    // ================= 战斗及房间逻辑 =================

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (player != Owner) return false;
        if (room == null) return false;
        if (!room.RoomType.IsCombatRoom()) return false;
        
        if (room.RoomType == RoomType.Boss && player.RunState.CurrentActIndex >= player.RunState.Acts.Count - 1)
        {
            return false;
        }

        // 纯粹替换金币为废料，不参与遗物奖励
        rewards.RemoveAll(r => r is GoldReward);

        int scrap = room.RoomType switch
        {
            RoomType.Elite => 2,
            RoomType.Boss => 5,
            _ => 1
        };

        ScrapCount[this] += scrap;
        DynamicVars["ScrapCount"].BaseValue = ScrapCount[this];

        var dream = Owner.Relics.FirstOrDefault(r => r is UnwillingDreamRelic) as UnwillingDreamRelic;
        dream?.ProcessScrapGained(scrap);

        return true;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType != RoomType.Shop) return Task.CompletedTask;

        ScrapCount[this] += 2;
        DynamicVars["ScrapCount"].BaseValue = ScrapCount[this];

        var dream = Owner.Relics.FirstOrDefault(r => r is UnwillingDreamRelic) as UnwillingDreamRelic;
        dream?.ProcessScrapGained(2);

        return Task.CompletedTask;
    }
}