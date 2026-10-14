using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using DonQuixote.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Powers;

[RegisterPower]
public sealed class FairDuelPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override decimal ModifyDamageMultiplicative(
        Creature? target, decimal amount, ValueProps props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target == Owner && IsDonQuixote(dealer))
        {
            return 1.2m;
        }
        if (dealer == Owner && IsDonQuixote(target))
        {
            return 0.8m;
        }
        return 1m;
    }

    private static bool IsDonQuixote(Creature? c)
    {
        return c?.Player?.Character is DonQuixoteCharacter;
    }
}