using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;

namespace DonQuixote.RestSite;

[HarmonyPatch(typeof(RestSiteOption))]
public static class CraftPatches
{
    [HarmonyPatch(nameof(RestSiteOption.Generate))]
    [HarmonyPostfix]
    public static void Postfix_Generate(Player player, ref ICollection<RestSiteOption> __result)
    {
        try
        {
            if (__result == null) __result = new List<RestSiteOption>();
            if (__result.Any(o => o.OptionId == CraftOption.IdKey)) return;

            // 只有堂吉诃德角色才加这个选项
            if (player.Character is not Characters.DonQuixoteCharacter) return;

            __result.Add(new CraftOption(player));
        }
        catch (Exception ex)
        {
            Godot.GD.Print($"[DonQuixote] 生成制作选项报错: {ex.Message}\n{ex.StackTrace}");
        }
    }
}