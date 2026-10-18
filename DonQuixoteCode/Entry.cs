using System;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using STS2RitsuLib;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Loader; // 补上这个命名空间，解决 ModTypeDiscoveryHub 找不到的问题

namespace DonQuixote;

public static class Entry
{
    public const string ModId = "DonQuixote";
    public static readonly string ResPath = $"res://{ModId}";
    
    // 补上 Logger 实例，解决对象引用报错
    public static readonly Logger Logger = new Logger(ModId);

    public static void Init()
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

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