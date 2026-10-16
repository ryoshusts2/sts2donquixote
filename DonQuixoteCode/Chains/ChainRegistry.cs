using MegaCrit.Sts2.Core.Models;

namespace DonQuixote.Chains;

public static class ChainRegistry
{
    public static CardModel? GetCardByIndex(int index) => index switch
    {
        1 => ModelDb.Card<Cards.AdventureBegins>(),
        2 => ModelDb.Card<Cards.FirstExpedition>(),
        3 => ModelDb.Card<Cards.FirstDuel>(),
        4 => ModelDb.Card<Cards.FirstVictory>(),
        5 => ModelDb.Card<Cards.FirstFailure>(),
        6 => ModelDb.Card<Cards.FirstWound>(),
        7 => ModelDb.Card<Cards.FirstNearDeath>(),
        8 => ModelDb.Card<Cards.Unacknowledged>(),
        9 => ModelDb.Card<Cards.Absurd>(),
        10 => ModelDb.Card<Cards.MyNameIsDonQuixote>(),
        _ => null
    };
}