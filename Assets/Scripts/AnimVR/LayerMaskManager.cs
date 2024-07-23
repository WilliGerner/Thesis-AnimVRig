using Oculus.Interaction;
using Oculus.Movement.AnimationRigging;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class LayerMaskManager : MonoBehaviour
{
    [SerializeField]
    private MirroredTransformManager _mirroredTransformManager;

    #region CustomToggle
    [SerializeField]
    private AnimationClip _animClip;
    [SerializeField]
    private AvatarMask _customMask;
    [SerializeField]
    private AvatarMask _baseLayerMask;
    [SerializeField]
    private RetargetingAnimationConstraint[] _retargetingConstraints;
    [SerializeField]
    private Animator[] _animators;

    #endregion

    [SerializeField]
    private TMPro.TextMeshProUGUI _currentMaskTxt;
    [SerializeField]
    private GameObject layerMenue;

    public bool _leftArm = false;
    public bool _rightArm = false;
    public bool _leftFoot = false;
    public bool _rightFoot = false;
    public bool _everything = false;
    public bool _nothing = true;

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
    public AvatarMask bothArmsBothFeetMask;
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

    private float _savedAnimTime;
    private string _currentAnimStateName;
    private int _currentAnimLayer;
    private Animator _currentAnimator;

    private void Awake()
    {
        Assert.IsNotNull(_animClip);
        Assert.IsNotNull(_customMask);
        Assert.IsNotNull(_baseLayerMask);
        Assert.IsTrue(_retargetingConstraints != null && _retargetingConstraints.Length > 0);
        Assert.IsTrue(_animators != null && _animators.Length > 0);
    }

    private void Start()
    {
        UpdateAllColors();
    }

    private void SaveCurrentAnimationState()
    {
        if (_animators.Length > 0)
        {
            for (int i = 0; i < _animators.Length; i++)
            {
                if (_animators[i].gameObject.activeSelf)
                {
                    _currentAnimator = _animators[i];
                }
            }
         //   _currentAnimator = _animators[0]; // Assuming we're dealing with the first animator for simplicity
            _currentAnimLayer = 0; // Assuming the first layer for simplicity
            var animatorStateInfo = _currentAnimator.GetCurrentAnimatorStateInfo(_currentAnimLayer);

            _currentAnimStateName = animatorStateInfo.shortNameHash.ToString();
            _savedAnimTime = animatorStateInfo.normalizedTime;

            Debug.Log($"Saved Animation State: {_currentAnimStateName} at time {_savedAnimTime}");
        }
    }

    private void RestoreAnimationState()
    {
        if (_currentAnimator != null && !string.IsNullOrEmpty(_currentAnimStateName))
        {
            Debug.Log($"Restoring Animation State: {_currentAnimStateName} at time {_savedAnimTime}");
            _currentAnimator.Play(_currentAnimStateName, _currentAnimLayer, _savedAnimTime);
        }
    }

    private void UpdateAvatarMasksForConstraints()
    {
        //foreach (var retargetConstraint in _retargetingConstraints)
        //{
        //    retargetConstraint.data.AvatarMaskComp = _customMask;
        //}
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
        if (_nothing) return nothingMask;
        if (_everything) return everythingMask;
        if (_leftArm && !_rightArm && !_leftFoot && !_rightFoot) return leftArmMask;
        if (!_leftArm && _rightArm && !_leftFoot && !_rightFoot) return rightArmMask;
        if (!_leftArm && !_rightArm && _leftFoot && !_rightFoot) return leftFootMask;
        if (!_leftArm && !_rightArm && !_leftFoot && _rightFoot) return rightFootMask;
        if (_leftArm && _rightArm && !_leftFoot && !_rightFoot) return bothArmsMask;
        if (!_leftArm && !_rightArm && _leftFoot && _rightFoot) return bothFeetMask;
        if (_leftArm && !_rightArm && _leftFoot && !_rightFoot) return leftArmLeftFootMask;
        if (_leftArm && !_rightArm && !_leftFoot && _rightFoot) return leftArmRightFootMask;
        if (!_leftArm && _rightArm && _leftFoot && !_rightFoot) return rightArmLeftFootMask;
        if (!_leftArm && _rightArm && _leftFoot && _rightFoot) return rightArmRightFootMask;
        if (_leftArm && _rightArm && _leftFoot && !_rightFoot) return bothArmsLeftFootMask;
        if (_leftArm && _rightArm && !_leftFoot && _rightFoot) return bothArmsRightFootMask;
        if (_leftArm && _rightArm && _leftFoot && _rightFoot) return bothArmsBothFeetMask;
        return nothingMask;
    }

    public void ToggleLeftArm()
    {
        SaveCurrentAnimationState();
        _leftArm = !_leftArm;
        ResetEverythingNothing();
        ChangeColor(_leftArm, BtnVisualToggleLeftArm);
        _mirroredTransformManager.ToggleLeftArm(_leftArm);
        UpdateCurrentMask();
        RestoreAnimationState();
    }

    public void ToggleRightArm()
    {
        SaveCurrentAnimationState();
        _rightArm = !_rightArm;
        ResetEverythingNothing();
        ChangeColor(_rightArm, BtnVisualToggleRightArm);
        _mirroredTransformManager.ToggleRightArm(_rightArm);
        UpdateCurrentMask();
        RestoreAnimationState();
    }

    public void ToggleLeftFoot()
    {
        SaveCurrentAnimationState();
        _leftFoot = !_leftFoot;
        ResetEverythingNothing();
        ChangeColor(_leftFoot, BtnVisualToggleLeftLeg);
        _mirroredTransformManager.ToggleLeftLeg(_leftFoot);
        UpdateCurrentMask();
        RestoreAnimationState();
    }

    public void ToggleRightFoot()
    {
        SaveCurrentAnimationState();
        _rightFoot = !_rightFoot;
        ResetEverythingNothing();
        ChangeColor(_rightFoot, BtnVisualToggleRightLeg);
        _mirroredTransformManager.ToggleRightLeg(_rightFoot);
        UpdateCurrentMask();
        RestoreAnimationState();
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
        SaveCurrentAnimationState();
        _everything = true;
        _nothing = false;
        ResetIndividualParts(true);
        UpdateAllColors();

        _customMask = DetermineAvatarMask();
        UpdateAvatarMasksForConstraints();
        ApplyBaseLayerMask();
        _mirroredTransformManager.ToggleEverything(_everything);
        _currentMaskTxt.text = _customMask.name;
        RestoreAnimationState();
    }

    public void ToggleNothing()
    {
        SaveCurrentAnimationState();
        _nothing = true;
        _everything = false;
        ResetIndividualParts(false);
        UpdateAllColors();

        _customMask = DetermineAvatarMask();
        UpdateAvatarMasksForConstraints();
        ApplyBaseLayerMask();
        _mirroredTransformManager.ToggleNothing();
        _currentMaskTxt.text = _customMask.name;
        RestoreAnimationState();
    }

    private void ResetEverythingNothing()
    {
        _everything = false;
        _nothing = false;
        _customMask = DetermineAvatarMask();
        UpdateAvatarMasksForConstraints();
        ApplyBaseLayerMask();
        _currentMaskTxt.text = _customMask.name;
    }

    private void ResetIndividualParts(bool state)
    {
        _leftArm = state;
        _rightArm = state;
        _leftFoot = state;
        _rightFoot = state;
    }

    private void UpdateCurrentMask()
    {
        _customMask = DetermineAvatarMask();
        UpdateAvatarMasksForConstraints();
        ApplyBaseLayerMask();
        _currentMaskTxt.text = _customMask.name;
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
