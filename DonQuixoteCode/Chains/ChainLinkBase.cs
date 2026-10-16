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

    // 用后从战斗中消耗（不进弃牌堆）
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

        // 只记录「本卡已用」——具体移除 + 发牌延后到战斗结束
        var dream = Owner.Relics.FirstOrDefault(r => r is UnwillingDreamRelic) as UnwillingDreamRelic;
        if (dream != null)
        {
            UnwillingDreamRelic.PendingChainIndex[dream] = ChainIndex;
            UnwillingDreamRelic.PendingChainUpgraded[dream] = IsUpgraded;
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}