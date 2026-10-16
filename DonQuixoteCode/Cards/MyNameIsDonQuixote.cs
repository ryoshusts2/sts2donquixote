using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using DonQuixote.Characters;
using DonQuixote.Powers;
using DonQuixote.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class MyNameIsDonQuixote : ModCardTemplate
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[0];

    public MyNameIsDonQuixote() : base(3, CardType.Skill, CardRarity.Ancient, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获得额外回合
        await PowerCmd.Apply<ExtraTurnPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, this);

        // 2. 消耗手/抽/弃所有卡
        var allInCombat = new List<CardModel>();
        allInCombat.AddRange(PileType.Hand.GetPile(Owner).Cards.ToList());
        allInCombat.AddRange(PileType.Draw.GetPile(Owner).Cards.ToList());
        allInCombat.AddRange(PileType.Discard.GetPile(Owner).Cards.ToList());
        foreach (var c in allInCombat)
        {
            await CardCmd.Exhaust(choiceContext, c);
        }

        // 3. 移除卡组中所有卡
        var deckCards = PileType.Deck.GetPile(Owner).Cards.ToList();
        if (deckCards.Count > 0)
        {
            await CardPileCmd.RemoveFromDeck(deckCards);
        }

        // 4. 加入 20 张二阶段固定卡到卡组
        var phase2Cards = new List<CardModel>();
        for (int i = 0; i < 4; i++) phase2Cards.Add(Owner.RunState.CreateCard<FantasyCharge>(Owner));
        for (int i = 0; i < 4; i++) phase2Cards.Add(Owner.RunState.CreateCard<MomentumCounter>(Owner));
        phase2Cards.Add(Owner.RunState.CreateCard<IronCavalryTrample>(Owner));
        phase2Cards.Add(Owner.RunState.CreateCard<ChargeToDeath>(Owner));
        for (int i = 0; i < 2; i++) phase2Cards.Add(Owner.RunState.CreateCard<SeekGiant>(Owner));
        phase2Cards.Add(Owner.RunState.CreateCard<YouShallNotBlockMe>(Owner));
        for (int i = 0; i < 2; i++) phase2Cards.Add(Owner.RunState.CreateCard<DuelMelody>(Owner));
        phase2Cards.Add(Owner.RunState.CreateCard<CompleteTheQuest>(Owner));
        phase2Cards.Add(Owner.RunState.CreateCard<PourOutWrath>(Owner));
        for (int i = 0; i < 2; i++) phase2Cards.Add(Owner.RunState.CreateCard<ActOnOpportunity>(Owner));
        phase2Cards.Add(Owner.RunState.CreateCard<FinalChargePrep>(Owner));

        foreach (var c in phase2Cards)
        {
            await CardPileCmd.Add(c, PileType.Deck);
        }

        // 5. 替换英雄的战利品 → 英雄的殊荣
        var heroRelic = Owner.Relics.FirstOrDefault(r => r is DonQuixoteRelic);
        if (heroRelic != null)
        {
            await RelicCmd.Remove(heroRelic);
        }
        var glory = ModelDb.Relic<HeroGloryRelic>().ToMutable();
        await RelicCmd.Obtain(glory, Owner, Owner.Relics.Count);

        // 6. 替换不愿清醒的梦 → 不会醒来的梦
        var dreamRelic = Owner.Relics.FirstOrDefault(r => r is UnwillingDreamRelic);
        if (dreamRelic != null)
        {
            await RelicCmd.Remove(dreamRelic);
        }
        var neverWake = ModelDb.Relic<NeverWakeDreamRelic>().ToMutable();
        await RelicCmd.Obtain(neverWake, Owner, Owner.Relics.Count);

        // 7. 失去所有粗糙废料遗物
        var roughScraps = Owner.Relics
            .Where(r => r.Id.Entry.StartsWith("DON_QUIXOTE_RELIC_ROUGH_SCRAP"))
            .ToList();
        foreach (var r in roughScraps)
        {
            await RelicCmd.Remove(r);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}