using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using DonQuixote.Characters;
using DonQuixote.Phases;
using DonQuixote.Powers;
using DonQuixote.Stance;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class PourOutWrath : ModCardTemplate, IStanceCard, IPhase2Card
{
    public StanceType Stance => StanceType.Charge;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<CripplePower>(1m),
        new PowerVar<BoneShatterPower>(1m)
    ];

    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    public PourOutWrath() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal missingHp = Owner.Creature.MaxHp - Owner.Creature.CurrentHp;
        if (missingHp < 0m) missingHp = 0m;

        await DamageCmd.Attack(missingHp)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);

        foreach (var enemy in CombatState.HittableEnemies.ToList())
        {
            await PowerCmd.Apply<CripplePower>(
                choiceContext, enemy, DynamicVars["CripplePower"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<BoneShatterPower>(
                choiceContext, enemy, DynamicVars["BoneShatterPower"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CripplePower"].UpgradeValueBy(1m);
        DynamicVars["BoneShatterPower"].UpgradeValueBy(1m);
    }
}