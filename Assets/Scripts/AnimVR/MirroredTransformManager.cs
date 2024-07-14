using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Oculus.Movement.Effects;

public class MirroredTransformManager : MonoBehaviour
{
    [SerializeField]
    private LateMirroredObject _lateMirroredObject;

    private List<LateMirroredObject.MirroredTransformPair> _originalMirroredTransformPairs;
    private List<LateMirroredObject.MirroredTransformPair> _currentMirroredTransformPairs;
    private FieldInfo _mirroredTransformPairsField;

    private void Awake()
    {
        if (_lateMirroredObject != null)
        {
            Debug.Log("_lateMirroredObject is assigned.");

            _mirroredTransformPairsField = typeof(LateMirroredObject).GetField("_mirroredTransformPairs", BindingFlags.NonPublic | BindingFlags.Instance);

            if (_mirroredTransformPairsField != null)
            {
                Debug.Log("_mirroredTransformPairsField is retrieved.");
                var mirroredTransformPairs = (LateMirroredObject.MirroredTransformPair[])_mirroredTransformPairsField.GetValue(_lateMirroredObject);

                if (mirroredTransformPairs != null)
                {
                    Debug.Log("mirroredTransformPairs is not null and has " + mirroredTransformPairs.Length + " elements.");
                    _originalMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(mirroredTransformPairs);
                    _currentMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(_originalMirroredTransformPairs);
                    Debug.Log("Original Mirrored Transform Pairs initialized with " + _originalMirroredTransformPairs.Count + " elements.");
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
        UpdateMirroredTransformPairs(isEnabled, "clavicle_l");
    }

    public void ToggleRightArm(bool isEnabled)
    {
        UpdateMirroredTransformPairs(isEnabled, "clavicle_r");
    }

    public void ToggleLeftLeg(bool isEnabled)
    {
        UpdateMirroredTransformPairs(isEnabled, "thigh_l");
    }

    public void ToggleRightLeg(bool isEnabled)
    {
        UpdateMirroredTransformPairs(isEnabled, "thigh_r");
    }

    private void UpdateMirroredTransformPairs(bool isEnabled, string bodyPartRoot)
    {
        Debug.Log("isEnabled is: "+ isEnabled);
        if (_originalMirroredTransformPairs == null)
        {
            Debug.LogError("_originalMirroredTransformPairs is null");
            return;
        }

        if (isEnabled)
        {
            // Hinzufügen der Teile, die zu diesem Körperteil gehören
            foreach (var pair in _originalMirroredTransformPairs)
            {
                if (IsBodyPart(pair.OriginalTransform, bodyPartRoot) && !_currentMirroredTransformPairs.Contains(pair))
                {
                    _currentMirroredTransformPairs.Add(pair);
                    Debug.Log($"Added part: {pair.OriginalTransform.name}");
                }
            }
        }
        else
        {
            // Entfernen der Teile, die zu diesem Körperteil gehören
            _currentMirroredTransformPairs.RemoveAll(pair => IsBodyPart(pair.OriginalTransform, bodyPartRoot));
            Debug.Log($"Removed part: {bodyPartRoot}");
        }

        _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
        Debug.Log($"{bodyPartRoot} is now {(isEnabled ? "enabled" : "disabled")}. Current count: {_currentMirroredTransformPairs.Count}");
    }

    private bool IsBodyPart(Transform transform, string bodyPartRoot)
    {
        if (transform.name.ToLower().Contains(bodyPartRoot))
        {
            return true;
        }

        Transform current = transform;
        while (current.parent != null)
        {
            if (current.parent.name.ToLower().Contains(bodyPartRoot))
            {
                return true;
            }
            current = current.parent;
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
        Debug.Log($"Everything is now {(isEnabled ? "enabled" : "disabled")}. Current count: {_currentMirroredTransformPairs.Count}");
    }

    public void ToggleNothing()
    {
        _currentMirroredTransformPairs.Clear();
        _mirroredTransformPairsField.SetValue(_lateMirroredObject, _currentMirroredTransformPairs.ToArray());
        Debug.Log("Nothing is enabled. Current count: 0");
    }
}
