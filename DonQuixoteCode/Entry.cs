using System;
using System.Reflection;
using HarmonyLib;
using Godot;
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
        
        GD.Print("[DonQuixote] Initializing...");

        try
        {
            // 传入 null 或者实例，如果必须需要 Logger 实例，可能会报错，所以包在 try 里
            RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, null);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[DonQuixote] EnsureGodotScriptsRegistered failed: {ex.Message}");
        }

        try
        {
            ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[DonQuixote] ModTypeDiscoveryHub failed: {ex.Message}");
        }

        try
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll(assembly);
            GD.Print("[DonQuixote] Harmony patches applied.");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[DonQuixote] Harmony patch failed: {ex.Message}\n{ex.StackTrace}");
        }

        GD.Print("[DonQuixote] initialized.");
    }
}