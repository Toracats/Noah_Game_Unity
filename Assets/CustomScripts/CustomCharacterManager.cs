namespace Naninovel
{
    /// <inheritdoc cref="ICharacterManager"/>
    [InitializeAtRuntime(@override: typeof(CharacterManager))] // CharacterManager の代わりにこのクラスを使用する
    public class CustomCharacterManager : CharacterManager // 親クラスを CharacterManager に変更
    {
        // CharacterManager と同じコンストラクタを呼び出す
        public CustomCharacterManager(CharactersConfiguration config, CameraConfiguration cameraConfig,
            ITextManager docs, ITextPrinterManager printers)
            : base(config, cameraConfig, docs, printers)
        {
        }

        // CustomCharacterManager に固有のメソッドのみを記述
        public virtual string GetAffiliation(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId)) return null;

            var meta = this.GetActorMetaOrDefault(characterId);
            if (!meta.HasName) return null;

            // CharacterMetadata から CharacterAffiliationData を取得
            var affiliationData = meta.GetCustomData<CharacterAffiliationData>();

            if (affiliationData != null && !string.IsNullOrEmpty(affiliationData.Affiliation))
            {
                var affiliationName = affiliationData.Affiliation;
                
                // 必要であれば、所属名に対しても式評価を行う (GetAuthorName と同様の処理)
                if (affiliationName.StartsWithFast("{") && affiliationName.EndsWithFast("}"))
                {
                    var expression = affiliationName.GetAfterFirst("{").GetBeforeLast("}");
                    affiliationName = ExpressionEvaluator.Evaluate<string>(expression, desc => Engine.Err($"Failed to evaluate '{characterId}' character affiliation: {desc}"));
                }
                return affiliationName;
            }

            // GetAuthorName と同様に、所属情報が見つからない場合は characterId を返すか、null を返すかなどを検討します。
            // ここでは null を返すようにしています。
            return null;
        }
    }
}
