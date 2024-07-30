using UnityEngine;

public class AnimationControllerFiller : MonoBehaviour
{
    Animator animator;
    string animationPath; // Pfad zu den Animationen

    private void Start()
    {
        animator = AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<Animator>();
        animationPath = AVRGameObjectRecorder.Instance.ReturnSafePath();
        //FillAnimationStates();
    }

    public void FillAnimationStates()
    {
        if (animator == null)
        {
            Debug.LogError("Animator ist nicht zugewiesen!");
            return;
        }

        var controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

        if (controller == null)
        {
            Debug.LogError("AnimatorController ist nicht gültig!");
            return;
        }

        foreach (var layer in controller.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.motion == null)
                {
                    string animName = state.state.name + "_Anim";
                    string fullPath = System.IO.Path.Combine(animationPath, animName + ".anim");

                    AnimationClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>(fullPath);

                    if (clip != null)
                    {
                        state.state.motion = clip;
                        Debug.Log($"Animation '{clip.name}' zu Zustand '{state.state.name}' hinzugefügt.");
                    }
                    else
                    {
                        Debug.LogWarning($"Animation '{fullPath}' nicht gefunden.");
                    }
                }
            }
        }

        UnityEditor.EditorUtility.SetDirty(controller); // Markiert den Controller als geändert
        UnityEditor.AssetDatabase.SaveAssets(); // Speichert die Änderungen
    }
}
