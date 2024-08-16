using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniAnimatorBar : MonoBehaviour
{
    public Animator animator;
    public TMP_Text clipNameText;
    public TMP_Text speedValueText;
    public Slider progressSlider;
    public CanvasGroup[] canvasGroups; // Array für mehrere CanvasGroups
    public List<GameObject> uiElementsToDeactivate; // Liste der UI-Elemente, die deaktiviert werden sollen
    public float fadeDuration = 0.5f;
    public Image playImg, pauseImg;

    private bool isPlaying = true;
    private bool isUserDraggingSlider = false; // Flag to check if the user is dragging the slider
    private const float speedStep = 0.1f; // Konstante für die Schrittweite

    void Start()
    {
        animator = AVRGameObjectRecorder.Instance._animatorMirrored;
        InitializeUI();
        UpdatePlayPauseIcon();
        HideMenu();
    }

    void Update()
    {
        UpdateUI();
    }

    public void InitializeUI()
    {
        if (animator == null)
            animator = AVRGameObjectRecorder.Instance._animatorMirrored;

        if (animator == null || animator.runtimeAnimatorController == null)
            return;

        var clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        // Überprüfen, ob es mindestens einen Clip gibt
        if (clipInfo.Length > 0)
        {
            var currentClip = clipInfo[0].clip;
            clipNameText.text = currentClip.name;
            speedValueText.text = animator.speed.ToString("F1");
            progressSlider.value = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
        }
        else
        {
            // Optional: Setze eine Standardanzeige, wenn kein Clip vorhanden ist
            clipNameText.text = "No Clip";
            speedValueText.text = animator.speed.ToString("F1");
            progressSlider.value = 0f;
        }
    }


    private void UpdateUI()
    {
        if (animator == null) animator = AVRGameObjectRecorder.Instance._animatorMirrored;
        if (animator == null || animator.runtimeAnimatorController == null) return;

        // Update the slider only if the user is not currently dragging it
        if (!isUserDraggingSlider)
        {
            // Get the normalized time of the current animation state
            float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            // If the animation is looping, keep the value between 0 and 1
            progressSlider.value = Mathf.Repeat(normalizedTime, 1f);
        }

        speedValueText.text = animator.speed.ToString("F1");
    }

    [ContextMenu("TogglePlayPause")]
    public void TogglePlayPause()
    {
        isPlaying = !isPlaying;
        if (isPlaying)
        {
            animator.speed = 1f;
        }
        else
        {
            animator.speed = 0f;
        }
        UpdatePlayPauseIcon();
    }

    private void UpdatePlayPauseIcon()
    {
        playImg.enabled = !isPlaying;
        pauseImg.enabled = isPlaying;
    }

    [ContextMenu("ReduceSpeed")]
    public void ReduceSpeed()
    {
        // Geschwindigkeit um 0.1 reduzieren, aber nicht unter 0.1 fallen
        animator.speed = Mathf.Max(0.1f, animator.speed - speedStep);
        speedValueText.text = animator.speed.ToString("F1");
    }

    [ContextMenu("IncreaseSpeed")]
    public void IncreaseSpeed()
    {
        // Geschwindigkeit um 0.1 erhöhen
        animator.speed += speedStep;
        speedValueText.text = animator.speed.ToString("F1");
    }

    public void SetAnimationProgress(float value)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;

        // Setze den Animator auf die entsprechende Zeit basierend auf dem Slider-Wert
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, value);

        // Setze die Geschwindigkeit auf 0, um die Animation an der Stelle zu pausieren
        animator.speed = 0f;
        isPlaying = false;
        UpdatePlayPauseIcon();
    }

    public void InitializeNewAnimation()
    {
        InitializeUI();
    }

    public void ShowMenu()
    {
        //StartCoroutine(FadeInMenu());
    }

    public void HideMenu()
    {
        //StartCoroutine(FadeOutMenu());
    }

    private IEnumerator FadeInMenu()
    {
        foreach (var group in canvasGroups)
        {
            group.gameObject.SetActive(true);
            group.interactable = true;
            group.blocksRaycasts = true;

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                group.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                yield return null;
            }
        }

        // Aktivieren Sie alle spezifizierten UI-Elemente nach dem Einblenden
        foreach (var element in uiElementsToDeactivate)
        {
            element.SetActive(true);
        }
    }

    private IEnumerator FadeOutMenu()
    {
        foreach (var group in canvasGroups)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                group.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
                yield return null;
            }

            group.interactable = false;
            group.blocksRaycasts = false;
        }

        // Deaktiviere alle spezifizierten UI-Elemente nach dem Ausblenden
        foreach (var element in uiElementsToDeactivate)
        {
            element.SetActive(false);
        }
    }

    // Diese Methode sollte mit dem Unity-Event OnPointerDown des Sliders verbunden werden
    public void OnSliderPointerDown()
    {
        isUserDraggingSlider = true;
    }

    // Diese Methode sollte mit dem Unity-Event OnPointerUp des Sliders verbunden werden
    public void OnSliderPointerUp()
    {
        isUserDraggingSlider = false;
        SetAnimationProgress(progressSlider.value);
    }
}
