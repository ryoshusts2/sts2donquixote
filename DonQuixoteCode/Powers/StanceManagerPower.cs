using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using DonQuixote.Stance;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Powers;

[RegisterPower]
public sealed class StanceManagerPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (GetCurrentStance() == StanceType.None)
        {
            await PowerCmd.Apply<NeutralStancePower>(
                new ThrowingPlayerChoiceContext(), Owner, 1m, Owner, null);
        }
    }

    public override decimal ModifyDamageMultiplicative(
        Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner) return 1m;
        if (cardSource is not IStanceCard sc) return 1m;
        if (sc.Stance == GetCurrentStance()) return 1m;
        return 0.7m;
    }

    public override decimal ModifyBlockMultiplicative(
        Creature target, decimal block, ValueProp props,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target != Owner) return 1m;
        if (cardSource is not IStanceCard sc) return 1m;
        if (sc.Stance == GetCurrentStance()) return 1m;
        return 0.7m;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (cardPlay.Card is not IStanceCard sc) return;

        // 姿态锁定：本回合不切换姿态
        if (Owner.GetPower<ChargeStanceLockPower>() != null) return;

        var current = GetCurrentStance();
        if (sc.Stance == current) return;

        await SwitchStance(choiceContext, sc.Stance);
    }

    private StanceType GetCurrentStance()
    {
        if (Owner.GetPower<ChargeStancePower>() != null) return StanceType.Charge;
        if (Owner.GetPower<CounterStancePower>() != null) return StanceType.Counter;
        if (Owner.GetPower<NeutralStancePower>() != null) return StanceType.Neutral;
        if (Owner.GetPower<RestStancePower>() != null) return StanceType.Rest;
        return StanceType.None;
    }

    private async Task SwitchStance(PlayerChoiceContext ctx, StanceType target)
    {
        switch (GetCurrentStance())
        {
            case StanceType.Charge:
                var charge = Owner.GetPower<ChargeStancePower>();
                if (charge != null) await PowerCmd.Remove(charge);
                break;
            case StanceType.Counter:
                var counter = Owner.GetPower<CounterStancePower>();
                if (counter != null) await PowerCmd.Remove(counter);
                break;
            case StanceType.Neutral:
                var neutral = Owner.GetPower<NeutralStancePower>();
                if (neutral != null) await PowerCmd.Remove(neutral);
                break;
            case StanceType.Rest:
                var rest = Owner.GetPower<RestStancePower>();
                if (rest != null) await PowerCmd.Remove(rest);
                break;
        }

        switch (target)
        {
            case StanceType.Charge:
                await PowerCmd.Apply<ChargeStancePower>(ctx, Owner, 1m, Owner, null);
                break;
            case StanceType.Counter:
                await PowerCmd.Apply<CounterStancePower>(ctx, Owner, 1m, Owner, null);
                break;
            case StanceType.Neutral:
                await PowerCmd.Apply<NeutralStancePower>(ctx, Owner, 1m, Owner, null);
                break;
            case StanceType.Rest:
                await PowerCmd.Apply<RestStancePower>(ctx, Owner, 1m, Owner, null);
                break;
        }
    }
}