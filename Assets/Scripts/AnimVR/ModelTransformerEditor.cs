using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ModelTransformer))]
public class ModelTransformerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ModelTransformer script = (ModelTransformer)target;
        if (GUILayout.Button("Set Position to Spawn"))
        {
            script.SetPosToSpawn();
        }
    }
}
