using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using DonQuixote.Characters;
using DonQuixote.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Relics;

[RegisterRelic(typeof(DonQuixoteRelicPool))]
public sealed class FantasyMountDiomedes : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    // 战斗开始获得 3 层无畏
    public override async Task BeforeCombatStart()
    {
        await PowerCmd.Apply<DauntlessPower>(
            new ThrowingPlayerChoiceContext(), Owner.Creature, 3m, Owner.Creature, null);
    }
    // 「无畏对堂吉诃德的影响升至 50%」—— 在 DauntlessPower 里检测本遗物
    // 「每次获得无畏时 +5 残废」—— 在 DauntlessPower.AfterApplied 里检测本遗物
}