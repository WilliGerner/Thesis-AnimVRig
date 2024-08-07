using UnityEngine;

public class ModelTransformer : MonoBehaviour
{
    public Transform transformModel; // Das gro�e Objekt, das bewegt wird
    public Transform miniModelObject; // Das kleine Objekt, das bewegt wird

    public Transform spawnPoint;
    public Transform anchor; // Der Anker, an den das gro�e Objekt positioniert werden soll
    public Transform targetCamera; // Die Zielkamera, zu der das gro�e Objekt ausgerichtet werden soll
    public Vector3 positionOffset; // Manuelle Positionsoffset
    public float movementMultiplier = 1.0f; // Multiplikator f�r Bewegung
    public float rotationMultiplier = 1.0f; // Multiplikator f�r Rotation
    public bool lookAtPlayer = false; // Boolean, um das Zielobjekt zum Player schauen zu lassen

    private Vector3 previousMiniModelPosition;
    private Quaternion previousMiniModelRotation;
    private Quaternion initialRotation;

    private Animator modelAnimator;

    private void OnEnable()
    {
        SetPosToSpawn();
        previousMiniModelPosition = miniModelObject.localPosition;
        previousMiniModelRotation = miniModelObject.localRotation;
        initialRotation = transformModel.localRotation; // Set initial rotation to current local rotation at start
    }

    private void Start()
    {
        SetInitialValues();
        if (miniModelObject != null)
        {
            previousMiniModelPosition = miniModelObject.localPosition;
            previousMiniModelRotation = miniModelObject.localRotation;
        }
        //  SetPosToSpawn();
    }

    public void SetInitialValues()
    {
        if (transformModel != null)
        {
           
            modelAnimator = transformModel.GetComponent<Animator>();
        }
    }


    void Update()
    {
        if (miniModelObject != null && transformModel != null && miniModelObject.gameObject.activeSelf)
        {
            if (modelAnimator != null)
            {
                modelAnimator.enabled = false;
            }

            if (miniModelObject.localPosition != previousMiniModelPosition)
            {
                Vector3 deltaPosition = miniModelObject.localPosition - previousMiniModelPosition;
                transformModel.localPosition += transformModel.parent.TransformVector(deltaPosition) * movementMultiplier;
                previousMiniModelPosition = miniModelObject.localPosition;
            }

            if (miniModelObject.localRotation != previousMiniModelRotation)
            {
                Quaternion deltaRotation = Quaternion.Inverse(previousMiniModelRotation) * miniModelObject.localRotation;
                deltaRotation = Quaternion.Inverse(deltaRotation); // Invertierung der Rotationsrichtung
                transformModel.localRotation = transformModel.localRotation * deltaRotation;
                previousMiniModelRotation = miniModelObject.localRotation;
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

            transformModel.position = anchor.position + positionOffset;
            transformModel.localRotation = initialRotation;

            if (lookAtPlayer)
            {
                Vector3 directionToCamera = targetCamera.position - transformModel.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);

                Vector3 eulerTargetRotation = targetRotation.eulerAngles;
                eulerTargetRotation.x = 0;
                eulerTargetRotation.z = 0;
                targetRotation = Quaternion.Euler(eulerTargetRotation);

                transformModel.rotation = targetRotation;
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

            transformModel.position = spawnPoint.position + positionOffset;
            transformModel.localRotation = initialRotation;

            if (lookAtPlayer)
            {
                Vector3 directionToCamera = targetCamera.position - transformModel.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);

                Vector3 eulerTargetRotation = targetRotation.eulerAngles;
                eulerTargetRotation.x = 0;
                eulerTargetRotation.z = 0;
                targetRotation = Quaternion.Euler(eulerTargetRotation);

                transformModel.rotation = targetRotation;
            }

            if (modelAnimator != null)
            {
                modelAnimator.enabled = true;
            }
        }
    }
}
