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
    public Transform interactablesParent; // Eltern-Objekt f�r die Interactables
    public List<InteractableUnityEventWrapper> interactables = new List<InteractableUnityEventWrapper>(); // Wird dynamisch gef�llt
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

    public void PlaySpecialAnim(string animName)
    {
        if (!targetAnimator.enabled) targetAnimator.enabled = true;
        targetAnimator.speed = 1;
        if (RequiresAvatar(animName))
        {
            AttachAvatar();
        }
        else
        {
            DetachAvatar();
        }

        targetAnimator.Play(animName);
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

        if (layerMaskManager._leftArm&& layerMaskManager._rightArm && layerMaskManager._leftFoot && layerMaskManager._rightFoot)
        {
            StudyScript.Instance.BindEverythingTask();
        }

        if (!layerMaskManager._leftArm && !layerMaskManager._rightArm && !layerMaskManager._leftFoot && !layerMaskManager._rightFoot)
        {
            StudyScript.Instance.BindNothingTask();
        }

        if (currentClip.name == "Sitzend Klatschen" && StudyScript.Instance.tutroial_done)
        {
            StudyScript.Instance.PlayClapAnimTask();
        }
        if (currentClip.name.Contains("Springen") && StudyScript.Instance.tutroial_done && StudyScript.Instance.scene_1_done)
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

    private bool RequiresAvatar(string clipName)
    {
        // Find the AnimationClip by name
        var clips = Resources.FindObjectsOfTypeAll<AnimationClip>();
        AnimationClip clip = null;

        foreach (var c in clips)
        {
            if (c.name == clipName)
            {
                clip = c;
                break;
            }
        }

        // If the clip is not found, return false
        if (clip == null)
        {
            Debug.LogError($"AnimationClip '{clipName}' not found.");
            return false;
        }

        // Check if the animation has no transform paths, which might indicate it requires an avatar
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

    private bool RequiresAvatar(AnimationClip clip)
    {
        // Pr�ft, ob die Animation keine Transform-Pfade hat, was darauf hindeuten k�nnte, dass sie einen Avatar ben�tigt
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
