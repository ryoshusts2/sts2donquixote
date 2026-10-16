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
public sealed class YouShallNotBlockMe : ModCardTemplate, IStanceCard, IPhase2Card
{
    public StanceType Stance => StanceType.Neutral;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    public YouShallNotBlockMe() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<TargetPower>(
            choiceContext, cardPlay.Target, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级：目标类型从单体改为全体
        // 通过修改 TargetType 不可行，改为在 OnPlay 里遍历所有敌人
    }
}