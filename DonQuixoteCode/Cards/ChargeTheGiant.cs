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
public sealed class ChargeTheGiant : ModCardTemplate, IStanceCard, IPhase2Card
{
    public StanceType Stance => StanceType.Charge;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15m, ValueProp.Move),
        new PowerVar<CripplePower>(2m)
    ];

    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    public ChargeTheGiant() : base(0, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await PowerCmd.Apply<CripplePower>(
            choiceContext, cardPlay.Target, DynamicVars["CripplePower"].BaseValue, Owner.Creature, this);

        // 自我伤害：5 点，穿透格挡
        await CreatureCmd.Damage(
            choiceContext, Owner.Creature, 5m,
            ValueProp.Unblockable | ValueProp.Unpowered, null);
    }

    protected override void OnUpgrade()
    {
        // 升级效果：移除消耗（用原版 RemoveKeyword）
        RemoveKeyword(CardKeyword.Exhaust);
    }
}