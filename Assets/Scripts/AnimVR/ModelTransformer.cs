using UnityEngine;

public class ModelTransformer : MonoBehaviour
{
    [SerializeField] Camera lookAtCam; 
    private Quaternion initialRotationOffset;
    public Transform transformModel; // Das große Objekt, das bewegt wird
    public Transform spawnPoint;
    public Transform anchor; // Der Anker, an den das große Objekt positioniert werden soll
    public Transform anchorMovemntSphere;
    public Transform anchorRotationSphere;
    public Transform targetCamera; // Die Zielkamera, zu der das große Objekt ausgerichtet werden soll

    public Vector3 positionOffset; // Manuelle Positionsoffset
    //public float positionWeight = 1.0f; // Grundgewicht für Positionsänderungen
    //public float rotationWeight = 1.0f; // Grundgewicht für Rotationsänderungen
    //public float positionSensitivity = 10.0f; // Sensitivität für Positionsänderungen
    //public float rotationSensitivity = 1.0f;  // Sensitivität für Rotationsänderungen
    public float movementMultiplier = 1.0f;


    public bool lookAtPlayer = false; // Boolean, um das Zielobjekt zum Player schauen zu lassen

    [SerializeField]
    GameObject movementSphere; // Das kleine Objekt, das bewegt wird
    private Vector3 movementSphereTransform;
    private Quaternion movementSphereRotation;
    private Quaternion initialRotation;

    [SerializeField]
    GameObject rotationModelSphere;
    private Vector3 rotationSphereTransform;
    private Quaternion rotationSphereRotation;
    public float rotationMultiplier = 1.0f; // Zeit, die Rotation erkannt wird

    private Animator modelAnimator;
    bool grabbedMovement,grabbedRotation;


    private void OnEnable()
    {
        SetPosToSpawn();
        movementSphereTransform = movementSphere.transform.localPosition;
        movementSphereRotation = movementSphere.transform.localRotation;

        rotationSphereTransform = rotationModelSphere.transform.localPosition;
        rotationSphereRotation = rotationModelSphere.transform.localRotation;
        initialRotation = transformModel.localRotation; // Set initial rotation to current local rotation at start
    }

    private void LateUpdate()
    {
       if(!grabbedMovement && movementSphere.gameObject.activeSelf)
        {
            movementSphere.gameObject.transform.position = anchorMovemntSphere.position;
            movementSphere.gameObject.transform.rotation = new Quaternion(gameObject.transform.rotation.x, 0, gameObject.transform.rotation.z, gameObject.transform.rotation.w);
            movementSphereTransform = movementSphere.transform.localPosition;
            movementSphereRotation = movementSphere.transform.localRotation;
        }

        if (!grabbedRotation && rotationModelSphere.gameObject.activeSelf)
        {
            rotationModelSphere.transform.position = anchorRotationSphere.position;
            rotationModelSphere.transform.rotation = new Quaternion(rotationModelSphere.transform.rotation.x, 0, rotationModelSphere.transform.rotation.z, rotationModelSphere.transform.rotation.w);
            rotationSphereTransform = rotationModelSphere.transform.localPosition;
            rotationSphereRotation = rotationModelSphere.transform.localRotation;
        }
    }

    void Update()
    {
        ManageGrabbingMovement();
        ManageGrabbingRotation();
    }

    public void ManageRotationSphere(bool enable)
    {
        rotationModelSphere.SetActive(enable);
    }

    void ManageGrabbingRotation()
    {
        if (grabbedRotation && rotationModelSphere != null && transformModel != null && rotationModelSphere.gameObject.activeSelf)
        {
            // Rotation: Invertiere die Y- und Z-Achse, da sie spiegelverkehrt sind
            Quaternion deltaRotation = rotationModelSphere.transform.localRotation * Quaternion.Inverse(rotationSphereRotation);
            Vector3 deltaEulerAngles = deltaRotation.eulerAngles;
            deltaEulerAngles.y = -deltaEulerAngles.y; // Invertiere Y-Achse (Rotation um die Y-Achse)
            deltaEulerAngles.z = -deltaEulerAngles.z; // Invertiere Z-Achse (Rotation um die Z-Achse)
            deltaRotation = Quaternion.Euler(deltaEulerAngles);
            transformModel.localRotation *= deltaRotation;   // Update the last position and rotation for the next frame
            rotationSphereRotation = rotationModelSphere.transform.localRotation;
        }
    }
    void ManageGrabbingMovement()
    {
        if (grabbedMovement && movementSphere != null && transformModel != null && movementSphere.gameObject.activeSelf)
        {
            // Position: Invertiere die X- und Z-Achse, da sie spiegelverkehrt sind
            Vector3 deltaPosition = movementSphere.transform.localPosition - movementSphereTransform;
            deltaPosition.x = deltaPosition.x; // Invertiere X-Achse
            deltaPosition.z = -deltaPosition.z; // Invertiere Z-Achse
            transformModel.localPosition += deltaPosition * movementMultiplier;
            // Update the last position and rotation for the next frame
            movementSphereTransform = movementSphere.transform.localPosition;
        }
    }

    public void GrabbedSphereMovement()
    {
        grabbedMovement = true;
        //InfoOverlay.Instance.ShowText("Grabbed");
    }

    public void UngrabbedSphereMovement()
    {
        grabbedMovement = false;
       this.gameObject.transform.LookAt(lookAtCam.transform);
    }

    public void GrabbedSphereRotation()
    {
        grabbedRotation = true;
    }

    public void UngrabbedSphereRotation()
    {
        grabbedRotation = false;
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
