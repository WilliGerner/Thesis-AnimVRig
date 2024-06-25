using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaderSingelton : MonoBehaviour
{
    public float indicatorBlinkSpeed = 2.0f; // Geschwindigkeit der Blinkanimation
    [SerializeField]
    private Color indicatorColor = Color.red; // Farbe des Indikators

    private static FaderSingelton instance;
    public static FaderSingelton Instance
    {
        get
        {
            if (instance == null)
            {
                // Versuch, eine vorhandene Instanz im Spiel zu finden
                instance = FindObjectOfType<FaderSingelton>();
                if (instance == null)
                {
                    // Erstellen eines neuen GameObjects, wenn keine Instanz gefunden wurde
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<FaderSingelton>();
                    singletonObject.name = typeof(FaderSingelton).ToString() + " (Singleton)";

                    // Sicherstellen, dass das Singleton-GameObject nicht zerstört wird, wenn die Szene wechselt
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    IEnumerator FadeInImgIndicator(Image imgToFade)
    {
        while (AVRGameObjectRecorder.Instance._canRecord && imgToFade.color.a < 1.0f)
        {
            Debug.Log("FadingActivatet");
            Color currentColor = imgToFade.color;
            currentColor.a += 2.0f * Time.deltaTime; // Steuert die Geschwindigkeit des Fade-Ins
            imgToFade.color = currentColor;
            yield return null;
        }
    }

    IEnumerator FadeOutImgIndicator(Image imgToFade)
    {
        while (!AVRGameObjectRecorder.Instance._canRecord && imgToFade.color.a > 0.0f)
        {
            Color currentColor = imgToFade.color;
            currentColor.a -= 2.0f * Time.deltaTime; // Steuert die Geschwindigkeit des Fade-Outs
            imgToFade.color = currentColor;
            yield return null;
        }
    }

    public void BlinkImg(Image imgToFade)
    {
        float alpha = Mathf.PingPong(Time.time * indicatorBlinkSpeed, 1.0f);
        imgToFade.color = new Color(indicatorColor.r, indicatorColor.g, indicatorColor.b, alpha);
    }
}

