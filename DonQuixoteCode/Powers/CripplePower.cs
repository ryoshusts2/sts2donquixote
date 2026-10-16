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
public sealed class CripplePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;

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
        return 1m + 0.05m * Amount;
    }

    // 库·丘林：每次获得残废时获得等量不屈
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var cuChulainn = Owner.Player?.Relics.Any(r => r is FantasyIdentityCrimsonKnight) == true;
        if (!cuChulainn) return;

        await PowerCmd.Apply<UnbrokenPower>(
            new ThrowingPlayerChoiceContext(), Owner, Amount, applier, null);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        int newAmount = Amount / 2;
        if (newAmount <= 0)
        {
            await PowerCmd.Remove(this);
        }
        else
        {
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, newAmount - Amount, null, null);
        }
    }
}