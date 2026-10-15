using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Powers;

[RegisterPower]
public sealed class ChargingUpPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;

        await PowerCmd.Apply<VigorPower>(
            new BlockingPlayerChoiceContext(), Owner, Amount, Owner, null);
    }
}