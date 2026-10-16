using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using DonQuixote.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Powers;

[RegisterPower]
public sealed class DauntlessPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override decimal ModifyDamageMultiplicative(
        Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target != Owner) return 1m;

        bool hasOath = Owner.GetPower<KnightOathPower>() != null;
        bool hasMount = Owner.Player?.Relics.Any(r => r is FantasyMountDiomedes) == true;
        if (hasOath || hasMount) return 0.5m;
        return 0.7m;
    }

    // 幻想坐骑：每次无畏层数增加时（含叠层）→ +5 残废
    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext, PowerModel power,
        decimal amount, Creature? target, Creature? applier, CardModel? cardSource)
    {
        if (power != this) return;
        if (target != Owner) return;
        if (amount <= 0) return;

        var mount = Owner.Player?.Relics.Any(r => r is FantasyMountDiomedes) == true;
        if (!mount) return;

        await PowerCmd.Apply<CripplePower>(
            choiceContext, Owner, 5m, applier, null);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Decrement(this);
        }
    }
}