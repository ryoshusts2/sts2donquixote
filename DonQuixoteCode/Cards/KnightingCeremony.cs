using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using DonQuixote.Characters;
using DonQuixote.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DonQuixote.Cards;

[RegisterCard(typeof(DonQuixoteCardPool))]
public sealed class KnightingCeremony : ModCardTemplate
{
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DauntlessPower>(2m),
        new PowerVar<DexterityPower>(1m),
        new PowerVar<VulnerablePower>(1m)
    ];

    public KnightingCeremony() : base(2, CardType.Skill, CardRarity.Ancient, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DauntlessPower>(
            choiceContext, Owner.Creature,
            DynamicVars["DauntlessPower"].BaseValue, Owner.Creature, this);

        await PowerCmd.Apply<DexterityPower>(
            choiceContext, Owner.Creature,
            DynamicVars["DexterityPower"].BaseValue, Owner.Creature, this);

        // 升级后 VulnerablePower.BaseValue = 0，施加 0 层等于没施加
        await PowerCmd.Apply<VulnerablePower>(
            choiceContext, Owner.Creature,
            DynamicVars["VulnerablePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        // 把易伤层数改成 0，描述也随之显示 0
        DynamicVars["VulnerablePower"].UpgradeValueBy(-1m);
    }
}