using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using DonQuixote.Characters;
using DonQuixote.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Chains;

public abstract class ChainLinkBase : ModCardTemplate
{
    protected abstract int ChainIndex { get; }

    public override bool GainsBlock => true;

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

        var dream = Owner.Relics.FirstOrDefault(r => r is UnwillingDreamRelic) as UnwillingDreamRelic;
        if (dream != null)
        {
            UnwillingDreamRelic.PendingChainIndex[dream] = ChainIndex;
            UnwillingDreamRelic.PendingChainUpgraded[dream] = IsUpgraded;
        }

        var deckCard = this.DeckVersion;
        if (deckCard != null)
        {
            await CardPileCmd.RemoveFromDeck(deckCard, showPreview: false);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}