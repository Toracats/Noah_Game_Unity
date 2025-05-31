using UnityEngine;

namespace Naninovel.UI
{
    /// <summary>
    /// Custom panel for displaying revealable text with additional affiliation information.
    /// </summary>
    public class CustomRevealableTextPrinterPanel : RevealableTextPrinterPanel
    {
        protected virtual AffiliationPanel AffilicationPanel => affiliationPanel;

        protected override void SetMessageAuthor(MessageAuthor author)
    {
        base.SetMessageAuthor(author); 

        var customCharacterManager = CharacterManager as CustomCharacterManager;

        if (customCharacterManager != null && affiliationPanel != null)
        {
            var affiliation = customCharacterManager.GetAffiliation(author.Id);
            SetAffiliationText(affiliation); 
            
            // 所属パネルの表示/非表示を制御
            affiliationPanel.gameObject.SetActive(!string.IsNullOrEmpty(affiliation));

            affiliationPanel.TextColor = AuthorMeta.NameColor; 
        }
        else // 所属を表示する必要がない場合（キャラクターがいない、カスタムマネージャーが設定されていないなど）
        {
            SetAffiliationText(""); // 所属テキストをクリア
            if (affiliationPanel != null) affiliationPanel.gameObject.SetActive(false); // パネルも非表示に
        }
    }

        [SerializeField] private AffiliationPanel affiliationPanel;
        [Tooltip("Image to display avatar of the currently printed text author (optional).")]

        protected virtual void SetAffiliationText(string text)
        {
            if (!AffilicationPanel) return;

            var isActive = !string.IsNullOrWhiteSpace(text);
            AffilicationPanel.gameObject.SetActive(isActive);
            if (!isActive) return;

            AffilicationPanel.Text = text;
        }
    }
}
