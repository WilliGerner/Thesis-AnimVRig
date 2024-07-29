using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class AnimationList : MonoBehaviour
{
    Animator targetAnimator;
    public Transform togglesParent; // Eltern-Objekt für die Toggles
    public GameObject miniUIPrefab;
    private List<Toggle> toggles = new List<Toggle>(); // Wird dynamisch gefüllt
    private GameObject currentMiniUI;
    public AnimationClip currentClip;
    private Avatar avatar;
    private Button rootMotionButton;
    private TextMeshProUGUI rootMotionButtonText;

    void Start()
    {
        AVRGameObjectRecorder.Instance.OnMotionAdded += SetUpAnimList;
        AVRGameObjectRecorder.Instance.OnChangeModel += SetUpAnimList;
        InitializeToggles();
        CreateMiniUI();
    }
    private void OnDestroy()
    {
        if (AVRGameObjectRecorder.Instance != null)
        {
            AVRGameObjectRecorder.Instance.OnMotionAdded -= SetUpAnimList;
            AVRGameObjectRecorder.Instance.OnChangeModel -= SetUpAnimList;
        }
    }

    private void InitializeToggles()
    {
        foreach (Transform child in togglesParent)
        {
            Toggle toggle = child.GetComponent<Toggle>();
            if (toggle != null)
                toggles.Add(toggle);
        }

        SetUpAnimList(); // Initialisiert die Liste nach dem Füllen der Toggles
    }

    private void CreateMiniUI()
    {
        if (miniUIPrefab != null && currentMiniUI == null)
        {
            currentMiniUI = Instantiate(miniUIPrefab, Vector3.zero, Quaternion.identity, togglesParent);
            currentMiniUI.SetActive(false); // Starte inaktiv
        }
    }

    public void SetUpAnimList()
    {
        targetAnimator = AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<Animator>(); // CHange maybe to AnimatorAnimationModel???
        avatar = targetAnimator.avatar;
       // targetAnimator = AVRGameObjectRecorder.Instance._animatorAnimationModel; // CHange maybe to AnimatorAnimationModel???
        if (targetAnimator != null)
        {
            var clips = targetAnimator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length && i < toggles.Count; i++)
            {
                SetToggleTextAndListener(toggles[i], clips[i]);
            }
        }
    }

    private void SetToggleTextAndListener(Toggle toggle, AnimationClip clip)
    {
        TextMeshProUGUI tmpText = toggle.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
            tmpText.text = clip.name;

        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(isOn => {
            if (isOn)
            {
                ShowMiniUI(toggle, clip);
              //  PlayAnimation(); // Starte die Animation direkt
            }
            else if (currentMiniUI)
            {
                currentMiniUI.SetActive(false);
            }
        });
    }

    private void ShowMiniUI(Toggle toggle, AnimationClip clip)
    {
        Debug.LogWarning("ShowMini Ui called");
        if (currentMiniUI != null)
        {
            currentClip = clip;
            Transform toggleParent = toggle.transform.parent; // Get the parent of the toggle
            currentMiniUI.transform.SetParent(toggleParent, false); // Set the mini UI's parent to the same parent as the toggle

            int toggleIndex = toggle.transform.GetSiblingIndex(); // Get the index of the toggle
            currentMiniUI.transform.SetSiblingIndex(toggleIndex + 1); // Set the mini UI's sibling index to be right after the toggle

            currentMiniUI.SetActive(true);
            currentMiniUI.transform.localEulerAngles = new Vector3(0, 0, 0);
            currentMiniUI.transform.localPosition = new Vector3(currentMiniUI.transform.localPosition.x, currentMiniUI.transform.localPosition.y, 0);
            AssignMiniUIButtons();
        }
    }

    private void AssignMiniUIButtons()
    {
        Button[] buttons = currentMiniUI.GetComponentsInChildren<Button>();
        buttons[0].onClick.RemoveAllListeners();
        buttons[1].onClick.RemoveAllListeners();
        buttons[2].onClick.RemoveAllListeners();
        buttons[3].onClick.RemoveAllListeners();

        rootMotionButton = buttons[0];
        rootMotionButtonText = rootMotionButton.GetComponentInChildren<TextMeshProUGUI>();

        buttons[0].onClick.AddListener(() => SetRootMotion());
        buttons[1].onClick.AddListener(() => PlayAnimation());
        buttons[1].onClick.AddListener(() => StudyScript.Instance.HitAnimWindowAndPlay());
        buttons[2].onClick.AddListener(() => RewindAnimation());
        buttons[3].onClick.AddListener(() => EndAnimation());

        UpdateRootMotionButton(); // Initialize button state when assigned
    }

    public void SetRootMotion()
    {
        targetAnimator.applyRootMotion = !targetAnimator.applyRootMotion;
        UpdateRootMotionButton();
    }

    private void UpdateRootMotionButton()
    {
        if (!targetAnimator.applyRootMotion)
        {
            rootMotionButton.image.color = Color.green;
            rootMotionButtonText.text = "Root Motion Active";
            StudyScript.Instance.SetRootMotion(true);
        }
        else
        {
            rootMotionButton.image.color = Color.red;
            rootMotionButtonText.text = "Root Motion Inactive";
            StudyScript.Instance.SetRootMotion(false);
        }
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
        if (currentClip.name == "Sitting Clap")
        {
            StudyScript.Instance.PlayClapAnimTask();
        }
        if (currentClip.name == "Jumping")
        {
            StudyScript.Instance.PlayJumpAnim();
        }
        if (currentClip.name == "StudyScene_1")
        {
            StudyScript.Instance.PlayNewClapAnim();
        }
        if (currentClip.name == "StudyScene_2")
        {
            StudyScript.Instance.PlayYourNewJumpAnim();
        }
        targetAnimator.Play(currentClip.name);
        Debug.Log("Anim: " + currentClip.name+"  should play noW");
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

    private IEnumerator HideMiniUIAfterDelay()
    {
        yield return new WaitForSeconds(5);
        if (currentMiniUI != null)
            currentMiniUI.SetActive(false);
    }
}
