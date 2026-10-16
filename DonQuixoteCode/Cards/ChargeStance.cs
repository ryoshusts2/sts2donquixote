using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using DonQuixote.Characters;
using DonQuixote.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class ChargeStance : ModCardTemplate
{
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(2m),
        new PowerVar<VulnerablePower>(2m)
    ];

    public ChargeStance() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        decimal weakAmount = DynamicVars["WeakPower"].BaseValue;
        decimal vulnAmount = DynamicVars["VulnerablePower"].BaseValue;

        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, weakAmount, Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, vulnAmount, Owner.Creature, this);

        var drawPile = PileType.Draw.GetPile(Owner).Cards.ToList();
        var strikes = drawPile.Where(c => c.Tags.Contains(CardTag.Strike)).ToList();
        foreach (var card in strikes)
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }

        // 升级：本回合冲锋攻击卡费用 -1
        if (IsUpgraded)
        {
            await PowerCmd.Apply<ChargeStanceDiscountPower>(
                choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果通过 OnPlay 里的 IsUpgraded 判断实现
    }
}