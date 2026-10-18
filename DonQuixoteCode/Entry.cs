using System;
using System.Reflection;
using HarmonyLib;
using Godot;
using STS2RitsuLib;

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
            // 注册 Godot 脚本和 Mod 程序集
            RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, null);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[DonQuixote] EnsureGodotScriptsRegistered failed: {ex.Message}");
        }

        // 注释掉了找不到的 ModTypeDiscoveryHub
        // try
        // {
        //     ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        // }
        // catch (Exception ex)
        // {
        //     GD.PrintErr($"[DonQuixote] ModTypeDiscoveryHub failed: {ex.Message}");
        // }

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