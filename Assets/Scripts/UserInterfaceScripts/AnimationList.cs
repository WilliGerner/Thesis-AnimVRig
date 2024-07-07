using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnimationList : MonoBehaviour
{
    Animator targetAnimator;
    public Transform togglesParent; // Eltern-Objekt für die Toggles
    public GameObject miniUIPrefab;
    private List<Toggle> toggles = new List<Toggle>(); // Wird dynamisch gefüllt
    private GameObject currentMiniUI;
    public AnimationClip currentClip;

    void Start()
    {
        InitializeToggles();
        CreateMiniUI();
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
        targetAnimator = AVRGameObjectRecorder.Instance._animatorAnimationModel; // CHange maybe to AnimatorAnimationModel???
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
                PlayAnimation(); // Starte die Animation direkt
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
            currentMiniUI.transform.position = toggle.transform.position + Vector3.down * 50;
            currentMiniUI.SetActive(true);
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
        buttons[0].onClick.AddListener(() => PlayAnimation());
        buttons[1].onClick.AddListener(() => PauseAnimation());
        buttons[2].onClick.AddListener(() => RewindAnimation());
        buttons[3].onClick.AddListener(() => EndAnimation());
    }

    public void PlayAnimation()
    {
        targetAnimator.Play(currentClip.name);
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
