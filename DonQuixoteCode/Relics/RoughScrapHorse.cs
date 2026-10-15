using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using DonQuixote.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace DonQuixote.Relics;

[RegisterRelic(typeof(DonQuixoteRelicPool))]
public sealed class RoughScrapHorse : ModRelicTemplate
{
    public static readonly SavedAttachedState<RoughScrapHorse, int> BattlesElapsed =
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

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature)) return;
        var pcs = Owner.PlayerCombatState;
        if (pcs == null) return;
        if (pcs.TurnNumber != 1) return;
        await PlayerCmd.GainEnergy(1, Owner);
    }
}