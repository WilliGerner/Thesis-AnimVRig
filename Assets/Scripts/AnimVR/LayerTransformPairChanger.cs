using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Oculus.Movement.Effects;

public class LayerTransformPairChanger : MonoBehaviour
{
    public LateMirroredObject _lateMirroredObject;
    public ModelTransformer _modelTransformer;
    private List<LateMirroredObject.MirroredTransformPair> _originalMirroredTransformPairs;
    private List<LateMirroredObject.MirroredTransformPair> _currentMirroredTransformPairs;
    private FieldInfo _mirroredTransformPairsField;

    private void Awake()
    {
        // Awake method remains empty or can include other initialization that does not depend on the GameObject being active
    }

    public void Initialize()
    {
        if (_lateMirroredObject != null)
        {
            _mirroredTransformPairsField = typeof(LateMirroredObject).GetField("_mirroredTransformPairs", BindingFlags.NonPublic | BindingFlags.Instance);

            if (_mirroredTransformPairsField != null)
            {
                var mirroredTransformPairs = (LateMirroredObject.MirroredTransformPair[])_mirroredTransformPairsField.GetValue(_lateMirroredObject);

                if (mirroredTransformPairs != null)
                {
                    _originalMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(mirroredTransformPairs);
                    _currentMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(_originalMirroredTransformPairs);
                }
                else
                {
                    Debug.LogError("mirroredTransformPairs is null.");
                }
            }
            else
            {
                Debug.LogError("_mirroredTransformPairsField is null.");
            }
        }
        else
        {
            Debug.LogError("_lateMirroredObject is not assigned.");
        }

        // Clear the mirrored transform pairs after initialization
       //  _currentMirroredTransformPairs.Clear();
        _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());

      //  this.enabled = false; // Disable the script after initialization
    }

    public void SetLateMirroredObject()
    {
        _originalMirroredTransformPairs.Clear();
        var mirroredTransformPairs = (LateMirroredObject.MirroredTransformPair[])_mirroredTransformPairsField.GetValue(_lateMirroredObject);
        _originalMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(mirroredTransformPairs);
        Initialize();
    }

    public void ChangeMirrorTransformerModel()
    {
        _modelTransformer.transformModel = _lateMirroredObject.transform.parent;
        _modelTransformer.modelAnimator = _lateMirroredObject.transform.parent.GetComponent<Animator>();
        SetLateMirroredObject();
    }

    public void UpdateLeftArmPairs(bool isEnabled)
    {
        // Füge alle möglichen mixamorig Varianten von 0 bis 9 hinzu
        UpdateMirroredTransformPairs(isEnabled, new string[]
        {
        "clavicle_l",
        "Left_UpperArm",
        "Left_LowerArm",
        "mixamorig:LeftArm",
        "mixamorig1:LeftArm",
        "mixamorig2:LeftArm",
        "mixamorig3:LeftArm",
        "mixamorig4:LeftArm",
        "mixamorig5:LeftArm",
        "mixamorig6:LeftArm",
        "mixamorig7:LeftArm",
        "mixamorig8:LeftArm",
        "mixamorig9:LeftArm",
        "mixamorig:LeftForeArm",
        "mixamorig1:LeftForeArm",
        "mixamorig2:LeftForeArm",
        "mixamorig3:LeftForeArm",
        "mixamorig4:LeftForeArm",
        "mixamorig5:LeftForeArm",
        "mixamorig6:LeftForeArm",
        "mixamorig7:LeftForeArm",
        "mixamorig8:LeftForeArm",
        "mixamorig9:LeftForeArm"
        });
    }


    public void UpdateRightArmPairs(bool isEnabled)
    {
        UpdateMirroredTransformPairs(isEnabled, new string[]
        {
        "clavicle_r",
        "Right_UpperArm",
        "Right_LowerArm",
        "mixamorig:RightArm",
        "mixamorig1:RightArm",
        "mixamorig2:RightArm",
        "mixamorig3:RightArm",
        "mixamorig4:RightArm",
        "mixamorig5:RightArm",
        "mixamorig6:RightArm",
        "mixamorig7:RightArm",
        "mixamorig8:RightArm",
        "mixamorig9:RightArm",
        "mixamorig:RightForeArm",
        "mixamorig1:RightForeArm",
        "mixamorig2:RightForeArm",
        "mixamorig3:RightForeArm",
        "mixamorig4:RightForeArm",
        "mixamorig5:RightForeArm",
        "mixamorig6:RightForeArm",
        "mixamorig7:RightForeArm",
        "mixamorig8:RightForeArm",
        "mixamorig9:RightForeArm"
        });
    }

    public void UpdateLeftLegPairs(bool isEnabled)
    {
        UpdateMirroredTransformPairs(isEnabled, new string[]
        {
        "thigh_l",
        "Left_UpperLeg",
        "Left_LowerLeg",
        "mixamorig:LeftUpLeg",
        "mixamorig1:LeftUpLeg",
        "mixamorig2:LeftUpLeg",
        "mixamorig3:LeftUpLeg",
        "mixamorig4:LeftUpLeg",
        "mixamorig5:LeftUpLeg",
        "mixamorig6:LeftUpLeg",
        "mixamorig7:LeftUpLeg",
        "mixamorig8:LeftUpLeg",
        "mixamorig9:LeftUpLeg",
        "mixamorig:LeftLeg",
        "mixamorig1:LeftLeg",
        "mixamorig2:LeftLeg",
        "mixamorig3:LeftLeg",
        "mixamorig4:LeftLeg",
        "mixamorig5:LeftLeg",
        "mixamorig6:LeftLeg",
        "mixamorig7:LeftLeg",
        "mixamorig8:LeftLeg",
        "mixamorig9:LeftLeg"
        });
    }


    public void UpdateRightLegPairs(bool isEnabled)
    {
        // Füge alle möglichen mixamorig Varianten von 0 bis 9 hinzu
        UpdateMirroredTransformPairs(isEnabled, new string[]
        {
        "thigh_r",
        "Right_UpperLeg",
        "Right_LowerLeg",
        "mixamorig:RightUpLeg",
        "mixamorig1:RightUpLeg",
        "mixamorig2:RightUpLeg",
        "mixamorig3:RightUpLeg",
        "mixamorig4:RightUpLeg",
        "mixamorig5:RightUpLeg",
        "mixamorig6:RightUpLeg",
        "mixamorig7:RightUpLeg",
        "mixamorig8:RightUpLeg",
        "mixamorig9:RightUpLeg",
        "mixamorig:RightLeg",
        "mixamorig1:RightLeg",
        "mixamorig2:RightLeg",
        "mixamorig3:RightLeg",
        "mixamorig4:RightLeg",
        "mixamorig5:RightLeg",
        "mixamorig6:RightLeg",
        "mixamorig7:RightLeg",
        "mixamorig8:RightLeg",
        "mixamorig9:RightLeg"
        });
    }


    private void UpdateMirroredTransformPairs(bool isEnabled, string[] bodyPartRoots)
    {
        if (_originalMirroredTransformPairs == null)
        {
            Debug.LogError("_originalMirroredTransformPairs is null");
            return;
        }

        if (isEnabled)
        {
            foreach (var pair in _originalMirroredTransformPairs)
            {
                if (IsBodyPart(pair.OriginalTransform, bodyPartRoots) && !_currentMirroredTransformPairs.Contains(pair))
                {
                    _currentMirroredTransformPairs.Add(pair);
                }
            }
            _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
        }
        else
        {
            _currentMirroredTransformPairs.RemoveAll(pair => IsBodyPart(pair.OriginalTransform, bodyPartRoots));
            _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
        }
    }

    private bool IsBodyPart(Transform transform, string[] bodyPartRoots)
    {
        foreach (var bodyPartRoot in bodyPartRoots)
        {
            if (transform.name.ToLower().Contains(bodyPartRoot.ToLower()))
            {
                return true;
            }

            Transform current = transform;
            while (current.parent != null)
            {
                if (current.parent.name.ToLower().Contains(bodyPartRoot.ToLower()))
                {
                    return true;
                }
                current = current.parent;
            }
        }
        return false;
    }

    public void SetToAllPairs()
    {
         _currentMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(_originalMirroredTransformPairs);
        _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
        _lateMirroredObject.enabled = true;
    }

    public void SetToZeroPairs()
    {
        _currentMirroredTransformPairs.Clear();
        _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
        _lateMirroredObject.enabled = false;
    }
}
