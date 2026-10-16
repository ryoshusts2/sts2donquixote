using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using DonQuixote.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Powers;

[RegisterPower]
public sealed class GuidePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    // 持有者（队友）打出单体攻击牌 → 堂吉诃德追击
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (cardPlay.Card.Type != CardType.Attack) return;
        if (cardPlay.Card.TargetType == TargetType.AllEnemies) return;
        if (cardPlay.Target == null) return;
        if (!cardPlay.Target.IsAlive) return;

        var donQuixote = FindDonQuixote();
        if (donQuixote == null) return;

        await CreatureCmd.Damage(
            choiceContext, cardPlay.Target, 6m,
            ValueProp.Unpowered, donQuixote.Creature);
        await PowerCmd.Apply<CripplePower>(
            choiceContext, cardPlay.Target, 1m, donQuixote.Creature, null);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }

    private Player? FindDonQuixote()
    {
        var runState = Owner.Player?.RunState;
        if (runState == null) return null;
        foreach (var p in runState.Players)
        {
            if (p.Character is DonQuixoteCharacter) return p;
        }
        return null;
    }
}