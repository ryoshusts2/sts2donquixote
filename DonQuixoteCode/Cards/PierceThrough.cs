using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using DonQuixote.Characters;
using DonQuixote.Phases;
using DonQuixote.Powers; // 引入残废能力
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
        new DynamicVar("BonusHits", 0m) // 记录额外次数
    ];

    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    public PierceThrough() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 总打击次数 = 基础消耗(X) + 升级额外次数
        int hits = ResolveEnergyXValue() + DynamicVars["BonusHits"].IntValue;
        if (hits <= 0) return;

        // 1. 执行多段伤害（动作连贯）
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hits)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 2. 结算完毕后，统一施加 次数×2 的残废
        await PowerCmd.Apply<CripplePower>(
            choiceContext, 
            cardPlay.Target, 
            hits * 2m, 
            Owner.Creature, 
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BonusHits"].UpgradeValueBy(2m);
    }
}