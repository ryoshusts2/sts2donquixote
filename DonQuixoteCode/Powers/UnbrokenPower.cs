using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Powers;

[RegisterPower]
public sealed class UnbrokenPower : ModPowerTemplate
{
    private bool _pendingImmunity;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    // 致死伤害免疫：保留 1 血
    public override decimal ModifyHpLostBeforeOsty(
        Creature target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return amount;
        if (Amount < 10) return amount;
        if (amount < target.CurrentHp) return amount;

        _pendingImmunity = true;
        return target.CurrentHp - 1;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext, Creature target, DamageResult result,
        ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return;
        if (!_pendingImmunity) return;
        _pendingImmunity = false;

        await PowerCmd.ModifyAmount(
            choiceContext, this, -10, null, null);
    }

    // 下个自己回合开始：回复等量生命。若溢出，则获得差值的不屈。
    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        if (Amount <= 0) return;

        // 1. 计算当前已损失的生命值
        decimal lostHp = Owner.MaxHp - Owner.CurrentHp;
        decimal healAmount = Amount;

        // 2. 执行回血，并移除当前的不屈
        await CreatureCmd.Heal(Owner, healAmount);
        await PowerCmd.Remove(this);

        // 3. 如果治疗量大于已损失生命值，说明溢出了
        if (healAmount > lostHp)
        {
            decimal overflow = healAmount - lostHp;
            
            // 获得溢出差值的不屈
            await PowerCmd.Apply<UnbrokenPower>(
                new ThrowingPlayerChoiceContext(), 
                Owner, 
                overflow, 
                Owner, 
                null);
        }
    }
}