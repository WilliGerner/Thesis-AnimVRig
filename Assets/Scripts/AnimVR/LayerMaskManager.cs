using Oculus.Interaction;
using Oculus.Movement.AnimationRigging;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class LayerMaskManager : MonoBehaviour
{
    [SerializeField]
    private LayerTransformPairChanger _mirroredTransformManager;
    [SerializeField]
    StudyManager studyManager;
    [SerializeField]
    private AvatarMask _customMask;
    [SerializeField]
    private AvatarMask _baseLayerMask;
    //[SerializeField]
    //private RetargetingAnimationConstraint[] _retargetingConstraints;

    [SerializeField]
    private TMPro.TextMeshProUGUI _currentMaskTxt;
    [SerializeField]
    private GameObject layerMenue;

    public bool _leftArm = false;
    public bool _rightArm = false;
    public bool _leftFoot = false;
    public bool _rightFoot = false;
    //public bool _everything = false;
    //public bool _nothing = true;

    public AvatarMask leftArmMask;
    public AvatarMask rightArmMask;
    public AvatarMask leftFootMask;
    public AvatarMask rightFootMask;
    public AvatarMask leftArmLeftFootMask;
    public AvatarMask leftArmRightFootMask;
    public AvatarMask rightArmLeftFootMask;
    public AvatarMask rightArmRightFootMask;
    public AvatarMask bothArmsMask;
    public AvatarMask bothFeetMask;
    public AvatarMask bothArmsLeftFootMask;
    public AvatarMask bothArmsRightFootMask;
    public AvatarMask leftArmBothFeetMask;
    public AvatarMask rightArmBothFeetMask;
    public AvatarMask everythingMask;
    public AvatarMask nothingMask;

    private RoundedBoxProperties roundedBoxProperties;
    [SerializeField]
    private GameObject BtnVisualToggleNothing;
    [SerializeField]
    private GameObject BtnVisualToggleLeftArm;
    [SerializeField]
    private GameObject BtnVisualToggleRightArm;
    [SerializeField]
    private GameObject BtnVisualToggleLeftLeg;
    [SerializeField]
    private GameObject BtnVisualToggleRightLeg;
    [SerializeField]
    private GameObject BtnVisualToggleHead;

    [SerializeField]
    AnimationList mainAnimList;

    private void Awake()
    {
        Assert.IsNotNull(_customMask);
        Assert.IsNotNull(_baseLayerMask);
    }

    private void OnEnable()
    {
        UpdateAllColors();
    }

    private void ApplyBaseLayerMask()
    {
        for (int i = 0; i < (int)AvatarMaskBodyPart.LastBodyPart; i++)
        {
            AvatarMaskBodyPart bodyPart = (AvatarMaskBodyPart)i;
            _baseLayerMask.SetHumanoidBodyPartActive(bodyPart, _customMask.GetHumanoidBodyPartActive(bodyPart));
        }
    }

    AvatarMask DetermineAvatarMask()
    {
        int mask = 0;
        mask |= _leftArm ? 1 : 0;    // Setzt Bit 0, wenn _leftArm true ist
        mask |= _rightArm ? 2 : 0;   // Setzt Bit 1, wenn _rightArm true ist
        mask |= _leftFoot ? 4 : 0;   // Setzt Bit 2, wenn _leftFoot true ist
        mask |= _rightFoot ? 8 : 0;  // Setzt Bit 3, wenn _rightFoot true ist

        switch (mask)
        {
            case 1:  // 0001 in Binär (nur _leftArm ist true)
                return leftArmMask;
            case 2:  // 0010 in Binär (nur _rightArm ist true)
                return rightArmMask;
            case 4:  // 0100 in Binär (nur _leftFoot ist true)
                return leftFootMask;
            case 8:  // 1000 in Binär (nur _rightFoot ist true)
                return rightFootMask;
            case 3:  // 0011 in Binär (beide Arme sind true)
                return bothArmsMask;
            case 12: // 1100 in Binär (beide Füße sind true)
                return bothFeetMask;
            case 5:  // 0101 in Binär (_leftArm und _leftFoot sind true)
                return leftArmLeftFootMask;
            case 9:  // 1001 in Binär (_leftArm und _rightFoot sind true)
                return leftArmRightFootMask;
            case 6:  // 0110 in Binär (_rightArm und _leftFoot sind true)
                return rightArmLeftFootMask;
            case 10: // 1010 in Binär (_rightArm und _rightFoot sind true)
                return rightArmRightFootMask;
            case 7:  // 0111 in Binär (beide Arme und _leftFoot sind true)
                return bothArmsLeftFootMask;
            case 11: // 1011 in Binär (beide Arme und _rightFoot sind true)
                return bothArmsRightFootMask;
            case 13: // 1101 in Binär (beide Füße und _leftArm sind true)
                return leftArmBothFeetMask;
            case 14: // 1110 in Binär (beide Füße und _rightArm sind true)
                return rightArmBothFeetMask;
            case 15: // 1111 in Binär (alle vier Gliedmaßen sind true)
                return everythingMask;
            case 0:  // 0000 in Binär (keine Gliedmaßen sind true)
                return nothingMask;
            default:
                return nothingMask;
        }
    }

    public void ToggleLeftArm()
    {
        Transform currentTransform = AVRGameObjectRecorder.Instance._animatorMirrored.gameObject.transform;
        //Debug.LogWarning("See Trans: " + currentTransform.position );
        _leftArm = !_leftArm;
        DeterMineAndApplyBaseLayer();
        ChangeColor(_leftArm, BtnVisualToggleLeftArm);
        _mirroredTransformManager.UpdateLeftArmPairs(_leftArm);


        if (mainAnimList.currentClip != null) StartCoroutine(mainAnimList.LateUpdateAnimPlay());
        //AVRGameObjectRecorder.Instance._animatorMirrored.gameObject.transform.position = currentTransform.position;
        //AVRGameObjectRecorder.Instance._animatorMirrored.gameObject.transform.rotation = currentTransform.rotation;
        //Debug.LogWarning("See Trans now: " + AVRGameObjectRecorder.Instance._animatorMirrored.gameObject.transform.position);
    }

    public void ToggleRightArm()
    {
        _rightArm = !_rightArm;
        DeterMineAndApplyBaseLayer();
        ChangeColor(_rightArm, BtnVisualToggleRightArm);
        _mirroredTransformManager.UpdateRightArmPairs(_rightArm); // Update TransformPaires
        if (mainAnimList.currentClip != null) StartCoroutine(mainAnimList.LateUpdateAnimPlay());
    }

    public void ToggleLeftFoot()
    {
        _leftFoot = !_leftFoot;
        DeterMineAndApplyBaseLayer();
        ChangeColor(_leftFoot, BtnVisualToggleLeftLeg);
        _mirroredTransformManager.UpdateLeftLegPairs(_leftFoot);
        if (mainAnimList.currentClip != null) StartCoroutine(mainAnimList.LateUpdateAnimPlay());
    }

    public void ToggleRightFoot()
    {
        _rightFoot = !_rightFoot;
        DeterMineAndApplyBaseLayer();
        ChangeColor(_rightFoot, BtnVisualToggleRightLeg);
        _mirroredTransformManager.UpdateRightLegPairs(_rightFoot);
        if (mainAnimList.currentClip != null) StartCoroutine(mainAnimList.LateUpdateAnimPlay());
    }

    public void ChangeColor(bool active, GameObject buttonVisualGO)
    {
        roundedBoxProperties = buttonVisualGO.GetComponent<RoundedBoxProperties>();

        if (roundedBoxProperties == null)
        {
            Debug.LogError("RoundedBoxProperties component not found on target GameObject.");
            return;
        }

        roundedBoxProperties.Color = active ? Color.green : Color.red;
        InvokePrivateMethod(roundedBoxProperties, "UpdateMaterialPropertyBlock");
    }

    public void ToggleEverything()
    {
        ResetIndividualParts(true);
        UpdateAllColors();
        DeterMineAndApplyBaseLayer();
    }

    public void ToggleNothing()
    {
        ResetIndividualParts(false);
        UpdateAllColors();
        DeterMineAndApplyBaseLayer();
        if (mainAnimList.currentClip != null) StartCoroutine(mainAnimList.LateUpdateAnimPlay());
    }

    private void DeterMineAndApplyBaseLayer()
    {
        _customMask = DetermineAvatarMask();
        _mirroredTransformManager._lateMirroredObject.enabled = true;
        ApplyBaseLayerMask();
        _currentMaskTxt.text = _customMask.name;
        if (_customMask == nothingMask)
        { _mirroredTransformManager.SetToZeroPairs(); }
        else if (_customMask == everythingMask)
        { _mirroredTransformManager.SetToAllPairs(); }


        if (studyManager != null)
        {
            if (_customMask == bothFeetMask) // Only for Study
            { StudyManager.Instance.SetFeetsTask(); }
            if (_customMask == bothArmsMask) // Only for Study
            { StudyManager.Instance.SetArmsTask(); }
        }
    }

    private void ResetIndividualParts(bool state)
    {
        _leftArm = state;
        _rightArm = state;
        _leftFoot = state;
        _rightFoot = state;
    }

    private void UpdateAllColors()
    {
        ChangeColor(_leftArm, BtnVisualToggleLeftArm);
        ChangeColor(_rightArm, BtnVisualToggleRightArm);
        ChangeColor(_leftFoot, BtnVisualToggleLeftLeg);
        ChangeColor(_rightFoot, BtnVisualToggleRightLeg);
    }

    public void DeactivateLayerUI()
    {
        this.gameObject.SetActive(false);
    }

    private void InvokePrivateMethod(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(target, null);
        }
        else
        {
            Debug.LogError($"Method {methodName} not found on {target.GetType().Name}");
        }
    }
}
