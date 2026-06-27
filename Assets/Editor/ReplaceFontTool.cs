using UnityEngine;
using UnityEditor;
using TMPro;

public class ReplaceFontTool : EditorWindow
{
    [MenuItem("Tools/Replace TMP Font on Scene")]
    static void ReplaceFont()
    {
        // Выберите свой новый шрифтовой ассет в Project Window
        TMP_FontAsset newFont = Selection.activeObject as TMP_FontAsset;
        if (newFont == null)
        {
            Debug.LogError("Сначала выделите в Project окне ваш новый TMP Font Asset!");
            return;
        }

        // Найти все объекты с TMP_Text на сцене (включая неактивные)
        TMP_Text[] allTexts = FindObjectsOfType<TMP_Text>(true);
        int count = 0;
        foreach (var tmp in allTexts)
        {
            Undo.RecordObject(tmp, "Replace Font");
            tmp.font = newFont;
            count++;
        }
        Debug.Log($"Шрифт заменён на {newFont.name} для {count} объектов.");
    }
}