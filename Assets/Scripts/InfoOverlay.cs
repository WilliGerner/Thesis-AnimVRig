using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class InfoOverlay : MonoBehaviour
{
    public static InfoOverlay Instance { get; private set; }

    [SerializeField] private GameObject _ActivRecImage;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private float displayDuration = 2.0f;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float blinkInterval = 0.5f; // Zeitintervall für das Blinken

    private Coroutine blinkCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{Destroy(gameObject);}
    }

    public void ShowText(string message)
    {
        StopAllCoroutines();
        textMeshProUGUI.text = message;
        textMeshProUGUI.alpha = 1;
        textMeshProUGUI.gameObject.SetActive(true);
        StartCoroutine(FadeText());
    }

    private IEnumerator FadeText()
    {
        yield return new WaitForSeconds(displayDuration);

        float elapsedTime = 0;
        Color originalColor = textMeshProUGUI.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            textMeshProUGUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        textMeshProUGUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        textMeshProUGUI.gameObject.SetActive(false);
    }

    public void ManageRecImage()
    {
        if (AVRGameObjectRecorder.Instance._canRecord)
        {
            if (!_ActivRecImage.activeSelf)
                _ActivRecImage.SetActive(true);

            if (blinkCoroutine == null)
                blinkCoroutine = StartCoroutine(BlinkRecImage());
        }
        else
        {
            _ActivRecImage.SetActive(false);
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
                _ActivRecImage.GetComponent<Image>().color = Color.black; // Zurücksetzen auf Schwarz
            }
        }
    }

    private IEnumerator BlinkRecImage()
    {
        RawImage image = _ActivRecImage.GetComponent<RawImage>();
        bool isRed = true;

        while (true)
        {
            image.color = isRed ? Color.red : Color.black;
            isRed = !isRed;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
