using Oculus.Movement.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVR_Related : MonoBehaviour
{
    public Avatar modelAvtar;
    public GameObject retargetGO;
    public GameObject mirroredObjects;
    public GameObject activeMirrored;
    public List<GameObject> mirroredVaraints;
    public List<LateMirroredObject.MirroredTransformPair> _currentMirroredTransformPairs;

    private void Start()
    {
        activeMirrored = GetActiveVaraint();
    }
    // Methode, um das aktive Element zur�ckzugeben
    public GameObject GetActiveVaraint()
    {
        foreach (var element in mirroredVaraints)
        {
            if (element.activeSelf)  // Pr�fe, ob das GameObject aktiv ist
            {
                SetLayerTransformPairsToNewActiv(activeMirrored.GetComponent<ModellVariantInstance>()._originalMirroredTransformPairs);
                return element;  // Gebe das aktive GameObject zur�ck
            }
        }
        return null;  // Gebe null zur�ck, wenn kein Element aktiv ist
    }

    void SetLayerTransformPairsToNewActiv(List<LateMirroredObject.MirroredTransformPair> newPairs)
    {
        _currentMirroredTransformPairs = newPairs;
    }
    
    //public List<LateMirroredObject.MirroredTransformPair> GetOriginalPairs()
    //{
    //    return _originalMirroredTransformPairs;
    //}

    //public void SetOriginalPairs(List<LateMirroredObject.MirroredTransformPair> pairsToSet)
    //{
    //    if (_originalMirroredTransformPairs != null) return;
    //    _originalMirroredTransformPairs = new List<LateMirroredObject.MirroredTransformPair>(pairsToSet);
    //}
}
