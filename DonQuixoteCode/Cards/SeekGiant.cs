using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using DonQuixote.Characters;
using DonQuixote.Phases;
using DonQuixote.Powers;
using DonQuixote.Stance;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class SeekGiant : ModCardTemplate, IStanceCard, IPhase2Card
{
    public StanceType Stance => StanceType.Rest;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("MaxPick", 1m)
    ];

    public SeekGiant() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DauntlessPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, this);

        var top = PileType.Draw.GetPile(Owner).Cards.Take(3).ToList();
        if (top.Count == 0) return;

        int maxPick = DynamicVars["MaxPick"].IntValue;
        if (maxPick <= 0) return;

        if (maxPick == 1)
        {
            // 未升级：用单选 UI（带跳过）
            var picked = await CardSelectCmd.FromChooseACardScreen(choiceContext, top, Owner, canSkip: true);
            if (picked != null)
            {
                await CardPileCmd.Add(picked, PileType.Hand);
            }
        }
        else
        {
            // 升级：用多选 UI
            var prompt = new LocString("card_selection", "选择要加入手牌的卡（可跳过）");
            var prefs = new CardSelectorPrefs(prompt, 0, maxPick);
            var pickedList = (await CardSelectCmd.FromSimpleGrid(
                choiceContext, top, Owner, prefs)).ToList();
            foreach (var card in pickedList)
            {
                await CardPileCmd.Add(card, PileType.Hand);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MaxPick"].UpgradeValueBy(2m);
    }
}