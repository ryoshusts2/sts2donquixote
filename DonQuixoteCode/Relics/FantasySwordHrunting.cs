using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using DonQuixote.Characters;
using DonQuixote.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Relics;

[RegisterRelic(typeof(DonQuixoteRelicPool))]
public sealed class FantasySwordHrunting : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner) return;
        if (cardPlay.Card.Type != CardType.Attack) return;

        await PowerCmd.Apply<StrengthPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, null);
        await PowerCmd.Apply<TempStrengthMarkerPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, null);
        await PowerCmd.Apply<CripplePower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, null);
    }
}