using MegaCrit.Sts2.Core.Models;

namespace DonQuixote.Chains;

public static class ChainRegistry
{
    public static CardModel? GetCardByIndex(int index) => index switch
    {
        1 => ModelDb.Card<Cards.FirstExpedition>(),
        2 => ModelDb.Card<Cards.FirstDuel>(),
        3 => ModelDb.Card<Cards.FirstVictory>(),
        4 => ModelDb.Card<Cards.FirstFailure>(),
        5 => ModelDb.Card<Cards.FirstWound>(),
        6 => ModelDb.Card<Cards.FirstNearDeath>(),
        7 => ModelDb.Card<Cards.Unacknowledged>(),
        8 => ModelDb.Card<Cards.Absurd>(),
        9 => ModelDb.Card<Cards.MyNameIsDonQuixote>(),
        _ => null
    };
}