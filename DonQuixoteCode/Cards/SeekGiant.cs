using System.Collections.Generic;
using System.Linq;
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
        var prompt = new LocString("rest_site_ui", "选择至多 " + maxPick + " 张加入手牌");
        var prefs = new CardSelectorPrefs(prompt, 0, maxPick);

        var picked = (await CardSelectCmd.FromSimpleGrid(
            choiceContext, top, Owner, prefs)).ToList();

        foreach (var card in picked)
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：可选数量从 1 变为 3
    }
}