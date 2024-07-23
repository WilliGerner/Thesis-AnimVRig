using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Oculus.Movement.Effects;

public class MirroredTransformManager : MonoBehaviour
{
    public LateMirroredObject _lateMirroredObject;

    private List<LateMirroredObject.MirroredTransformPair> _originalMirroredTransformPairs;
    private List<LateMirroredObject.MirroredTransformPair> _currentMirroredTransformPairs;
    private FieldInfo _mirroredTransformPairsField;

    private void Awake()
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
    }

    public void ToggleLeftArm(bool isEnabled)
    {
        UpdateMirroredTransformPairs(isEnabled, new string[] { "clavicle_l", "Left_UpperArm", "Left_LowerArm", "mixamorig1:LeftArm", "mixamorig1:LeftForeArm" });
    }

    public void ToggleRightArm(bool isEnabled)
    {
        UpdateMirroredTransformPairs(isEnabled, new string[] { "clavicle_r", "Right_UpperArm", "Right_LowerArm", "mixamorig1:RightArm", "mixamorig1:RightForeArm" });
    }

    public void ToggleLeftLeg(bool isEnabled)
    {
        UpdateMirroredTransformPairs(isEnabled, new string[] { "thigh_l", "Left_UpperLeg", "Left_LowerLeg", "mixamorig1:LeftUpLeg", "mixamorig1:LeftLeg" });
    }

    public void ToggleRightLeg(bool isEnabled)
    {
        UpdateMirroredTransformPairs(isEnabled, new string[] { "thigh_r", "Right_UpperLeg", "Right_LowerLeg", "mixamorig1:RightUpLeg", "mixamorig1:RightLeg" });
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
        }
        else
        {
            _currentMirroredTransformPairs.RemoveAll(pair => IsBodyPart(pair.OriginalTransform, bodyPartRoots));
        }

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


    public void ToggleEverything(bool isEnabled)
    {
        if (isEnabled)
        {
            _currentMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(_originalMirroredTransformPairs);
        }
        else
        {
            _currentMirroredTransformPairs.Clear();
        }

        _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
    }

    public void ToggleNothing()
    {
        _currentMirroredTransformPairs.Clear();
        _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
    }
}
