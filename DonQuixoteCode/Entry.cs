using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace DonQuixote;

[ModInitializer(nameof(Initialize))]
public partial class Entry
{
    // ModId 需要和 DonQuixote.json 里的 id 保持一致。
    // res://DonQuixote/... 里的 DonQuixote 是 PCK 资源目录，不是 C# namespace。
    public const string ModId = "DonQuixote";
    public const string ResPath = $"res://{ModId}";

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Godot C# 脚本注册
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);

        // RitsuLib 内容自动注册（扫描 [RegisterCard] / [RegisterRelic] 等）
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        // Harmony 补丁应用（关键：应用 CardPoolPatch / CraftPatches 等）
        try
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll(assembly);
            Logger.Info("DonQuixote Harmony patches applied.");
        }
        catch (System.Exception ex)
        {
            Logger.Error($"DonQuixote Harmony patch failed: {ex.Message}\n{ex.StackTrace}");
        }

        Logger.Info("DonQuixote initialized.");
    }
}
