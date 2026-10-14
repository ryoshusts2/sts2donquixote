using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Rooms;
using MegaCrit.Sts2.Core.Rewards;
using DonQuixote.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace DonQuixote.Relics;

[RegisterRelic(typeof(DonQuixoteRelicPool))]
[RegisterCharacterStarterRelic(typeof(DonQuixoteCharacter))]
public sealed class DonQuixoteRelic : ModRelicTemplate
{
    // 废料计数，随存档保存。默认 0。
    public static readonly SavedAttachedState<DonQuixoteRelic, int> ScrapCount =
        new("ScrapCount", _ => 0);

    public override RelicRarity Rarity => RelicRarity.Starter;

    // 遗物描述里的 {ScrapCount} 占位符，对应这个值。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("ScrapCount", ScrapCount[this])
    ];

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    // 战斗奖励改造：移除金币，掉落废料。
    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (player != Owner) return false;
        if (room == null) return false;
        if (!room.RoomType.IsCombatRoom()) return false;
        // 最终 Boss 房间不掉废料（原版逻辑，此处保留）
        if (room.RoomType == RoomType.Boss && player.RunState.CurrentActIndex >= player.RunState.Acts.Count - 1)
        {
            return false;
        }

        int scrap = room.RoomType switch
        {
            RoomType.Elite => 2,
            RoomType.Boss => 5,
            _ => 1
        };

        // 移除所有金币奖励
        rewards.RemoveAll(r => r is GoldReward);

        // 增加废料计数并刷新 UI
        ScrapCount[this] += scrap;
        DynamicVars["ScrapCount"].BaseValue = ScrapCount[this];

        return true;
    }

    // 进入房间：如果是商店，+2 废料
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType != RoomType.Shop) return Task.CompletedTask;

        ScrapCount[this] += 2;
        DynamicVars["ScrapCount"].BaseValue = ScrapCount[this];

        return Task.CompletedTask;
    }
}