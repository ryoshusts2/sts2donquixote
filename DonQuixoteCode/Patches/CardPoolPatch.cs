using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using DonQuixote.Characters;
using DonQuixote.Phases;
using DonQuixote.Relics;

namespace DonQuixote.Patches;

[HarmonyPatch(typeof(CardPoolModel))]
public static class CardPoolPatch
{
    [HarmonyPatch(nameof(CardPoolModel.GetUnlockedCards))]
    [HarmonyPostfix]
    public static IEnumerable<CardModel> Postfix_GetUnlockedCards(
        IEnumerable<CardModel> __result, CardPoolModel __instance)
    {
        if (__instance is not DonQuixoteCardPool) return __result;

        var runManager = RunManager.Instance;
        if (runManager == null || runManager.State == null) return __result;

        var player = runManager.State.Players.FirstOrDefault(p => p.Character is DonQuixoteCharacter);
        if (player == null) return __result;

        bool isPhase2 = player.Relics.Any(r => r is HeroGloryRelic);

        if (isPhase2)
        {
            return __result.Where(c => c is IPhase2Card);
        }
        return __result.Where(c => c is not IPhase2Card);
    }
}