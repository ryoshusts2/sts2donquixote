using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using DonQuixote.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Relics;

[RegisterRelic(typeof(DonQuixoteRelicPool))]
public sealed class FantasyIdentityCrimsonKnight : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    // 每次玩家回合开始（也就是每场战斗的第一回合以及后续回合），检查并强制替换二阶段立绘
    public override Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player) return Task.CompletedTask;

        try
        {
            var creatureNode = Owner.Creature.GetCreatureNode();
            if (creatureNode != null)
            {
                var sprite = creatureNode.FindChild("Visuals", true, false) as Sprite2D;
                if (sprite != null)
                {
                    var phase2Tex = GD.Load<Texture2D>($"{Entry.ResPath}/images/characters/DonQuixote_idle_phase2.png");
                    if (phase2Tex != null && sprite.Texture != phase2Tex)
                    {
                        sprite.Texture = phase2Tex;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"[DonQuixote] 遗物替换二阶段立绘失败: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}