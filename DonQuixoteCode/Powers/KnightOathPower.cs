using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Powers;

[RegisterPower]
public sealed class KnightOathPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    // 队友受伤 × 0.8（少受 20%）
    public override decimal ModifyDamageMultiplicative(
        Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target == null) return 1m;
        if (target == Owner) return 1m;
        if (target.Side != Owner.Side) return 1m;
        if (!target.IsPlayer) return 1m;
        return 0.8m;
    }

    // 队友受伤时，堂吉诃德承受 20%
    public override async Task BeforeDamageReceived(
        PlayerChoiceContext choiceContext, Creature target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == null) return;
        if (target == Owner) return;
        if (target.Side != Owner.Side) return;
        if (!target.IsPlayer) return;

        // 防止递归：转移伤害本身是 Unpowered，不再触发
        if (props.HasFlag(ValueProp.Unpowered)) return;

        decimal redirect = amount * 0.2m;
        if (redirect <= 0m) return;

        await CreatureCmd.Damage(
            choiceContext, Owner, redirect,
            ValueProp.Unblockable | ValueProp.Unpowered, null);
    }
}