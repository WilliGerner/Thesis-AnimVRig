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
    /// <summary>
    /// Animation clip to play.
    /// </summary>
    [SerializeField]
    private AnimationClip _animClip;
    /// <summary>
    /// Mask to apply.
    /// </summary>
    [SerializeField]
    private AvatarMask _customMask;
    /// <summary>
    /// Retargeting constraints to fix based on animation state.
    /// </summary>
    [SerializeField]
    private RetargetingAnimationConstraint[] _retargetingConstraints;
    /// <summary>
    /// Animators to control.
    /// </summary>
    [SerializeField]
    private Animator[] _animators;
    /// <summary>
    /// True if animation is enabled, false is not.
    /// </summary>
    [SerializeField]
    private bool _customAnimEnabled = false;
    /// <summary>
    /// Text to update to based on animation state.
    /// </summary>
    [SerializeField]
    private TMPro.TextMeshPro _worldText;
    /// <summary>
    /// Animator parameter name.
    /// </summary>
    [SerializeField]
    private string _animParamName = "Wave";

    private const string _ANIM_OFF_TEXT = "Anim off";
    private const string _ANIM_ON_TEXT = "Anim on";
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
    // Define the Avatar Masks for each combination
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
    GameObject BtnVisualToggleNothing;
    [SerializeField]
    GameObject BtnVisualToggleLeftArm;
    [SerializeField]
    GameObject BtnVisualToggleRightArm;
    [SerializeField]
    GameObject BtnVisualToggleLeftLeg;
    [SerializeField]
    GameObject BtnVisualToggleRightLeg;
    [SerializeField]
    GameObject BtnVisualToggleHead;

    private void Awake()
    {
        Assert.IsNotNull(_animClip);
        Assert.IsNotNull(_customMask);
        Assert.IsTrue(_retargetingConstraints != null && _retargetingConstraints.Length > 0);
        Assert.IsTrue(_animators != null && _animators.Length > 0);
        Assert.IsNotNull(_worldText);
       
    }

    private void Start()
    {
        ToggleLeftFoot();
    }


    private void Update()
    {
        // since the animation rig set up might reboot due to calibration
        // keep setting parameter to the proper value.
        foreach (var animator in _animators)
        {
            animator.SetBool(_animParamName, _customAnimEnabled);
        }
    }
    public void SwapAnimState()
    {
        _customAnimEnabled = !_customAnimEnabled;
        EnforceAnimState();
    }

    private void EnforceAnimState()
    {
        foreach (var retargetConstraint in _retargetingConstraints)
        {
            retargetConstraint.data.AvatarMaskComp =
                _customAnimEnabled ? _customMask : null;
        }
        foreach (var animator in _animators)
        {
            animator.SetBool(_animParamName, _customAnimEnabled);
        }
        _worldText.text = _customAnimEnabled ?
            _ANIM_ON_TEXT : _ANIM_OFF_TEXT;
    }

    AvatarMask DetermineAvatarMask()
    {
        if (_nothing)
        {
            return nothingMask;
        }
        if (_everything)
        {
            return everythingMask;
        }

        if (_leftArm && !_rightArm && !_leftFoot && !_rightFoot)
        {
            return leftArmMask;
        }
        if (!_leftArm && _rightArm && !_leftFoot && !_rightFoot)
        {
            return rightArmMask;
        }
        if (!_leftArm && !_rightArm && _leftFoot && !_rightFoot)
        {
            return leftFootMask;
        }
        if (!_leftArm && !_rightArm && !_leftFoot && _rightFoot)
        {
            return rightFootMask;
        }
        if (_leftArm && _rightArm && !_leftFoot && !_rightFoot)
        {
            return bothArmsMask;
        }
        if (!_leftArm && !_rightArm && _leftFoot && _rightFoot)
        {
            return bothFeetMask;
        }
        if (_leftArm && !_rightArm && _leftFoot && !_rightFoot)
        {
            return leftArmLeftFootMask;
        }
        if (_leftArm && !_rightArm && !_leftFoot && _rightFoot)
        {
            return leftArmRightFootMask;
        }
        if (!_leftArm && _rightArm && _leftFoot && !_rightFoot)
        {
            return rightArmLeftFootMask;
        }
        if (!_leftArm && _rightArm && !_leftFoot && _rightFoot)
        {
            return rightArmRightFootMask;
        }
        if (_leftArm && _rightArm && _leftFoot && !_rightFoot)
        {
            return bothArmsLeftFootMask;
        }
        if (_leftArm && _rightArm && !_leftFoot && _rightFoot)
        {
            return bothArmsRightFootMask;
        }
        if (_leftArm && !_rightArm && _leftFoot && _rightFoot)
        {
            return leftArmBothFeetMask;
        }
        if (!_leftArm && _rightArm && _leftFoot && _rightFoot)
        {
            return rightArmBothFeetMask;
        }
        if (_leftArm && _rightArm && _leftFoot && _rightFoot)
        {
            return bothArmsBothFeetMask;
        }

        // Default case
        return nothingMask;
    }

    public void ToggleLeftArm()
    {
        if (_customAnimEnabled) SwapAnimState();
        _leftArm = !_leftArm;
        ResetEverythingNothing();
        ChangeColor(_leftArm, BtnVisualToggleLeftArm);
        _mirroredTransformManager.ToggleLeftArm(_leftArm);
    }

    public void ToggleRightArm()
    {
        if (_customAnimEnabled) SwapAnimState();
        _rightArm = !_rightArm;
        ResetEverythingNothing();
        ChangeColor(_rightArm, BtnVisualToggleRightArm);
        _mirroredTransformManager.ToggleRightArm(_rightArm);
    }

    public void ToggleLeftFoot()
    {
        if (_customAnimEnabled) SwapAnimState();
        _leftFoot = !_leftFoot;
        ResetEverythingNothing();
        ChangeColor(_leftFoot, BtnVisualToggleLeftLeg);
        _mirroredTransformManager.ToggleLeftLeg(_leftFoot);
    }

    public void ToggleRightFoot()
    {
        if (_customAnimEnabled) SwapAnimState();
        _rightFoot = !_rightFoot;
        ResetEverythingNothing();
        ChangeColor(_rightFoot, BtnVisualToggleRightLeg);
        _mirroredTransformManager.ToggleRightLeg(_rightFoot);
    }


    public void ChangeColor(bool active, GameObject buttonVisualGO)
    {
        roundedBoxProperties = buttonVisualGO.GetComponent<RoundedBoxProperties>();

        if (roundedBoxProperties == null)
        {
            Debug.LogError("RoundedBoxProperties component not found on target GameObject.");
            return;
        }

        if (active)
        {
            roundedBoxProperties.Color = Color.green;
        }
        else roundedBoxProperties.Color = Color.red;

        InvokePrivateMethod(roundedBoxProperties, "UpdateMaterialPropertyBlock");
    }
    public void ToggleEverything()
    {
        _everything = !_everything;
        if (_everything)
        {
            _nothing = false;
            _leftArm = true;
            ChangeColor(_leftArm, BtnVisualToggleLeftArm);
            _rightArm = true;
            ChangeColor(_rightArm, BtnVisualToggleRightArm);
            _leftFoot = true;
            ChangeColor(_rightFoot, BtnVisualToggleLeftLeg);
            _rightFoot = true;
            ChangeColor(_rightFoot, BtnVisualToggleRightLeg);
        }
        _mirroredTransformManager.ToggleEverything(_everything);
        Debug.Log("_customMask is now: " + _customMask.name);
        //ChangeColor(_everything, BtnVisualToggleHead);
    }

    public void ToggleNothing()
    {
        _nothing = !_nothing;
        if (_nothing)
        {
            _everything = false;
            ResetIndividualParts();
            _mirroredTransformManager.ToggleNothing();
        }
    }

    private void ResetEverythingNothing()
    {
        _everything = false;
        _nothing = false;
        _customMask = DetermineAvatarMask();
        Debug.Log("_customMask is now: " + _customMask.name);
        _currentMaskTxt.text = _customMask.name;
    }

    private void ResetIndividualParts()
    {
        _leftArm = false;
        ChangeColor(_leftArm, BtnVisualToggleLeftArm);
        _rightArm = false;
        ChangeColor(_rightArm, BtnVisualToggleRightArm);
        _leftFoot = false;
        ChangeColor(_rightFoot, BtnVisualToggleLeftLeg);
        _rightFoot = false;
        ChangeColor(_rightFoot, BtnVisualToggleRightLeg);
        _customMask = DetermineAvatarMask();
        Debug.Log("_customMask is now: " + _customMask.name);
        _currentMaskTxt.text = _customMask.name;
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
            Debug.LogError($"Methode {methodName} nicht gefunden auf {target.GetType().Name}");
        }
    }
}
