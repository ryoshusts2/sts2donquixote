using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace DonQuixote.Relics;

[RegisterRelic(typeof(DonQuixoteRelicPool))]
public sealed class RoughScrapLance : ModRelicTemplate
{
    public static readonly SavedAttachedState<RoughScrapLance, int> BattlesElapsed =
        new("BattlesElapsed", _ => 0);

    public override RelicRarity Rarity => RelicRarity.Event;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    public override async Task AfterObtained()
    {
        await CreatureCmd.Heal(Owner.Creature, Owner.Creature.MaxHp * 0.15m);
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        BattlesElapsed[this]++;
        if (BattlesElapsed[this] >= 2)
        {
            await RelicCmd.Remove(this);
        }
    }

    // 冲锋卡造成伤害时，其他敌人受到等量伤害
    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result,
        ValueProp props, Creature target, CardModel? cardSource)
    {
        // 只处理：堂吉诃德 → 敌方，且来源是带 Strike tag 的攻击牌
        if (dealer != Owner.Creature) return;
        if (cardSource == null) return;
        if (cardSource.Owner != Owner) return;
        if (cardSource.Type != CardType.Attack) return;
        if (!cardSource.Tags.Contains(CardTag.Strike)) return;
        if (target.Side == Owner.Creature.Side) return;

        // 【防护】全体攻击卡不触发溅射（避免连锁爆炸）
        if (cardSource.TargetType == TargetType.AllEnemies) return;

        // 【防护】本次伤害本身是溅射（Unpowered）时不触发（防连锁）
        if (props.HasFlag(ValueProp.Unpowered)) return;

        decimal splash = result.TotalDamage;
        if (splash <= 0) return;

        var combatState = target.CombatState;
        if (combatState == null) return;

        foreach (var enemy in combatState.HittableEnemies.ToList())
        {
            if (enemy == target) continue;
            await CreatureCmd.Damage(
                choiceContext, enemy, splash,
                ValueProp.Unpowered, Owner.Creature, null);
        }
    }
}