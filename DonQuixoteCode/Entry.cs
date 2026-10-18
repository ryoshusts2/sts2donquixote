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
    
    // 提供 Logger 实例，避免对象引用报错
    public static readonly Logger Logger = new Logger(ModId);

    public static void Init()
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        
        // 自动注册 (如果需要 ModTypeDiscoveryHub，通常在这个命名空间下即可调用)
        // 如果还是报错，说明旧手机的代码里有不同的 using，但这句是标准写法
        try {
            ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        } catch {
            Logger.Info("ModTypeDiscoveryHub 注册失败或不存在，跳过。");
        }

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