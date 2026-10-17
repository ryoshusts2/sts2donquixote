using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using DonQuixote.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Chains;

public abstract class ChainLinkBase : ModCardTemplate
{
    protected abstract int ChainIndex { get; }

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(20m, ValueProp.Move)
    ];

    protected ChainLinkBase() : base(3, CardType.Skill, CardRarity.Basic, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        var deckPile = CardPile.Get(PileType.Deck, Owner);

        // 1. 从持久 Deck 中移除“当前卡”
        var usedTemplate = ChainRegistry.GetCardByIndex(ChainIndex);
        if (usedTemplate != null)
        {
            var toRemove = deckPile.Cards.Where(c => c.Id.Equals(usedTemplate.Id)).Take(1).ToList();
            if (toRemove.Count > 0)
            {
                await CardPileCmd.RemoveFromDeck(toRemove);
            }
        }

        // 2. 获取“下一张卡”
        var nextTemplate = ChainRegistry.GetCardByIndex(ChainIndex + 1);
        if (nextTemplate != null)
        {
            // 2.1 加入持久 Deck
            var nextDeckCard = RunState.CreateCard(nextTemplate, Owner);
            if (IsUpgraded) CardCmd.Upgrade(nextDeckCard);
            await CardPileCmd.Add(nextDeckCard, deckPile);

            // 2.2 加入当前战斗的弃牌堆，让你即时能拿到（严格遵守交接文档：用 CombatState.CreateCard）
            var nextCombatCard = CombatState.CreateCard(nextTemplate, Owner);
            if (IsUpgraded) CardCmd.Upgrade(nextCombatCard);
            await CardPileCmd.AddGeneratedCardToCombat(nextCombatCard, PileType.Discard, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}