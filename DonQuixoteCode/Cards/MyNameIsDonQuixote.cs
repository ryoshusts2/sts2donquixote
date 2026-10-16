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
        Godot.GD.Print("[DonQuixote] ===== 吾名堂吉诃德 打出 =====");

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

        // 4. 加入 20 张二阶段卡（原版正确写法：Deck 用 RunState.CreateCard，Draw 用 CombatState.CreateCard）
        var deckPile = CardPile.Get(PileType.Deck, Owner);
        var drawPile = CardPile.Get(PileType.Draw, Owner);

        for (int i = 0; i < 4; i++)
        {
            await CardPileCmd.Add(RunState.CreateCard<FantasyCharge>(Owner), deckPile);
            await CardPileCmd.Add(CombatState.CreateCard<FantasyCharge>(Owner), drawPile);
        }
        for (int i = 0; i < 4; i++)
        {
            await CardPileCmd.Add(RunState.CreateCard<MomentumCounter>(Owner), deckPile);
            await CardPileCmd.Add(CombatState.CreateCard<MomentumCounter>(Owner), drawPile);
        }
        await CardPileCmd.Add(RunState.CreateCard<IronCavalryTrample>(Owner), deckPile);
        await CardPileCmd.Add(CombatState.CreateCard<IronCavalryTrample>(Owner), drawPile);
        await CardPileCmd.Add(RunState.CreateCard<ChargeToDeath>(Owner), deckPile);
        await CardPileCmd.Add(CombatState.CreateCard<ChargeToDeath>(Owner), drawPile);
        for (int i = 0; i < 2; i++)
        {
            await CardPileCmd.Add(RunState.CreateCard<SeekGiant>(Owner), deckPile);
            await CardPileCmd.Add(CombatState.CreateCard<SeekGiant>(Owner), drawPile);
        }
        await CardPileCmd.Add(RunState.CreateCard<YouShallNotBlockMe>(Owner), deckPile);
        await CardPileCmd.Add(CombatState.CreateCard<YouShallNotBlockMe>(Owner), drawPile);
        for (int i = 0; i < 2; i++)
        {
            await CardPileCmd.Add(RunState.CreateCard<DuelMelody>(Owner), deckPile);
            await CardPileCmd.Add(CombatState.CreateCard<DuelMelody>(Owner), drawPile);
        }
        await CardPileCmd.Add(RunState.CreateCard<CompleteTheQuest>(Owner), deckPile);
        await CardPileCmd.Add(CombatState.CreateCard<CompleteTheQuest>(Owner), drawPile);
        await CardPileCmd.Add(RunState.CreateCard<PourOutWrath>(Owner), deckPile);
        await CardPileCmd.Add(CombatState.CreateCard<PourOutWrath>(Owner), drawPile);
        for (int i = 0; i < 2; i++)
        {
            await CardPileCmd.Add(RunState.CreateCard<ActOnOpportunity>(Owner), deckPile);
            await CardPileCmd.Add(CombatState.CreateCard<ActOnOpportunity>(Owner), drawPile);
        }
        await CardPileCmd.Add(RunState.CreateCard<FinalChargePrep>(Owner), deckPile);
        await CardPileCmd.Add(CombatState.CreateCard<FinalChargePrep>(Owner), drawPile);

        Godot.GD.Print("[DonQuixote] Added 20 cards to Deck + Draw");

        // 5. 替换英雄的战利品 → 英雄的殊荣
        var heroRelic = Owner.Relics.FirstOrDefault(r => r is DonQuixoteRelic);
        if (heroRelic != null) await RelicCmd.Remove(heroRelic);
        await RelicCmd.Obtain(ModelDb.Relic<HeroGloryRelic>().ToMutable(), Owner, Owner.Relics.Count);

        // 6. 替换不愿清醒的梦 → 不会醒来的梦
        var dreamRelic = Owner.Relics.FirstOrDefault(r => r is UnwillingDreamRelic);
        if (dreamRelic != null) await RelicCmd.Remove(dreamRelic);
        await RelicCmd.Obtain(ModelDb.Relic<NeverWakeDreamRelic>().ToMutable(), Owner, Owner.Relics.Count);

        // 7. 失去所有粗糙废料遗物
        var roughScraps = Owner.Relics
            .Where(r => r.Id.Entry.StartsWith("DON_QUIXOTE_RELIC_ROUGH_SCRAP"))
            .ToList();
        foreach (var r in roughScraps) await RelicCmd.Remove(r);

        // 8. 获得 7 个幻想遗物
        await RelicCmd.Obtain(ModelDb.Relic<FantasySwordHrunting>().ToMutable(), Owner, Owner.Relics.Count);
        await RelicCmd.Obtain(ModelDb.Relic<FantasyShieldAegis>().ToMutable(), Owner, Owner.Relics.Count);
        await RelicCmd.Obtain(ModelDb.Relic<FantasyHelmetMambrino>().ToMutable(), Owner, Owner.Relics.Count);
        await RelicCmd.Obtain(ModelDb.Relic<FantasyArmorAchilles>().ToMutable(), Owner, Owner.Relics.Count);
        await RelicCmd.Obtain(ModelDb.Relic<FantasySpearLonginus>().ToMutable(), Owner, Owner.Relics.Count);
        await RelicCmd.Obtain(ModelDb.Relic<FantasyMountDiomedes>().ToMutable(), Owner, Owner.Relics.Count);
        await RelicCmd.Obtain(ModelDb.Relic<FantasyIdentityCrimsonKnight>().ToMutable(), Owner, Owner.Relics.Count);

        Godot.GD.Print("[DonQuixote] ===== 吾名堂吉诃德 完成 =====");
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}