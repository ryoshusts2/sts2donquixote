using System;
using MegaCrit.Sts2.Core.Models;

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
        // 9 = 吾名堂吉诃德 —— 批 6c 未实现，暂时返回 null
        _ => null
    };

    public static ModelId? GetIdByIndex(int index) => index switch
    {
        1 => ModelDb.GetId<Cards.FirstExpedition>(),
        2 => ModelDb.GetId<Cards.FirstDuel>(),
        3 => ModelDb.GetId<Cards.FirstVictory>(),
        4 => ModelDb.GetId<Cards.FirstFailure>(),
        5 => ModelDb.GetId<Cards.FirstWound>(),
        6 => ModelDb.GetId<Cards.FirstNearDeath>(),
        7 => ModelDb.GetId<Cards.Unacknowledged>(),
        8 => ModelDb.GetId<Cards.Absurd>(),
        _ => null
    };
}