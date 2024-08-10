using UnityEngine;
using UnityEditor;
using System.IO;

public class AnimationClipLoader : EditorWindow
{
    private string filePath = "Assets/AnimationContent/"; // Default path to search for animation files
    private string fileName = "animation.json"; // Default file name

    [MenuItem("Tools/Animation Clip Loader")]
    public static void ShowWindow()
    {
        GetWindow<AnimationClipLoader>("Animation Clip Loader");
    }

    private void OnGUI()
    {
        GUILayout.Label("Load Animation Clip from JSON", EditorStyles.boldLabel);

        filePath = EditorGUILayout.TextField("File Path", filePath);
        fileName = EditorGUILayout.TextField("File Name", fileName);

        if (GUILayout.Button("Load Animation"))
        {
            LoadAnimationClip();
        }
    }

    private void LoadAnimationClip()
    {
        string fullPath = Path.Combine(filePath, fileName);

        if (File.Exists(fullPath))
        {
            AnimationClip clip = AnimationClipSerializer.LoadAnimationClip(fullPath);

            if (clip != null)
            {
                string savePath = Path.Combine(filePath, $"{clip.name}.anim");
                AssetDatabase.CreateAsset(clip, savePath);
                AssetDatabase.SaveAssets();

                EditorUtility.DisplayDialog("Success", $"Animation Clip '{clip.name}' successfully loaded and saved at {savePath}", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Failed to load the animation clip. The JSON file may be corrupted or in an invalid format.", "OK");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "File not found at the specified path.", "OK");
        }
    }
}
