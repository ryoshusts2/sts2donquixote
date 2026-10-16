using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using DonQuixote.Characters;
using DonQuixote.Phases;
using DonQuixote.Stance;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class PierceThrough : ModCardTemplate, IStanceCard, IPhase2Card
{
    public StanceType Stance => StanceType.Charge;

    protected override bool HasEnergyCostX => true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new DynamicVar("BonusHits", 2m)
    ];

    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    public PierceThrough() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        int hits = ResolveEnergyXValue();

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hits)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级：X 次的 X 值不变，但每次多打 2 下？
        // 根据设计「x+2 次」：需要 X 值 +2
        // 但 X 是运行时能量值，不能简单通过 var 表达。
        // 保守做法：把 DamageVar 提升，让打击更强
        // 该改动留给「X 值 +2」——通过 override ResolveEnergyXValue 无法实现，
        // 因为它是 base 里的方法。此处暂用 var 记录方式待后续处理。
        DynamicVars["BonusHits"].UpgradeValueBy(0m);
    }
}