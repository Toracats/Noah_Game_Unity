using Naninovel;
using UnityEngine;
public class CharacterAffiliationData : CustomMetadata<SpriteCharacter>
{
    [Tooltip("キャラクターの所属 (例: 学校名、組織名など)")]
    public string Affiliation = ""; // 所属を保存する文字列フィールド
}
public class CharacterLineColorData : CustomMetadata<SpriteCharacter>
{
    [Tooltip("キャラクターのUI線の色")]
    public string LineColor = "#FFFFFF"; // セリフの色を保存する文字列フィールド
}