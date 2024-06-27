using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimationControllerFiller))]
public class AnimationControllerFillerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AnimationControllerFiller myScript = (AnimationControllerFiller)target;
        if (GUILayout.Button("Fill Animation States"))
        {
            myScript.FillAnimationStates();
        }
    }
}
