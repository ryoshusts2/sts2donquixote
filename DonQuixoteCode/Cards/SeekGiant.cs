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

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[0];

    public SeekGiant() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DauntlessPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, this);

        var top = PileType.Draw.GetPile(Owner).Cards.Take(3).ToList();
        if (top.Count == 0) return;

        int maxPick = IsUpgraded ? 3 : 1;

        // 使用原版选择 UI（FromSimpleGrid）
        // prompt 用中文 key 作为 fallback 文本
        var prompt = new LocString("card_selection", "选择要加入手牌的卡（可跳过）");
        var prefs = new CardSelectorPrefs(prompt, 0, maxPick);

        var picked = (await CardSelectCmd.FromSimpleGrid(
            choiceContext, top, Owner, prefs)).ToList();

        foreach (var card in picked)
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }

        // 未选的卡自然留在抽牌堆原位 —— FromSimpleGrid 只是 UI，不移动卡
        // 但为保险起见，未选的卡位置由游戏自身维护，无需处理
    }

    protected override void OnUpgrade()
    {
        // 升级效果由 OnPlay 里的 IsUpgraded 判断实现
    }
}