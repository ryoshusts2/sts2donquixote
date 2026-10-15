using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using DonQuixote.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace DonQuixote.Relics;

[RegisterRelic(typeof(DonQuixoteRelicPool))]
[RegisterCharacterStarterRelic(typeof(DonQuixoteCharacter))]
public sealed class UnwillingDreamRelic : ModRelicTemplate
{
    public static readonly SavedAttachedState<UnwillingDreamRelic, int> DreamCount =
        new("DreamCount", _ => 50);

    public static readonly SavedAttachedState<UnwillingDreamRelic, bool> Locked =
        new("Locked", _ => false);

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("DreamCount", DreamCount[this])
    ];

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    public override Task AfterObtained()
    {
        Owner.RelicRemoved += OnRelicRemoved;
        return Task.CompletedTask;
    }

    // 战斗胜利：当前生命 > 10 → 计数 -3
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (Locked[this]) return;
        if (Owner.Creature.CurrentHp > 10)
        {
            await IncreaseAsync(-3);
        }
    }

    // 死亡：+20（仅多人模式生效——单人模式死亡即游戏结束）
    public override async Task AfterDeath(
        PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature != Owner.Creature) return;
        if (wasRemovalPrevented) return;
        if (Locked[this]) return;
        await IncreaseAsync(20);
    }

    // 外部调用：英雄的战利品获得废料时
    public void ProcessScrapGained(int amount)
    {
        if (Locked[this]) return;
        int newValue = Math.Max(0, DreamCount[this] - amount);
        DreamCount[this] = newValue;
        DynamicVars["DreamCount"].BaseValue = newValue;
    }

    // 失去粗糙废料遗物时触发
    private void OnRelicRemoved(RelicModel relic)
    {
        if (Locked[this]) return;
        if (relic.Id.Entry.StartsWith("DON_QUIXOTE_RELIC_ROUGH_SCRAP"))
        {
            TaskHelper.RunSafely(IncreaseAsync(5));
        }
    }

    private async Task IncreaseAsync(int amount)
    {
        if (Locked[this]) return;
        int newValue = Math.Clamp(DreamCount[this] + amount, 0, 100);
        DreamCount[this] = newValue;
        DynamicVars["DreamCount"].BaseValue = newValue;

        if (newValue >= 100)
        {
            await HandleThresholdAsync();
        }
    }

    private async Task HandleThresholdAsync()
    {
        if (Locked[this]) return;
        Locked[this] = true;

        // 失去英雄的战利品
        var heroRelic = Owner.Relics.FirstOrDefault(r => r is DonQuixoteRelic);
        if (heroRelic != null)
        {
            await RelicCmd.Remove(heroRelic);
        }

        // 获得「自我？」
        var selfRelic = ModelDb.Relic<SelfRelic>().ToMutable();
        await RelicCmd.Obtain(selfRelic, Owner, Owner.Relics.Count);
    }
}