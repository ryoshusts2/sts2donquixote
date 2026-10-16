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
public sealed class MomentumCounter : ModCardTemplate, IStanceCard, IPhase2Card
{
    public StanceType Stance => StanceType.Counter;

    public override bool GainsBlock => true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new PowerVar<CounterPower>(1m)
    ];

    public MomentumCounter() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<CounterPower>(
            choiceContext, Owner.Creature,
            DynamicVars["CounterPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["CounterPower"].UpgradeValueBy(1m);
    }
}