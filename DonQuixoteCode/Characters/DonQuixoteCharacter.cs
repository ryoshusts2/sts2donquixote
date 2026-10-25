using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Godot;
using STS2RitsuLib.Scaffolding.Visuals;

namespace DonQuixote.Characters;

[RegisterCharacter]
public sealed class DonQuixoteCharacter : ModCharacterTemplate<DonQuixoteCardPool, DonQuixoteRelicPool, DonQuixotePotionPool>
{
    public static readonly Color ThemeColor = new(0.42f, 0.65f, 0.72f);

    private static readonly string SceneRoot = $"{Entry.ResPath}/scenes/characters";
    private static readonly string ImageRoot = $"{Entry.ResPath}/images/characters";
    private static readonly string CharacterScenePath = $"{SceneRoot}/DonQuixote_character.tscn";
    private static readonly string EnergyCounterScenePath = $"{SceneRoot}/DonQuixote_energy_counter.tscn";
    private static readonly string MerchantScenePath = $"{SceneRoot}/DonQuixote_merchant.tscn";
    private static readonly string RestSiteScenePath = $"{SceneRoot}/DonQuixote_rest_site.tscn";
    private static readonly string CharacterSelectBgScenePath = $"{SceneRoot}/DonQuixote_character_select_bg.tscn";

    public override Color NameColor => ThemeColor;
    public override Color EnergyLabelOutlineColor => new(0.08f, 0.18f, 0.24f);
    public override Color MapDrawingColor => ThemeColor;

    public override CharacterGender Gender => CharacterGender.Neutral;

    public override int StartingHp => 70;
    public override int StartingGold => 99;

    public override CharacterAssetProfile AssetProfile => new(
        Scenes: new CharacterSceneAssetSet(
            VisualsPath: CharacterScenePath,
            EnergyCounterPath: EnergyCounterScenePath,
            MerchantAnimPath: MerchantScenePath,
            RestSiteAnimPath: RestSiteScenePath),
        Ui: new CharacterUiAssetSet(
            IconTexturePath: $"{ImageRoot}/DonQuixote_character_icon.png",
            IconOutlineTexturePath: $"{ImageRoot}/DonQuixote_character_icon_outline.png",
            IconPath: $"{SceneRoot}/DonQuixote_icon.tscn",
            CharacterSelectBgPath: CharacterSelectBgScenePath,
            CharacterSelectIconPath: $"{ImageRoot}/DonQuixote_character_select.png",
            CharacterSelectLockedIconPath: $"{ImageRoot}/DonQuixote_character_select_locked.png",
            MapMarkerPath: $"{ImageRoot}/DonQuixote_map_marker.png"),
        VisualCues: ModVisualCues.CueSet()
            // 【核心修改】：只保留 idle，删掉 attack 和 hit，防止出牌时强制切回一阶段
            .Single("idle", $"{ImageRoot}/DonQuixote_idle.png")
            .Build());

    public override string? PlaceholderCharacterId => "ironclad";
    public override bool RequiresEpochAndTimeline => false;
    public override float AttackAnimDelay => 0f;
    public override float CastAnimDelay => 0f;

    protected override NCreatureVisuals? TryCreateCreatureVisuals()
    {
        return RitsuGodotNodeFactories.CreateFromScenePath<NCreatureVisuals>(
            CharacterScenePath);
    }

    public override List<string> GetArchitectAttackVfx()
    {
        return
        [
            "vfx/vfx_attack_blunt",
            "vfx/vfx_heavy_blunt",
            "vfx/vfx_attack_slash",
            "vfx/vfx_bloody_impact",
            "vfx/vfx_rock_shatter"
        ];
    }
}