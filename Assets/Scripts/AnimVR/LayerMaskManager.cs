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
    private RetargetingAnimationConstraint[] _retargetingConstraints;
    [SerializeField]
    private Animator[] _animators;
    [SerializeField]
    private bool _customAnimEnabled = false;
    [SerializeField]
    private TMPro.TextMeshPro _worldText;
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

    }

    private void Update()
    {
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
        else
        {
            roundedBoxProperties.Color = Color.red;
        }

        InvokePrivateMethod(roundedBoxProperties, "UpdateMaterialPropertyBlock");
    }

    public void ToggleEverything()
    {
        _everything = true;
        
        _nothing = false;
        _leftArm = true;
        _rightArm = true;
        _leftFoot = true;
        _rightFoot = true;
        UpdateAllColors();

        _customMask = DetermineAvatarMask();
        _mirroredTransformManager.ToggleEverything(_everything);
        _currentMaskTxt.text = _customMask.name;
    }

    public void ToggleNothing()
    {
        _nothing =true;
        _everything = false;
        ResetIndividualParts();
        UpdateAllColors();
        _mirroredTransformManager.ToggleNothing();
        
        _customMask = DetermineAvatarMask();
        _currentMaskTxt.text = _customMask.name;
    }

    private void ResetEverythingNothing()
    {
        _everything = false;
        _nothing = false;
        _customMask = DetermineAvatarMask();
        _currentMaskTxt.text = _customMask.name;
    }

    private void ResetIndividualParts()
    {
        _leftArm = false;
        _rightArm = false;
        _leftFoot = false;
        _rightFoot = false;
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
            Debug.LogError($"Methode {methodName} nicht gefunden auf {target.GetType().Name}");
        }
    }
}
