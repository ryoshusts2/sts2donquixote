using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using DonQuixote.Cards;
using DonQuixote.Powers;
using DonQuixote.Stance;

namespace DonQuixote.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
public static class HoverTipPatches
{
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance == null) return;

        var extraTips = new List<IHoverTip>(__result ?? Enumerable.Empty<IHoverTip>());

        // 1. 动态挂载卡牌涉及的自定义 Power
        if (__instance is DuelOfHonor) extraTips.Add(HoverTipFactory.FromPower<FairDuelPower>());
        if (__instance is ReinInAndTurn) extraTips.Add(HoverTipFactory.FromPower<ReinInMarkerPower>());
        if (__instance is ChargingUp) extraTips.Add(HoverTipFactory.FromPower<ChargingUpPower>());
        if (__instance is KnightingCeremony) extraTips.Add(HoverTipFactory.FromPower<DauntlessPower>());
        if (__instance is KnightlyTraining) extraTips.Add(HoverTipFactory.FromPower<KnightlyTrainingPower>());
        if (__instance is Mastery) extraTips.Add(HoverTipFactory.FromPower<MasteryPower>());
        
        if (__instance is InTheNameOfTheKnight) 
        {
            extraTips.Add(HoverTipFactory.FromPower<DauntlessPower>());
            extraTips.Add(HoverTipFactory.FromPower<KnightOathPower>());
        }
        
        if (__instance is DauntlessCharge) extraTips.Add(HoverTipFactory.FromPower<DauntlessPower>());
        if (__instance is FantasyCharge) extraTips.Add(HoverTipFactory.FromPower<CripplePower>());
        if (__instance is MomentumCounter) extraTips.Add(HoverTipFactory.FromPower<CounterPower>());
        if (__instance is IronCavalryTrample) extraTips.Add(HoverTipFactory.FromPower<BoneShatterPower>());
        if (__instance is YouShallNotBlockMe) extraTips.Add(HoverTipFactory.FromPower<TargetPower>());
        if (__instance is DuelMelody) extraTips.Add(HoverTipFactory.FromPower<CripplePower>());
        if (__instance is AreYouWorthy) extraTips.Add(HoverTipFactory.FromPower<CounterPower>());
        if (__instance is Weakness) extraTips.Add(HoverTipFactory.FromPower<CounterPower>());
        if (__instance is MoveTooSlow) extraTips.Add(HoverTipFactory.FromPower<SlowMoverPower>());
        
        if (__instance is CruelMethods) 
        {
            extraTips.Add(HoverTipFactory.FromPower<BoneShatterPower>());
            extraTips.Add(HoverTipFactory.FromPower<CripplePower>());
        }
        
        if (__instance is ChargeTheGiant) extraTips.Add(HoverTipFactory.FromPower<CripplePower>());
        
        if (__instance is PourOutWrath) 
        {
            extraTips.Add(HoverTipFactory.FromPower<CripplePower>());
            extraTips.Add(HoverTipFactory.FromPower<BoneShatterPower>());
        }
        
        if (__instance is PierceThrough) extraTips.Add(HoverTipFactory.FromPower<CripplePower>());
        if (__instance is TagAlongForNow) extraTips.Add(HoverTipFactory.FromPower<GuidePower>());
        if (__instance is MuscleMemory) extraTips.Add(HoverTipFactory.FromPower<MuscleMemoryPower>());
        if (__instance is SeekGiant) extraTips.Add(HoverTipFactory.FromPower<DauntlessPower>());

        // 2. 动态挂载姿态弹窗
        if (__instance is IStanceCard stanceCard)
        {
            switch (stanceCard.Stance)
            {
                case StanceType.Charge:
                    extraTips.Add(HoverTipFactory.FromPower<ChargeStancePower>());
                    break;
                case StanceType.Counter:
                    extraTips.Add(HoverTipFactory.FromPower<CounterStancePower>());
                    break;
                case StanceType.Rest:
                    extraTips.Add(HoverTipFactory.FromPower<RestStancePower>());
                    break;
                case StanceType.Neutral:
                    extraTips.Add(HoverTipFactory.FromPower<NeutralStancePower>());
                    break;
            }
        }

        // 直接返回，不强求去重，防止报错
        __result = extraTips;
    }
}