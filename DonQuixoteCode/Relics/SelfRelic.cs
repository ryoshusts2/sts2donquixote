using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Relics;

[RegisterRelic(typeof(SharedRelicPool))]
public sealed class SelfRelic : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    private int _cardsPlayedThisTurn;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    // 进入房间 -3 最大生命值
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        await CreatureCmd.LoseMaxHp(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            3m,
            isFromCard: false);
    }

    // 一回合最多出 5 张牌
    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (autoPlayType != AutoPlayType.None) return true;
        if (card.Owner != Owner) return true;
        if (_cardsPlayedThisTurn >= 5) return false;
        return true;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner) _cardsPlayedThisTurn++;
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner.Creature)) _cardsPlayedThisTurn = 0;
        return Task.CompletedTask;
    }
}