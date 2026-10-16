using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using DonQuixote.Characters;
using DonQuixote.Phases;
using DonQuixote.Powers;
using DonQuixote.Stance;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class CruelMethods : ModCardTemplate, IStanceCard, IPhase2Card
{
    public StanceType Stance => StanceType.Charge;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new PowerVar<BoneShatterPower>(2m),
        new PowerVar<CripplePower>(5m)
    ];

    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    public CruelMethods() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await PowerCmd.Apply<BoneShatterPower>(
            choiceContext, cardPlay.Target, DynamicVars["BoneShatterPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<CripplePower>(
            choiceContext, cardPlay.Target, DynamicVars["CripplePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10m);
    }
}