//using UnityEngine;
//using System.Collections.Generic;
//using System.Reflection;
//using Oculus.Movement.Effects;

//public class LayerTransformPairChanger : MonoBehaviour
//{
//    public LateMirroredObject _lateMirroredObject;
//    public ModelTransformer _modelTransformer;
//    private List<LateMirroredObject.MirroredTransformPair> _originalMirroredTransformPairs;
//    private List<LateMirroredObject.MirroredTransformPair> _currentMirroredTransformPairs;
//    private FieldInfo _mirroredTransformPairsField;

//    private void Awake()
//    {
//        // Awake method remains empty or can include other initialization that does not depend on the GameObject being active
//    }

//    public void Initialize()
//    {
//        if (_lateMirroredObject != null)
//        {
//            _mirroredTransformPairsField = typeof(LateMirroredObject).GetField("_mirroredTransformPairs", BindingFlags.NonPublic | BindingFlags.Instance);

//            if (_mirroredTransformPairsField != null)
//            {
//                var mirroredTransformPairs = (LateMirroredObject.MirroredTransformPair[])_mirroredTransformPairsField.GetValue(_lateMirroredObject);

//                if (mirroredTransformPairs != null)
//                {
//                    _originalMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(mirroredTransformPairs);
//                    _currentMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(_originalMirroredTransformPairs);
//                }
//                else
//                {
//                    Debug.LogError("mirroredTransformPairs is null.");
//                }
//            }
//            else
//            {
//                Debug.LogError("_mirroredTransformPairsField is null.");
//            }
//        }
//        else
//        {
//            Debug.LogError("_lateMirroredObject is not assigned.");
//        }

//        // Clear the mirrored transform pairs after initialization
//       //  _currentMirroredTransformPairs.Clear();
//        _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());

//      //  this.enabled = false; // Disable the script after initialization
//    }

//    public void SetLateMirroredObject()
//    {
//        _originalMirroredTransformPairs.Clear();
//        var mirroredTransformPairs = (LateMirroredObject.MirroredTransformPair[])_mirroredTransformPairsField.GetValue(_lateMirroredObject);
//        _originalMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(mirroredTransformPairs);
//        Initialize();
//    }

//    public void ChangeMirrorTransformerModel()
//    {
//        _modelTransformer.transformModel = _lateMirroredObject.transform.parent;
//        _modelTransformer.modelAnimator = _lateMirroredObject.transform.parent.GetComponent<Animator>();
//        SetLateMirroredObject();
//    }

//    public void UpdateLeftArmPairs(bool isEnabled)
//    {
//        // Füge alle möglichen mixamorig Varianten von 0 bis 9 hinzu
//        UpdateMirroredTransformPairs(isEnabled, new string[]
//        {
//        "clavicle_l",
//        "Left_UpperArm",
//        "Left_LowerArm",
//        "mixamorig:LeftArm",
//        "mixamorig1:LeftArm",
//        "mixamorig2:LeftArm",
//        "mixamorig3:LeftArm",
//        "mixamorig4:LeftArm",
//        "mixamorig5:LeftArm",
//        "mixamorig6:LeftArm",
//        "mixamorig7:LeftArm",
//        "mixamorig8:LeftArm",
//        "mixamorig9:LeftArm",
//        "mixamorig:LeftForeArm",
//        "mixamorig1:LeftForeArm",
//        "mixamorig2:LeftForeArm",
//        "mixamorig3:LeftForeArm",
//        "mixamorig4:LeftForeArm",
//        "mixamorig5:LeftForeArm",
//        "mixamorig6:LeftForeArm",
//        "mixamorig7:LeftForeArm",
//        "mixamorig8:LeftForeArm",
//        "mixamorig9:LeftForeArm"
//        });
//    }


//    public void UpdateRightArmPairs(bool isEnabled)
//    {
//        UpdateMirroredTransformPairs(isEnabled, new string[]
//        {
//        "clavicle_r",
//        "Right_UpperArm",
//        "Right_LowerArm",
//        "mixamorig:RightArm",
//        "mixamorig1:RightArm",
//        "mixamorig2:RightArm",
//        "mixamorig3:RightArm",
//        "mixamorig4:RightArm",
//        "mixamorig5:RightArm",
//        "mixamorig6:RightArm",
//        "mixamorig7:RightArm",
//        "mixamorig8:RightArm",
//        "mixamorig9:RightArm",
//        "mixamorig:RightForeArm",
//        "mixamorig1:RightForeArm",
//        "mixamorig2:RightForeArm",
//        "mixamorig3:RightForeArm",
//        "mixamorig4:RightForeArm",
//        "mixamorig5:RightForeArm",
//        "mixamorig6:RightForeArm",
//        "mixamorig7:RightForeArm",
//        "mixamorig8:RightForeArm",
//        "mixamorig9:RightForeArm"
//        });
//    }

//    public void UpdateLeftLegPairs(bool isEnabled)
//    {
//        UpdateMirroredTransformPairs(isEnabled, new string[]
//        {
//        "thigh_l",
//        "Left_UpperLeg",
//        "Left_LowerLeg",
//        "mixamorig:LeftUpLeg",
//        "mixamorig1:LeftUpLeg",
//        "mixamorig2:LeftUpLeg",
//        "mixamorig3:LeftUpLeg",
//        "mixamorig4:LeftUpLeg",
//        "mixamorig5:LeftUpLeg",
//        "mixamorig6:LeftUpLeg",
//        "mixamorig7:LeftUpLeg",
//        "mixamorig8:LeftUpLeg",
//        "mixamorig9:LeftUpLeg",
//        "mixamorig:LeftLeg",
//        "mixamorig1:LeftLeg",
//        "mixamorig2:LeftLeg",
//        "mixamorig3:LeftLeg",
//        "mixamorig4:LeftLeg",
//        "mixamorig5:LeftLeg",
//        "mixamorig6:LeftLeg",
//        "mixamorig7:LeftLeg",
//        "mixamorig8:LeftLeg",
//        "mixamorig9:LeftLeg"
//        });
//    }


//    public void UpdateRightLegPairs(bool isEnabled)
//    {
//        // Füge alle möglichen mixamorig Varianten von 0 bis 9 hinzu
//        UpdateMirroredTransformPairs(isEnabled, new string[]
//        {
//        "thigh_r",
//        "Right_UpperLeg",
//        "Right_LowerLeg",
//        "mixamorig:RightUpLeg",
//        "mixamorig1:RightUpLeg",
//        "mixamorig2:RightUpLeg",
//        "mixamorig3:RightUpLeg",
//        "mixamorig4:RightUpLeg",
//        "mixamorig5:RightUpLeg",
//        "mixamorig6:RightUpLeg",
//        "mixamorig7:RightUpLeg",
//        "mixamorig8:RightUpLeg",
//        "mixamorig9:RightUpLeg",
//        "mixamorig:RightLeg",
//        "mixamorig1:RightLeg",
//        "mixamorig2:RightLeg",
//        "mixamorig3:RightLeg",
//        "mixamorig4:RightLeg",
//        "mixamorig5:RightLeg",
//        "mixamorig6:RightLeg",
//        "mixamorig7:RightLeg",
//        "mixamorig8:RightLeg",
//        "mixamorig9:RightLeg"
//        });
//    }


//    private void UpdateMirroredTransformPairs(bool isEnabled, string[] bodyPartRoots)
//    {
//        if (_originalMirroredTransformPairs == null)
//        {
//            Debug.LogError("_originalMirroredTransformPairs is null");
//            return;
//        }

//        if (isEnabled)
//        {
//            foreach (var pair in _originalMirroredTransformPairs)
//            {
//                if (IsBodyPart(pair.OriginalTransform, bodyPartRoots) && !_currentMirroredTransformPairs.Contains(pair))
//                {
//                    _currentMirroredTransformPairs.Add(pair);
//                }
//            }
//            _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
//        }
//        else
//        {
//            _currentMirroredTransformPairs.RemoveAll(pair => IsBodyPart(pair.OriginalTransform, bodyPartRoots));
//            _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
//        }
//    }

//    private bool IsBodyPart(Transform transform, string[] bodyPartRoots)
//    {
//        foreach (var bodyPartRoot in bodyPartRoots)
//        {
//            if (transform.name.ToLower().Contains(bodyPartRoot.ToLower()))
//            {
//                return true;
//            }

//            Transform current = transform;
//            while (current.parent != null)
//            {
//                if (current.parent.name.ToLower().Contains(bodyPartRoot.ToLower()))
//                {
//                    return true;
//                }
//                current = current.parent;
//            }
//        }
//        return false;
//    }

//    public void SetToAllPairs()
//    {
//         _currentMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(_originalMirroredTransformPairs);
//        _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
//        _lateMirroredObject.enabled = true;
//    }

//    public void SetToZeroPairs()
//    {
//        _currentMirroredTransformPairs.Clear();
//        _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
//        _lateMirroredObject.enabled = false;
//    }
//}

using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Oculus.Movement.Effects;

public class LayerTransformPairChanger : MonoBehaviour
{
    public LateMirroredObject _lateMirroredObject;
    public ModelTransformer _modelTransformer;
    private List<LateMirroredObject.MirroredTransformPair> _currentMirroredTransformPairs;
    private FieldInfo _mirroredTransformPairsField;
    public AVR_Related _avrRelated;
    public LayerMaskManager _layerMaskManager;

    private void Awake()
    {
        // Find the AVR_Related component
        FindAVRRelatedComponent();
        // Initialize the mirrored transform pairs
        Initialize();
    }

    public void Initialize()
    {
        if (_lateMirroredObject != null)
        {
            _mirroredTransformPairsField = typeof(LateMirroredObject).GetField("_mirroredTransformPairs", BindingFlags.NonPublic | BindingFlags.Instance);

            if (_mirroredTransformPairsField != null)
            {
                // Use the original pairs from AVR_Related
                var originalMirroredTransformPairs = _avrRelated._currentMirroredTransformPairs;

                if (originalMirroredTransformPairs != null && originalMirroredTransformPairs.Count > 0)
                {
                    _currentMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(originalMirroredTransformPairs);
                }
                else
                {
                    Debug.LogError("Original mirrored transform pairs are null or empty in AVR_Related.");
                }

                // Set the mirrored transform pairs in the LateMirroredObject
                _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
            }
            else
            {
                Debug.LogError("Failed to initialize mirrored transform pairs.");
            }
        }
        else
        {
            Debug.LogError("_lateMirroredObject is not assigned.");
        }
    }

    public void ChangeMirrorTransformerModel()
    {
        if (_lateMirroredObject == null)
        {
            Debug.LogError("_lateMirroredObject is not assigned in ChangeMirrorTransformerModel.");
            return;
        }

        // Update the model transformer with the new model
        _modelTransformer.transformModel = _lateMirroredObject.transform.parent;
        _modelTransformer.modelAnimator = _lateMirroredObject.transform.parent.GetComponent<Animator>();

        // After changing the model, find the new AVR_Related component
        FindAVRRelatedComponent();

        // Reinitialize to set up the mirrored transform pairs for the new model
        Initialize();
    }

    private void FindAVRRelatedComponent()
    {
        // Start searching from the current _lateMirroredObject and go up the hierarchy
        Transform currentTransform = _lateMirroredObject.transform;

        // Traverse up to 3 levels in the hierarchy to find the AVR_Related component
        for (int i = 0; i < 4; i++)
        {
            if (currentTransform == null)
            {
                break;
            }

            _avrRelated = currentTransform.GetComponent<AVR_Related>();

            if (_avrRelated != null)
            {
                return; // If found, stop searching
            }

            currentTransform = currentTransform.parent;
        }

        if (_avrRelated == null)
        {
            Debug.LogError("AVR_Related component not found in the parent hierarchy.");
        }
    }

    public void UpdateLeftArmPairs(bool isEnabled)
    {
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
        if (_avrRelated == null || _currentMirroredTransformPairs == null)
        {
            Debug.LogError("_avrRelated or _currentMirroredTransformPairs is null");
            return;
        }

        // Wir beginnen mit einer frischen Kopie der ursprünglichen Paare
        _currentMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(_avrRelated._currentMirroredTransformPairs);

        // Entferne alle Paare, die zu inaktiven Körperteilen gehören
        if (!_layerMaskManager._leftArm)
        {
            _currentMirroredTransformPairs.RemoveAll(pair => IsBodyPart(pair.OriginalTransform, new[] {
            "clavicle_l", "Left_UpperArm", "Left_LowerArm",
            "mixamorig:LeftArm", "mixamorig1:LeftArm", "mixamorig2:LeftArm",
            "mixamorig3:LeftArm", "mixamorig4:LeftArm", "mixamorig5:LeftArm",
            "mixamorig6:LeftArm", "mixamorig7:LeftArm", "mixamorig8:LeftArm",
            "mixamorig9:LeftArm", "mixamorig:LeftForeArm", "mixamorig1:LeftForeArm",
            "mixamorig2:LeftForeArm", "mixamorig3:LeftForeArm", "mixamorig4:LeftForeArm",
            "mixamorig5:LeftForeArm", "mixamorig6:LeftForeArm", "mixamorig7:LeftForeArm",
            "mixamorig8:LeftForeArm", "mixamorig9:LeftForeArm"
        }));
        }
        if (!_layerMaskManager._rightArm)
        {
            _currentMirroredTransformPairs.RemoveAll(pair => IsBodyPart(pair.OriginalTransform, new[] {
            "clavicle_r", "Right_UpperArm", "Right_LowerArm",
            "mixamorig:RightArm", "mixamorig1:RightArm", "mixamorig2:RightArm",
            "mixamorig3:RightArm", "mixamorig4:RightArm", "mixamorig5:RightArm",
            "mixamorig6:RightArm", "mixamorig7:RightArm", "mixamorig8:RightArm",
            "mixamorig9:RightArm", "mixamorig:RightForeArm", "mixamorig1:RightForeArm",
            "mixamorig2:RightForeArm", "mixamorig3:RightForeArm", "mixamorig4:RightForeArm",
            "mixamorig5:RightForeArm", "mixamorig6:RightForeArm", "mixamorig7:RightForeArm",
            "mixamorig8:RightForeArm", "mixamorig9:RightForeArm"
        }));
        }
        if (!_layerMaskManager._leftFoot)
        {
            _currentMirroredTransformPairs.RemoveAll(pair => IsBodyPart(pair.OriginalTransform, new[] {
            "thigh_l", "Left_UpperLeg", "Left_LowerLeg",
            "mixamorig:LeftUpLeg", "mixamorig1:LeftUpLeg", "mixamorig2:LeftUpLeg",
            "mixamorig3:LeftUpLeg", "mixamorig4:LeftUpLeg", "mixamorig5:LeftUpLeg",
            "mixamorig6:LeftUpLeg", "mixamorig7:LeftUpLeg", "mixamorig8:LeftUpLeg",
            "mixamorig9:LeftUpLeg", "mixamorig:LeftLeg", "mixamorig1:LeftLeg",
            "mixamorig2:LeftLeg", "mixamorig3:LeftLeg", "mixamorig4:LeftLeg",
            "mixamorig5:LeftLeg", "mixamorig6:LeftLeg", "mixamorig7:LeftLeg",
            "mixamorig8:LeftLeg", "mixamorig9:LeftLeg"
        }));
        }
        if (!_layerMaskManager._rightFoot)
        {
            _currentMirroredTransformPairs.RemoveAll(pair => IsBodyPart(pair.OriginalTransform, new[] {
            "thigh_r", "Right_UpperLeg", "Right_LowerLeg",
            "mixamorig:RightUpLeg", "mixamorig1:RightUpLeg", "mixamorig2:RightUpLeg",
            "mixamorig3:RightUpLeg", "mixamorig4:RightUpLeg", "mixamorig5:RightUpLeg",
            "mixamorig6:RightUpLeg", "mixamorig7:RightUpLeg", "mixamorig8:RightUpLeg",
            "mixamorig9:RightUpLeg", "mixamorig:RightLeg", "mixamorig1:RightLeg",
            "mixamorig2:RightLeg", "mixamorig3:RightLeg", "mixamorig4:RightLeg",
            "mixamorig5:RightLeg", "mixamorig6:RightLeg", "mixamorig7:RightLeg",
            "mixamorig8:RightLeg", "mixamorig9:RightLeg"
        }));
        }

        // Jetzt fügen wir die aktiven Körperteile wieder hinzu
        if (isEnabled)
        {
            foreach (var pair in _avrRelated._currentMirroredTransformPairs)
            {
                if (IsBodyPart(pair.OriginalTransform, bodyPartRoots) && !_currentMirroredTransformPairs.Contains(pair))
                {
                    _currentMirroredTransformPairs.Add(pair);
                }
            }
        }
        else
        {
            _currentMirroredTransformPairs.RemoveAll(pair => IsBodyPart(pair.OriginalTransform, bodyPartRoots));
        }

        // Anwenden der aktualisierten Paare auf das LateMirroredObject
        _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
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

    private void DebugPrintMirroredTransformPairs(List<LateMirroredObject.MirroredTransformPair> pairs, string context)
    {
        Debug.Log($"[{context}] MirroredTransformPairs count: {pairs.Count}");
        foreach (var pair in pairs)
        {
            Debug.Log($"[{context}] Original: {pair.OriginalTransform.name}, Mirrored: {pair.MirroredTransform.name}");
        }
    }

    public void SetToAllPairs()
    {
        // Set _currentMirroredTransformPairs to a new list based on the original pairs from AVR_Related
        _currentMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(_avrRelated._currentMirroredTransformPairs);
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

