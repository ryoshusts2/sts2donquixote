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
public sealed class IronCavalryTrample : ModCardTemplate, IStanceCard, IPhase2Card
{
    public StanceType Stance => StanceType.Charge;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BoneShatterPower>(1m)
    ];

    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    public IronCavalryTrample() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        decimal blockDamage = Owner.Creature.Block;
        if (blockDamage <= 0) blockDamage = 0;

        await DamageCmd.Attack(blockDamage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await PowerCmd.Apply<BoneShatterPower>(
            choiceContext, cardPlay.Target,
            DynamicVars["BoneShatterPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BoneShatterPower"].UpgradeValueBy(1m);
    }
}