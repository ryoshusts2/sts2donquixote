using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using DonQuixote.Characters;
using DonQuixote.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class InTheNameOfTheKnight : ModCardTemplate
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DauntlessPower>(3m)
    ];

    public InTheNameOfTheKnight() : base(3, CardType.Power, CardRarity.Rare, TargetType.AllAllies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);

        // 给自己打上「骑士之誓」标记：无畏减伤效果提升至 50%
        await PowerCmd.Apply<KnightOathPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, this);

        if (CombatState == null) return;

        // 所有玩家（自己 + 队友）获得无畏
        var targets = new List<Creature> { Owner.Creature };
        targets.AddRange(
            CombatState.GetTeammatesOf(Owner.Creature)
                .Where(c => c != null && c.IsAlive && c.IsPlayer && c != Owner.Creature));

        foreach (var teammate in targets)
        {
            await PowerCmd.Apply<DauntlessPower>(
                choiceContext, teammate, DynamicVars["DauntlessPower"].BaseValue, Owner.Creature, this);
        }

        // TODO: 队友受到伤害时转移 20% 给堂吉诃德
        // 需要跨 hook 存状态，等做多人模式时实现
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars["DauntlessPower"].UpgradeValueBy(1m);
    }
}