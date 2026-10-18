using System;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using STS2RitsuLib;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DonQuixote;

public static class Entry
{
    public const string ModId = "DonQuixote";
    public static readonly string ResPath = $"res://{ModId}";

    public static void Init()
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        // Harmony 补丁应用（关键：应用 CardPoolPatch / CraftPatches / HoverTipPatches 等）
        try
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll(assembly);
            Logger.Info("DonQuixote Harmony patches applied.");
        }
        catch (Exception ex)
        {
            Logger.Error($"DonQuixote Harmony patch failed: {ex.Message}\n{ex.StackTrace}");
        }

        Logger.Info("DonQuixote initialized.");
    }
}