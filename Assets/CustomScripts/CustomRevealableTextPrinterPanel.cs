using UnityEngine;

namespace Naninovel.UI // 元のnamespaceを維持
{
    public class CustomRevealableTextPrinterPanel : RevealableTextPrinterPanel
    {
        [Header("Custom Settings")] // インスペクターで見やすくするためのヘッダー
        [Tooltip("ダイアログボックスの線など、キャラクターの色を反映させたいImageを制御するコントローラー。")]
        [SerializeField] private CharacterLineColorController lineImageController; // ダイアログの線のImageを制御するコンポーネント

        // AffiliationPanel の参照 (既存のコードから)
        protected virtual AffiliationPanel AffilicationPanel => affiliationPanel;
        [SerializeField] private AffiliationPanel affiliationPanel; // インスペクターで設定するAffiliationPanel (既存のコードから)


        protected override void SetMessageAuthor(MessageAuthor author)
        {
            base.SetMessageAuthor(author);

            // ICharacterManager を CustomCharacterManager として取得
            // CharacterManager プロパティは ICharacterManager 型なのでキャストが必要
            var customCharacterManager = CharacterManager as CustomCharacterManager; 

            // AffiliationPanel の処理 (既存のコードをベースに)
            if (customCharacterManager != null && affiliationPanel != null)
            {
                var affiliation = customCharacterManager.GetAffiliation(author.Id);
                SetAffiliationText(affiliation);
                affiliationPanel.gameObject.SetActive(!string.IsNullOrEmpty(affiliation));
                // affiliationPanel.TextColor = AuthorMeta.NameColor; // この行は元のコードにありましたが、今回の色の主題とは別なのでコメントアウト推奨。必要なら残してください。
            }
            else if (affiliationPanel != null)
            {
                SetAffiliationText("");
                affiliationPanel.gameObject.SetActive(false);
            }

            // CharacterLineColorController の処理 (新規追加)
            if (lineImageController != null)
            {
                if (customCharacterManager != null && !string.IsNullOrEmpty(author.Id))
                {
                    lineImageController.UpdateLineColor(author.Id, customCharacterManager);
                }
                else
                {
                    // 発言者がいない、またはCustomCharacterManagerでない場合はデフォルト色に
                    lineImageController.ResetToDefaultColor();
                }
            }
        }

        // SetAffiliationText メソッド (既存のコードから)
        protected virtual void SetAffiliationText(string text)
        {
            if (!AffilicationPanel) return;

            var isActive = !string.IsNullOrWhiteSpace(text);
            AffilicationPanel.gameObject.SetActive(isActive);
            if (!isActive) return;

            AffilicationPanel.Text = text;
        }

        // OnDisableなどでコントローラーの状態をリセットすることも検討できます
        // 例えば、プリンターが非表示になったら線の色をデフォルトに戻すなど
        protected override void OnDisable()
        {
            base.OnDisable();
            if (lineImageController != null)
            {
                lineImageController.ResetToDefaultColor();
            }
        }
    }
}