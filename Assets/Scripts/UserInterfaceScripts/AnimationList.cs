using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Oculus.Movement.Effects;
using Oculus.Interaction;
using UnityEditor;

public class AnimationList : MonoBehaviour
{
    [SerializeField]
    MiniAnimatorBar miniAnimBar;
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
                interactable.gameObject.SetActive(true); // Reactivate interactables when loaded
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
            for (int i = 0; i < interactables.Count; i++)
            {
                if (i < clips.Length)
                {
                    SetInteractableTextAndListener(interactables[i], clips[i]);
                    interactables[i].gameObject.SetActive(true); // Ensure necessary interactables are active
                }
                else
                {
                    interactables[i].gameObject.SetActive(false); // Deactivate surplus interactables
                }
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
        if (targetAnimator == null) targetAnimator = AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<Animator>();
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
        CheckStudyTasks(animName);
        //miniAnimBar.InitializeUI();
        //miniAnimBar.ShowMenu();
    }

    public IEnumerator LateUpdateAnimPlay()
    {
        yield return new WaitForFixedUpdate();

        if (!targetAnimator.enabled) targetAnimator.enabled = true;
        targetAnimator.speed = 1;
        if (RequiresAvatar(currentClip))
        {
            AttachAvatar();
        }
        else DetachAvatar();
        targetAnimator.Play(currentClip.name);
        //miniAnimBar.InitializeUI();
        //miniAnimBar.ShowMenu();
    }

    public void PlayAnimation()
    {
        if (targetAnimator == null) targetAnimator = AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<Animator>();
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
        CheckStudyTasks(currentClip.name);


        targetAnimator.Play(currentClip.name);
        //miniAnimBar.InitializeUI();
        //miniAnimBar.ShowMenu();

    }

    void CheckStudyTasks(string clipName)
    {
        if (layerMaskManager._leftArm && layerMaskManager._rightArm && layerMaskManager._leftFoot && layerMaskManager._rightFoot)
        {
            StudyScript.Instance.BindEverythingTask();
        }

        if (!layerMaskManager._leftArm && !layerMaskManager._rightArm && !layerMaskManager._leftFoot && !layerMaskManager._rightFoot)
        {
            StudyScript.Instance.BindNothingTask();
        }

        if (clipName == "Sitzend Klatschen" && StudyScript.Instance.tutroial_done)
        {
            StudyScript.Instance.PlayClapAnimTask();
        }
        if (clipName.Contains("Springen") && StudyScript.Instance.tutroial_done && StudyScript.Instance.scene_1_done)
        {
            StudyScript.Instance.PlayJumpAnim();
        }
        if (clipName.Contains("StudyScene_1") && StudyScript.Instance.tutroial_done)
        {
            Debug.Log("StudyScene_1 Played");
            StudyScript.Instance.PlayNewClapAnim();
        }
        if (clipName.Contains("StudyScene_2") && StudyScript.Instance.tutroial_done && StudyScript.Instance.scene_1_done)
        {
            StudyScript.Instance.PlayYourNewJumpAnim();
        }
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
