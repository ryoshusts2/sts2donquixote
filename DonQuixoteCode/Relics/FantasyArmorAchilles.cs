using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using DonQuixote.Characters;
using DonQuixote.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Relics;

[RegisterRelic(typeof(DonQuixoteRelicPool))]
public sealed class FantasyArmorAchilles : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    // 单次伤害上限 15
    public override decimal ModifyDamageCap(
        Creature? target, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target != Owner.Creature) return decimal.MaxValue;
        return 15m;
    }

    // 残废 ≥ 25 时，低于 15 的伤害提高到 15
    public override decimal ModifyDamageAdditive(
        Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target != Owner.Creature) return 0m;

        var cripple = Owner.Creature.GetPower<CripplePower>();
        if (cripple == null || cripple.Amount < 25) return 0m;
        if (amount >= 15m) return 0m;

        return 15m - amount;
    }
}