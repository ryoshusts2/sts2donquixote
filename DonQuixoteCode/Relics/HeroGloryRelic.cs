using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Rewards;
using DonQuixote.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace DonQuixote.Relics;

[RegisterRelic(typeof(DonQuixoteRelicPool))]
public sealed class HeroGloryRelic : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    // 账本：记录二阶段是否击杀敌人触发了奖励
    public static readonly SavedAttachedState<HeroGloryRelic, bool> PendingQuestReward =
        new("PendingQuestReward", _ => false);

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (player != Owner) return false;
        if (room == null) return false;
        if (!room.RoomType.IsCombatRoom()) return false;

        // 检查账本，如果有，就塞入遗物奖励
        if (PendingQuestReward[this])
        {
            rewards.Add(new RelicReward(player));
            PendingQuestReward[this] = false;
        }

        return true;
    }
}