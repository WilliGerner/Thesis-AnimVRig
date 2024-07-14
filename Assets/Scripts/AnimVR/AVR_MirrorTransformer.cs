using System.Collections;
using UnityEngine;

public class AVR_MirrorTransformer : MonoBehaviour
{
    public Transform modelObject; // Das große Objekt, das bewegt wird
    public Vector3 startPosition; // Startposition des Mini-Objekts
    public float returnSpeed = 2.0f; // Geschwindigkeit des Zurücksetzens
    public float fadeDuration = 1.0f; // Dauer des Ausblendens/Einschaltens
    public bool useGlobalDirection = true; // Schalter für globalen oder lokalen Modus

    public Transform rotateScaleObject; // Das zweite kleine Objekt
    private Vector3 secondMiniObjectStartPosition; // Startposition des zweiten Mini-Objekts
    private Quaternion secondMiniObjectStartRotation; // Startrotation des zweiten Mini-Objekts
    private Vector3 secondMiniObjectStartScale; // Startskala des zweiten Mini-Objekts

    private bool isDragging = false;
    private Renderer[] miniObjectRenderers;

    private bool scaleActive = false; // Schalter für aktives Skalieren
    private bool rotateActive = false; // Schalter für aktives Rotieren

    void Start()
    {

    }

    private void OnEnable()
    {
        startPosition = transform.position; // Initiale Startposition speichern
        miniObjectRenderers = GetComponentsInChildren<Renderer>();

        if (rotateScaleObject != null)
        {
            secondMiniObjectStartPosition = rotateScaleObject.position;
            secondMiniObjectStartRotation = rotateScaleObject.rotation;
            secondMiniObjectStartScale = rotateScaleObject.localScale;
        }
    }

    void Update()
    {
        if (isDragging)
        {
            // Offset basierend auf der Bewegung des Mini-Objekts berechnen
            Vector3 offset = transform.position - startPosition;

            if (useGlobalDirection)
            {
                // Globaler Modus: Bewegung des großen Objekts in Weltkoordinaten
                modelObject.position += offset;
            }
            else
            {
                // Lokaler Modus: Bewegung des großen Objekts relativ zu seiner eigenen Rotation
                modelObject.position += modelObject.TransformDirection(offset);
            }

            startPosition = transform.position;
        }

        if (scaleActive)
        {
            CopyScale();
        }

        if (rotateActive)
        {
            CopyRotation();
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
        StartCoroutine(ReturnToStartPosition());
    }

    public void ActivateScaling(bool activate)
    {
        scaleActive = activate;
        if (!scaleActive)
        {
            ResetSecondMiniObject();
        }
    }

    public void ActivateRotating(bool activate)
    {
        rotateActive = activate;
        if (!rotateActive)
        {
            ResetSecondMiniObject();
        }
    }

    private void CopyScale()
    {
        if (rotateScaleObject != null)
        {
            modelObject.localScale = rotateScaleObject.localScale;
        }
    }

    private void CopyRotation()
    {
        if (rotateScaleObject != null)
        {
            modelObject.rotation = rotateScaleObject.rotation;
        }
    }

    public void ResetSecondMiniObject()
    {
        if (rotateScaleObject != null)
        {
            rotateScaleObject.position = secondMiniObjectStartPosition;
            rotateScaleObject.rotation = secondMiniObjectStartRotation;
            rotateScaleObject.localScale = secondMiniObjectStartScale;
        }
    }

    private IEnumerator ReturnToStartPosition()
    {
        // Ausblenden
        yield return StartCoroutine(FadeOut());

        // Mini-Objekt zurück zur Startposition bewegen
        while (Vector3.Distance(transform.position, startPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }

        // Sichere die genaue Position am Ende
        transform.position = startPosition;

        // Einblenden
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            foreach (Renderer renderer in miniObjectRenderers)
            {
                Color color = renderer.material.color;
                color.a = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
                renderer.material.color = color;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (Renderer renderer in miniObjectRenderers)
        {
            Color color = renderer.material.color;
            color.a = 0;
            renderer.material.color = color;
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            foreach (Renderer renderer in miniObjectRenderers)
            {
                Color color = renderer.material.color;
                color.a = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                renderer.material.color = color;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (Renderer renderer in miniObjectRenderers)
        {
            Color color = renderer.material.color;
            color.a = 1;
            renderer.material.color = color;
        }
    }
}