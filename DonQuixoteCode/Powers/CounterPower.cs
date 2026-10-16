using System.Linq;
using System.Threading.Tasks;
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
public sealed class CounterPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext, Creature target, DamageResult result,
        ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return;
        if (dealer == null) return;
        if (!props.IsPoweredAttack()) return;

        bool hasMambrino = Owner.Player?.Relics.Any(r => r is FantasyHelmetMambrino) == true;

        // 无曼布里诺时：必须完全格挡
        if (!hasMambrino && !result.WasFullyBlocked) return;

        decimal perStack = 5m;
        var memory = Owner.GetPower<MuscleMemoryPower>();
        if (memory != null) perStack = memory.Amount;

        // 曼布里诺：触发时自身 +1 残废
        if (hasMambrino)
        {
            await PowerCmd.Apply<CripplePower>(
                choiceContext, Owner, 1m, Owner, null);
        }

        await CreatureCmd.Damage(
            choiceContext, dealer, perStack * Amount,
            ValueProp.Unpowered, Owner);
        await PowerCmd.Decrement(this);
    }
}