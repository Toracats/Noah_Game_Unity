using Naninovel; // CustomCharacterManager を参照するため
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 指定されたImageの色を、キャラクターに基づいて変更するシンプルなコントローラー。
/// CustomCharacterManagerから色情報を取得します。
/// </summary>
[RequireComponent(typeof(Image))]
public class CharacterLineColorController : MonoBehaviour
{
    [Tooltip("色が適用されるImageコンポーネント。")]
    public Image targetImage;

    [Tooltip("キャラクターに色が定義されていない場合や、発言者がいない場合に使用されるデフォルトの色。")]
    public Color defaultLineColor = Color.white;

    void Awake()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (targetImage == null)
        {
            Debug.LogError("[CharacterLineColorController] Target Image is not set and no Image component found on this GameObject. Disabling component.", this);
            enabled = false;
            return;
        }
        // 初期状態はデフォルト色にしておく
        targetImage.color = defaultLineColor;
    }

    /// <summary>
    /// 指定されたキャラクターIDに基づいてImageの色を更新します。
    /// </summary>
    /// <param name="characterId">色を取得する対象のキャラクターID。nullや空の場合はデフォルト色が使用されます。</param>
    /// <param name="charManager">色情報を取得するためのCustomCharacterManagerのインスタンス。</param>
    public void UpdateLineColor(string characterId, CustomCharacterManager charManager)
    {
        if (targetImage == null) return; // targetImageがなければ何もしない

        if (charManager == null)
        {
            // Debug.LogWarning("[CharacterLineColorController] CustomCharacterManager instance is null. Using default color.");
            targetImage.color = defaultLineColor;
            return;
        }

        if (string.IsNullOrEmpty(characterId))
        {
            targetImage.color = defaultLineColor;
            return;
        }

        Color? lineColor = charManager.GetLineColor(characterId);
        if (lineColor.HasValue)
        {
            targetImage.color = lineColor.Value;
        }
        else
        {
            targetImage.color = defaultLineColor;
        }
    }

    /// <summary>
    /// Imageの色をデフォルト色にリセットします。
    /// </summary>
    public void ResetToDefaultColor()
    {
        if (targetImage != null)
        {
            targetImage.color = defaultLineColor;
        }
    }
}