using UnityEngine;


public class LayerMaskManager : MonoBehaviour
{
    [SerializeField]
    private GameObject layerMenue;
    public bool _leftArm;
    public bool _rightArm;
    public bool _leftFoot;
    public bool _rightFoot;
    public bool _everything;
    public bool _nothing;

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
        _leftArm = !_leftArm;
        ResetEverythingNothing();
    }

    public void ToggleRightArm()
    {
        _rightArm = !_rightArm;
        ResetEverythingNothing();
    }

    public void ToggleLeftFoot()
    {
        _leftFoot = !_leftFoot;
        ResetEverythingNothing();
    }

    public void ToggleRightFoot()
    {
        _rightFoot = !_rightFoot;
        ResetEverythingNothing();
    }

    public void ToggleEverything()
    {
        _everything = !_everything;
        if (_everything)
        {
            _nothing = false;
            ResetIndividualParts();
        }
    }

    public void ToggleNothing()
    {
        _nothing = !_nothing;
        if (_nothing)
        {
            _everything = false;
            ResetIndividualParts();
        }
    }

    private void ResetEverythingNothing()
    {
        _everything = false;
        _nothing = false;
        AvatarMask currentMask = DetermineAvatarMask();
    }

    private void ResetIndividualParts()
    {
        _leftArm = false;
        _rightArm = false;
        _leftFoot = false;
        _rightFoot = false;
        AvatarMask currentMask = DetermineAvatarMask();
    }
}
