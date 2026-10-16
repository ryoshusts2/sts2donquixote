using System;

namespace DonQuixote.Chains;

public static class ChainRegistry
{
    public static Type? GetTypeByIndex(int index) => index switch
    {
        1 => typeof(Cards.FirstExpedition),
        2 => typeof(Cards.FirstDuel),
        3 => typeof(Cards.FirstVictory),
        4 => typeof(Cards.FirstFailure),
        5 => typeof(Cards.FirstWound),
        6 => typeof(Cards.FirstNearDeath),
        7 => typeof(Cards.Unacknowledged),
        8 => typeof(Cards.Absurd),
        9 => typeof(Cards.MyNameIsDonQuixote),
        _ => null
    };
}