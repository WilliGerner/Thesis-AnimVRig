using UnityEngine;

public class AVR_MirrorTransformer : MonoBehaviour
{
    public Transform modelObject; // Das große Objekt, das bewegt wird
    public Transform miniModelObject; // Das kleine Objekt, das bewegt wird

    public Transform spawnPoint;
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

    private Animator modelAnimator;

    private void OnEnable()
    {
        SetInitialValues();
    }

    private void Start()
    {
        if (miniModelObject != null)
        {
            previousMiniModelPosition = miniModelObject.localPosition;
            previousMiniModelRotation = miniModelObject.localRotation;
            previousMiniModelScale = miniModelObject.localScale;
        }
        //  SetPosToSpawn();
    }

    void SetInitialValues()
    {
     

        if (modelObject != null)
        {
            initialRotation = modelObject.localRotation; // Set initial rotation to current local rotation at start
            modelAnimator = modelObject.GetComponent<Animator>();
        }
    }


    void Update()
    {
        if (miniModelObject != null && modelObject != null)
        {
            if (modelAnimator != null)
            {
                modelAnimator.enabled = false;
            }

            if (miniModelObject.localPosition != previousMiniModelPosition)
            {
                Vector3 deltaPosition = miniModelObject.localPosition - previousMiniModelPosition;
                modelObject.localPosition += modelObject.parent.TransformVector(deltaPosition) * movementMultiplier;
                previousMiniModelPosition = miniModelObject.localPosition;
            }

            if (miniModelObject.localRotation != previousMiniModelRotation)
            {
                Quaternion deltaRotation = Quaternion.Inverse(previousMiniModelRotation) * miniModelObject.localRotation;
                deltaRotation = Quaternion.Inverse(deltaRotation); // Invertierung der Rotationsrichtung
                modelObject.localRotation = modelObject.localRotation * deltaRotation;
                previousMiniModelRotation = miniModelObject.localRotation;
            }

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
            if (modelAnimator != null)
            {
                modelAnimator.enabled = false;
            }

            modelObject.position = anchor.position + positionOffset;
            modelObject.localRotation = initialRotation;

            if (lookAtPlayer)
            {
                Vector3 directionToCamera = targetCamera.position - modelObject.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);

                Vector3 eulerTargetRotation = targetRotation.eulerAngles;
                eulerTargetRotation.x = 0;
                eulerTargetRotation.z = 0;
                targetRotation = Quaternion.Euler(eulerTargetRotation);

                modelObject.rotation = targetRotation;
            }

            if (modelAnimator != null)
            {
                modelAnimator.enabled = true;
            }
        }
    }

    public void SetPosToSpawn()
    {
        if (spawnPoint != null)
        {
            if (modelAnimator != null)
            {
                modelAnimator.enabled = false;
            }

            modelObject.position = spawnPoint.position + positionOffset;
            modelObject.localRotation = initialRotation;

            if (lookAtPlayer)
            {
                Vector3 directionToCamera = targetCamera.position - modelObject.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);

                Vector3 eulerTargetRotation = targetRotation.eulerAngles;
                eulerTargetRotation.x = 0;
                eulerTargetRotation.z = 0;
                targetRotation = Quaternion.Euler(eulerTargetRotation);

                modelObject.rotation = targetRotation;
            }

            if (modelAnimator != null)
            {
                modelAnimator.enabled = true;
            }
        }
    }
}
