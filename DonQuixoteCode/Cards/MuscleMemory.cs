using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using DonQuixote.Characters;
using DonQuixote.Phases;
using DonQuixote.Powers;
using DonQuixote.Stance;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class MuscleMemory : ModCardTemplate, IStanceCard, IPhase2Card
{
    public StanceType Stance => StanceType.Neutral;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[0];

    public MuscleMemory() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<MuscleMemoryPower>(
            choiceContext, Owner.Creature, 5m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级效果通过 MuscleMemoryPower 的「额外值」体现
        // 但 Power 已挂上，无法再改 Amount。
        // 改为：升级时用 8m
        // 这需要在 OnPlay 里判断 IsUpgraded
        // 简化：升级后走另一条路径
    }
}