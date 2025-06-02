using UnityEngine; // ColorUtility のために追加

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

            // CharacterMetadata から CustomData を取得
            var affiliationData = meta.GetCustomData<CustomData>();

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
            return null;
        }

        // 新しく追加するメソッド: キャラクターIDからラインカラーを取得する
        public virtual Color? GetLineColor(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId)) return null;

            var meta = this.GetActorMetaOrDefault(characterId);
            // CharacterMetadata に Name が設定されているかではなく、meta オブジェクト自体が有効かを確認
            // GetActorMetaOrDefault は見つからない場合デフォルトの CharacterMetadata を返すので、
            // カスタムデータを持っているかは GetCustomData の結果で判断する
            if (meta == null) return null; 

            var colorData = meta.GetCustomData<CustomData>();

            if (colorData != null && !string.IsNullOrEmpty(colorData.LineColor))
            {
                string colorString = colorData.LineColor;

                // 色文字列に対しても式評価を行う
                if (colorString.StartsWithFast("{") && colorString.EndsWithFast("}"))
                {
                    var expression = colorString.GetAfterFirst("{").GetBeforeLast("}");
                    // エラーハンドラは Action<string> なので値を返さない
                    string evaluatedColorString = ExpressionEvaluator.Evaluate<string>(expression, errorMsg => 
                    {
                        Engine.Warn($"Failed to evaluate line color expression '{expression}' for character '{characterId}': {errorMsg}. Using raw value '{colorData.LineColor}'.");
                    });

                    // 評価結果がnullまたは空の場合、元のcolorStringを使用するか、エラーとして処理するか選択
                    // ここでは評価結果が有効ならそれを使用し、そうでなければ元の文字列で続行
                    if (!string.IsNullOrEmpty(evaluatedColorString))
                    {
                        colorString = evaluatedColorString;
                    }
                    else if (evaluatedColorString == null) // 式評価自体が失敗した場合など
                    {
                        Engine.Warn($"Evaluated line color expression for character '{characterId}' resulted in null. Using raw value '{colorData.LineColor}'.");
                        // colorString は colorData.LineColor のまま
                    }
                }
                
                if (string.IsNullOrWhiteSpace(colorString))
                {
                     Engine.Warn($"Line color string for character '{characterId}' is empty after evaluation. Using default color.");
                     return null;
                }

                if (ColorUtility.TryParseHtmlString(colorString, out Color parsedColor))
                {
                    return parsedColor;
                }
                else
                {
                    Engine.Warn($"Failed to parse line color string '{colorString}' for character '{characterId}'. Please ensure it's a valid HTML color code (e.g., #RRGGBB) or a Unity color name. Using default color.");
                    return null; 
                }
            }
            // カラーデータがない場合やLineColorが空の場合はnullを返す
            return null;
        }
    }
}