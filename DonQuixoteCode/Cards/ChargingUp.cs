using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using DonQuixote.Characters;
using DonQuixote.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class ChargingUp : ModCardTemplate
{
    private const int BaseEnergyCost = 2;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("VigorPerTurn", 8m)
    ];

    public ChargingUp() : base(BaseEnergyCost, CardType.Power, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<ChargingUpPower>(
            choiceContext, Owner.Creature, DynamicVars["VigorPerTurn"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VigorPerTurn"].UpgradeValueBy(4m);
    }
}