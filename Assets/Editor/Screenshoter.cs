using UnityEditor;
using UnityEngine;
using System.IO;

public class Screenshoter : EditorWindow
{
    private static string screenshotName = "Screenshot";
    private static int scaleFactor = 4;
    
    // Ctrl+R (Windows) / Cmd+R (Mac)
    [MenuItem("Tools/Screenshots/Take a picture %#p")]
    static void TakeScreenshot()
    {
        var screenshotsFolder = ScreenshotsFolder();
        var fullPath = FullPath(screenshotsFolder);

        ScreenCapture.CaptureScreenshot(fullPath, scaleFactor);
        
        Debug.Log($"✅ Скриншот сохранён: {fullPath}");
        Debug.Log($"📁 Папка проекта: {screenshotsFolder}");
    }

    [MenuItem("Tools/Screenshots/OpenFolder")]
    static void OpenFolder()
    {
        string projectFolder = Path.GetDirectoryName(Application.dataPath);
        string screenshotsFolder = Path.Combine(projectFolder, "Screenshots");
        EditorUtility.RevealInFinder(screenshotsFolder);
    }

    private static string ScreenshotsFolder()
    {
        string projectFolder = Path.GetDirectoryName(Application.dataPath);
        string screenshotsFolder = Path.Combine(projectFolder, "Screenshots");
        
        if (!Directory.Exists(screenshotsFolder))
            Directory.CreateDirectory(screenshotsFolder);
        return screenshotsFolder;
    }

    private static string FullPath(string screenshotsFolder)
    {
        string fileName = $"{screenshotName}_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        return Path.Combine(screenshotsFolder, fileName);
    }
}