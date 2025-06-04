using UnityEngine;
using System.Collections.Generic; // IReadOnlyList のため
using Naninovel; // PrintedMessage のため

namespace Naninovel.UI
{
    public class CustomRevealableTextPrinterPanel : RevealableTextPrinterPanel
    {
        [Header("Custom Settings")]
        [SerializeField] private CharacterLineColorController lineImageController;

        [Tooltip("AuthorNamePanelとAffiliationPanelを内包するコンテナオブジェクト（例: Gradation）。両パネルが非表示の場合にこのコンテナも非表示にします。")]
        [SerializeField] private GameObject gradationObject; // InfoContainer から Gradation に名前変更 (ユーザーの指摘に合わせて)

        [Tooltip("表示/非表示を Gradation オブジェクトと連動させたい Line オブジェクト。")]
        [SerializeField] private GameObject lineObject; // インスペクターで Line オブジェクトをアサインしてください

        [SerializeField] private AffiliationPanel affiliationPanel;

        protected override void SetMessageAuthor(MessageAuthor author)
        {
            base.SetMessageAuthor(author);

            UpdateGradationAndLineVisibility(); // メソッド名を変更して呼び出し

            var customCharacterManager = CharacterManager as CustomCharacterManager;

            if (customCharacterManager != null && affiliationPanel != null)
            {
                var affiliation = customCharacterManager.GetAffiliation(author.Id);
                SetAffiliationText(affiliation);
            }
            else if (affiliationPanel != null)
            {
                SetAffiliationText("");
            }

            if (lineImageController != null)
            {
                if (customCharacterManager != null && !string.IsNullOrEmpty(author.Id))
                {
                    lineImageController.UpdateLineColor(author.Id, customCharacterManager);
                }
                else
                {
                    lineImageController.ResetToDefaultColor();
                }
            }

        }

        protected virtual void SetAffiliationText(string text)
        {
            if (affiliationPanel == null) return;

            var isActive = !string.IsNullOrWhiteSpace(text);

            if (!isActive)
            {
                affiliationPanel.gameObject.SetActive(false); // 非アクティブならテキストセット前に非表示にして終了
                                                              // affiliationPanel.Text = string.Empty; // 必要であればテキストもクリア
                return;
            }

            // パネルが非表示だった場合、まず表示状態にする
            if (!affiliationPanel.gameObject.activeSelf)
            {
                affiliationPanel.gameObject.SetActive(true);
            }

            affiliationPanel.Text = text; // テキストをセット
        }

        // Gradation と Line の表示/非表示をまとめて制御するメソッド
        protected virtual void UpdateGradationAndLineVisibility()
        {
            // gradationObject が設定されていなければ何もしない (エラー回避)
            if (gradationObject == null)
            {
                Debug.LogWarning("Gradation Object is not assigned in the inspector.", this);
                // lineObject のチェックもここで行うか、個別に行う
                if (lineObject == null) Debug.LogWarning("Line Object is not assigned in the inspector.", this);
                return;
            }

            bool isAuthorNamePanelActive = base.AuthorNamePanel != null && base.AuthorNamePanel.gameObject.activeSelf;
            bool isAffiliationPanelActive = affiliationPanel != null && affiliationPanel.gameObject.activeSelf;

            // AuthorNamePanel または AffiliationPanel のどちらかがアクティブなら、Gradation と Line を表示
            bool shouldBeActive = isAuthorNamePanelActive || isAffiliationPanelActive;

            gradationObject.SetActive(shouldBeActive);

            // lineObject が設定されていれば、Gradation と同じ状態に設定
            if (lineObject != null)
            {
                lineObject.SetActive(shouldBeActive);
            }
        }

        public override void SetMessages(IReadOnlyList<PrintedMessage> messages)
        {
            base.SetMessages(messages);
        }

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