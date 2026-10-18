using System;
using System.Linq;
using System.Threading.Tasks;
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
        if (IsUpgraded)
        {
            // 升级后：对全体敌人施加
            await PowerCmd.Apply<TargetPower>(
                choiceContext, 
                Owner.Creature.CombatState.HittableEnemies, 
                1m, 
                Owner.Creature, 
                this);
        }
        else
        {
            // 未升级：对单体施加
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await PowerCmd.Apply<TargetPower>(
                choiceContext, 
                cardPlay.Target, 
                1m, 
                Owner.Creature, 
                this);
        }
    }

    protected override void OnUpgrade()
    {
        // 逻辑已在 OnPlay 里通过 IsUpgraded 处理
    }
}