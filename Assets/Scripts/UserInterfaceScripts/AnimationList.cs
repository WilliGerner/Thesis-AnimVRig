using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Oculus.Movement.Effects;
using Oculus.Interaction;
using UnityEditor;

public class AnimationList : MonoBehaviour
{
    Animator targetAnimator;
    public Transform interactablesParent; // Eltern-Objekt für die Interactables
    public List<InteractableUnityEventWrapper> interactables = new List<InteractableUnityEventWrapper>(); // Wird dynamisch gefüllt
    public AnimationClip currentClip;
    private Avatar avatar;
    [SerializeField]
    LayerMaskManager layerMaskManager;

    void Start()
    {
        AVRGameObjectRecorder.Instance.OnMotionAdded += SetUpAnimList;
        AVRGameObjectRecorder.Instance.OnChangeModel += SetUpAnimList;
        LoadInteractablesFromParent();
    }

    private void OnDestroy()
    {
        if (AVRGameObjectRecorder.Instance != null)
        {
            AVRGameObjectRecorder.Instance.OnMotionAdded -= SetUpAnimList;
            AVRGameObjectRecorder.Instance.OnChangeModel -= SetUpAnimList;
        }
    }

    void LoadInteractablesFromParent()
    {
        interactables.Clear();
        foreach (Transform child in interactablesParent)
        {
            InteractableUnityEventWrapper interactable = child.GetComponent<InteractableUnityEventWrapper>();
            if (interactable != null)
            {
                interactables.Add(interactable);
            }
        }
        if (interactables.Count > 0)
        {
            SetUpAnimList();
        }
    }

    public void SetUpAnimList()
    {
        targetAnimator = AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<Animator>();
        avatar = targetAnimator.avatar;
        if (targetAnimator != null)
        {
            var clips = targetAnimator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length && i < interactables.Count; i++)
            {
                SetInteractableTextAndListener(interactables[i], clips[i]);
            }
        }
    }

    private void SetInteractableTextAndListener(InteractableUnityEventWrapper interactable, AnimationClip clip)
    {
        TextMeshPro tmpText = interactable.transform.GetChild(1).transform.GetComponentInChildren<TextMeshPro>();
        if (tmpText != null)
            tmpText.text = clip.name;

        interactable.WhenSelect.RemoveAllListeners();
        interactable.WhenSelect.AddListener(() => {
            currentClip = clip;
            PlayAnimation(); // Starte die Animation direkt
        });
    }

    public void SetRootMotion()
    {
        targetAnimator = AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<Animator>();
        targetAnimator.applyRootMotion = !targetAnimator.applyRootMotion;
        if (targetAnimator.applyRootMotion) StudyScript.Instance.SetRootMotion(true);
        else StudyScript.Instance.SetRootMotion(true);
    }

    public void PlayAnimation()
    {
        if (!targetAnimator.enabled) targetAnimator.enabled = true;
        targetAnimator.speed = 1;
        if (RequiresAvatar(currentClip))
        {
            AttachAvatar();
        }
        else
        {
            DetachAvatar();
        }

        if (layerMaskManager._everything)
        {
            StudyScript.Instance.BindEverythingTask();
        }

        if (layerMaskManager._nothing)
        {
            StudyScript.Instance.BindNothingTask();
        }

        if (currentClip.name == "Sitting Clap" && StudyScript.Instance.tutroial_done)
        {
            StudyScript.Instance.PlayClapAnimTask();
        }
        if (currentClip.name.Contains("Jumping") && StudyScript.Instance.tutroial_done && StudyScript.Instance.scene_1_done)
        {
            StudyScript.Instance.PlayJumpAnim();
        }
        if (currentClip.name.Contains("StudyScene_1") && StudyScript.Instance.tutroial_done)
        {
            StudyScript.Instance.PlayNewClapAnim();
        }
        if (currentClip.name.Contains("StudyScene_2") && StudyScript.Instance.tutroial_done && StudyScript.Instance.scene_1_done)
        {
            StudyScript.Instance.PlayYourNewJumpAnim();
        }
        targetAnimator.Play(currentClip.name);
    }

    private bool RequiresAvatar(AnimationClip clip)
    {
        // Prüft, ob die Animation keine Transform-Pfade hat, was darauf hindeuten könnte, dass sie einen Avatar benötigt
        var bindings = AnimationUtility.GetCurveBindings(clip);
        foreach (var binding in bindings)
        {
            if (binding.propertyName.StartsWith("m_LocalPosition") ||
                binding.propertyName.StartsWith("m_LocalRotation") ||
                binding.propertyName.StartsWith("m_LocalScale"))
            {
                return false;
            }
        }
        return true;
    }

    private void AttachAvatar()
    {
        if (avatar != null)
        {
            targetAnimator.avatar = avatar;
        }
    }

    private void DetachAvatar()
    {
        // Entfernt den Avatar, indem der Animator Avatar auf null gesetzt wird
        targetAnimator.avatar = null;
    }

    public void PauseAnimation()
    {
        targetAnimator.speed = 0;
    }

    public void RewindAnimation()
    {
        targetAnimator.Play(currentClip.name, 0, 0);
        targetAnimator.speed = 0;
    }

    public void EndAnimation()
    {
        targetAnimator.Play(currentClip.name, 0, 1);
        targetAnimator.speed = 0;
    }
}
