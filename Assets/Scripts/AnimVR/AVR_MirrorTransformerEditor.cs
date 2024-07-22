using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AVR_MirrorTransformer))]
public class AVR_MirrorTransformerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AVR_MirrorTransformer script = (AVR_MirrorTransformer)target;
        if (GUILayout.Button("Set Position to Spawn"))
        {
            script.SetPosToSpawn();
        }
    }
}
