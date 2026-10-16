using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Powers;

[RegisterPower]
public sealed class RestStancePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    // 喘息：进入无效果；退出 +1 费 + 抽 1
    public override async Task AfterRemoved(Creature oldOwner)
    {
        var ctx = new ThrowingPlayerChoiceContext();
        if (oldOwner.Player != null)
        {
            await PlayerCmd.GainEnergy(1, oldOwner.Player);
            await CardPileCmd.Draw(ctx, 1m, oldOwner.Player);
        }
    }
}