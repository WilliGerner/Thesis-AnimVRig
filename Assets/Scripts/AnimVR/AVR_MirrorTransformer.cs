using UnityEngine;

public class AVR_MirrorTransformer : MonoBehaviour
{
    public Transform modelObject; // Das gro�e Objekt, das bewegt wird
    public Transform miniModelObject; // Das kleine Objekt, das bewegt wird
    public Transform anchor; // Der Anker, an den das gro�e Objekt positioniert werden soll
    public Transform targetCamera; // Die Zielkamera, zu der das gro�e Objekt ausgerichtet werden soll
    public float movementMultiplier = 1.0f; // Multiplikator f�r Bewegung
    public float rotationMultiplier = 1.0f; // Multiplikator f�r Rotation
    public float scalingMultiplier = 1.0f; // Multiplikator f�r Skalierung

    private Vector3 previousMiniModelPosition;
    private Quaternion previousMiniModelRotation;
    private Vector3 previousMiniModelScale;
    private Quaternion initialRotation;
    private bool initialRotationSet = false;


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
        }
    }
    void Start()
    {
       
    }

    void Update()
    {
        if (miniModelObject != null && modelObject != null)
        {
            // �berpr�fen der Positions�nderung
            if (miniModelObject.localPosition != previousMiniModelPosition)
            {
                Vector3 deltaPosition = miniModelObject.localPosition - previousMiniModelPosition;
                modelObject.localPosition += modelObject.parent.TransformVector(deltaPosition) * movementMultiplier;
                previousMiniModelPosition = miniModelObject.localPosition;
            }

            // �berpr�fen der Rotations�nderung
            if (miniModelObject.localRotation != previousMiniModelRotation)
            {
                Quaternion deltaRotation = Quaternion.Inverse(previousMiniModelRotation) * miniModelObject.localRotation;
                deltaRotation = Quaternion.Inverse(deltaRotation); // Invertiere die Rotationsrichtung
                modelObject.localRotation = modelObject.localRotation * deltaRotation;
                previousMiniModelRotation = miniModelObject.localRotation;
            }

            // �berpr�fen der Skalierungs�nderung
            if (miniModelObject.localScale != previousMiniModelScale)
            {
                modelObject.localScale = Vector3.Scale(modelObject.localScale, Vector3.one + (miniModelObject.localScale - previousMiniModelScale) * scalingMultiplier);
                previousMiniModelScale = miniModelObject.localScale;
            }
        }
    }

    public void MoveToAnchor()
    {
        if (anchor != null)
        {
            // Setze die Position des gro�en Objekts auf den Anker
            modelObject.position = anchor.position;

            // Setze die Rotation des gro�en Objekts auf die initiale Rotation
            modelObject.localRotation = initialRotation;

            // Berechne die Rotation zur Zielkamera f�r das gro�e Objekt
            Vector3 directionToCamera = targetCamera.position - modelObject.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);

            // Anwenden der neuen Rotation, aber nur die Y-Achse
            Vector3 eulerTargetRotation = targetRotation.eulerAngles;
            eulerTargetRotation.x = 0;
            eulerTargetRotation.z = 0;
            targetRotation = Quaternion.Euler(eulerTargetRotation);

            modelObject.rotation = targetRotation;
        }
    }
}
