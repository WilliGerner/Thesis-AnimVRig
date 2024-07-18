using UnityEngine;

public class AVR_MirrorTransformer : MonoBehaviour
{
    public Transform modelObject; // Das große Objekt, das bewegt wird
    public Transform miniModelObject; // Das kleine Objekt, das bewegt wird
    public Transform anchor; // Der Anker, an den das große Objekt positioniert werden soll
    public Transform targetCamera; // Die Zielkamera, zu der das große Objekt ausgerichtet werden soll
    public Vector3 positionOffset; // Manuelle Positionsoffset
    public float movementMultiplier = 1.0f; // Multiplikator für Bewegung
    public float rotationMultiplier = 1.0f; // Multiplikator für Rotation
    public float scalingMultiplier = 1.0f; // Multiplikator für Skalierung
    public bool lookAtPlayer = false; // Boolean, um das Zielobjekt zum Player schauen zu lassen

    private Vector3 previousMiniModelPosition;
    private Quaternion previousMiniModelRotation;
    private Vector3 previousMiniModelScale;
    private Quaternion initialRotation;
    private bool initialRotationSet = false;

    private Animator modelAnimator;

    private void OnEnable()
    {
        if (miniModelObject != null)
        {
            previousMiniModelPosition = miniModelObject.localPosition;
            previousMiniModelRotation = miniModelObject.localRotation;
            previousMiniModelScale = miniModelObject.localScale;
        }

        if (modelObject != null)
        {
            initialRotation = modelObject.localRotation; // Set initial rotation to current local rotation at start
            modelAnimator = modelObject.GetComponent<Animator>(); // Holen Sie sich die Animator-Komponente
        }
    }

    void Update()
    {
        if (miniModelObject != null && modelObject != null)
        {
            // Deaktivieren Sie den Animator, bevor Sie Änderungen vornehmen
            if (modelAnimator != null)
            {
                modelAnimator.enabled = false;
            }

            // Überprüfen der Positionsänderung
            if (miniModelObject.localPosition != previousMiniModelPosition)
            {
                Vector3 deltaPosition = miniModelObject.localPosition - previousMiniModelPosition;
                modelObject.localPosition += modelObject.parent.TransformVector(deltaPosition) * movementMultiplier;
                previousMiniModelPosition = miniModelObject.localPosition;
            }

            // Überprüfen der Rotationsänderung
            if (miniModelObject.localRotation != previousMiniModelRotation)
            {
                Quaternion deltaRotation = Quaternion.Inverse(previousMiniModelRotation) * miniModelObject.localRotation;
                deltaRotation = Quaternion.Inverse(deltaRotation); // Invertiere die Rotationsrichtung
                modelObject.localRotation = modelObject.localRotation * deltaRotation;
                previousMiniModelRotation = miniModelObject.localRotation;
            }

            // Überprüfen der Skalierungsänderung
            if (miniModelObject.localScale != previousMiniModelScale)
            {
                modelObject.localScale = Vector3.Scale(modelObject.localScale, Vector3.one + (miniModelObject.localScale - previousMiniModelScale) * scalingMultiplier);
                previousMiniModelScale = miniModelObject.localScale;
            }

            //// Reaktivieren Sie den Animator nach Änderungen
            //if (modelAnimator != null)
            //{
            //    modelAnimator.enabled = true;
            //}
        }
    }

    public void MoveToAnchor()
    {
        if (anchor != null)
        {
            // Deaktivieren Sie den Animator, bevor Sie Änderungen vornehmen
            if (modelAnimator != null)
            {
                modelAnimator.enabled = false;
            }

            // Setze die Position des großen Objekts auf den Anker plus den manuellen Offset
            modelObject.position = anchor.position + positionOffset;

            // Setze die Rotation des großen Objekts auf die initiale Rotation
            modelObject.localRotation = initialRotation;

            // Wenn `lookAtPlayer` true ist, drehe das Zielobjekt zum Player (nur Y-Achse)
            if (lookAtPlayer)
            {
                // Berechne die Rotation zur Zielkamera für das große Objekt
                Vector3 directionToCamera = targetCamera.position - modelObject.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);

                // Anwenden der neuen Rotation, aber nur die Y-Achse
                Vector3 eulerTargetRotation = targetRotation.eulerAngles;
                eulerTargetRotation.x = 0;
                eulerTargetRotation.z = 0;
                targetRotation = Quaternion.Euler(eulerTargetRotation);

                modelObject.rotation = targetRotation;
            }

            // Reaktivieren Sie den Animator nach Änderungen
            if (modelAnimator != null)
            {
                modelAnimator.enabled = true;
            }
        }
    }
}
