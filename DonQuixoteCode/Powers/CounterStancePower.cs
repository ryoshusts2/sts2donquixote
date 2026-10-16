using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Powers;

[RegisterPower]
public sealed class CounterStancePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<DexterityPower>(ctx, Owner, 5m, Owner, null);
        await PowerCmd.Apply<WeakPower>(ctx, Owner, 1m, Owner, null);

        // 反击补至 4 层
        var counter = Owner.GetPower<CounterPower>();
        if (counter == null)
        {
            await PowerCmd.Apply<CounterPower>(ctx, Owner, 4m, Owner, null);
        }
        else if (counter.Amount < 4)
        {
            await PowerCmd.ModifyAmount(ctx, counter, 4 - counter.Amount, null, null);
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<DexterityPower>(ctx, oldOwner, -5m, oldOwner, null);
        await PowerCmd.Apply<WeakPower>(ctx, oldOwner, -1m, oldOwner, null);
    }
}