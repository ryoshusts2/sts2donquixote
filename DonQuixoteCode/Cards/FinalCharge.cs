using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using DonQuixote.Characters;
using DonQuixote.Phases;
using DonQuixote.Stance;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class FinalCharge : ModCardTemplate, IStanceCard, IPhase2Card
{
    public StanceType Stance => StanceType.Charge;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[0];

    public FinalCharge() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.Self, true)
    {
    }

    // 只做「消耗」，不做「加牌」——加牌放到 AfterCardPlayed
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var allCards = new List<CardModel>();
        allCards.AddRange(PileType.Hand.GetPile(Owner).Cards.ToList());
        allCards.AddRange(PileType.Draw.GetPile(Owner).Cards.ToList());
        allCards.AddRange(PileType.Discard.GetPile(Owner).Cards.ToList());

        foreach (var card in allCards)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
    }

    // 卡视觉打完后，再填满手牌
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card != this) return;

        while (PileType.Hand.GetPile(Owner).Cards.Count < CardPile.MaxCardsInHand)
        {
            var newCard = Owner.CombatState.CreateCard<ChargeTheGiant>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade()
    {
    }
}