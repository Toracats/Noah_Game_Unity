using Naninovel;
using UnityEngine;
public class CharacterAffiliationData : CustomMetadata<SpriteCharacter>
{
    [Tooltip("キャラクターの所属 (例: 学校名、組織名など)")]
    public string Affiliation = ""; // 所属を保存する文字列フィールド
    [Tooltip("キャラクターのUIの線に使う色")]
    public string CharacterLineColor = ""; // キャラクターの色を保存するColorフィールド (デフォルトは白)
}