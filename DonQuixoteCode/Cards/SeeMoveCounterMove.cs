using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using DonQuixote.Characters;
using DonQuixote.Phases;
using DonQuixote.Stance;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class SeeMoveCounterMove : ModCardTemplate, IStanceCard, IPhase2Card
{
    public StanceType Stance => StanceType.Rest;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[0];

    public SeeMoveCounterMove() : base(3, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        bool wasAttacking = cardPlay.Target.Monster?.IntendsToAttack == true;

        await CreatureCmd.Stun(cardPlay.Target);

        if (wasAttacking)
        {
            await PlayerCmd.GainEnergy(1, Owner);
            await CardPileCmd.Draw(choiceContext, 1m, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}