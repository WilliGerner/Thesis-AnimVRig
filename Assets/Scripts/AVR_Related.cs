using Oculus.Movement.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVR_Related : MonoBehaviour
{
    public GameObject retargetGO;
    public GameObject mirroredObjects;
    public GameObject activeMirrored;
    public List<GameObject> mirroredVaraints;
    public List<LateMirroredObject.MirroredTransformPair> _originalMirroredTransformPairs;

    private void Start()
    {
        activeMirrored = GetActiveVaraint();
    }
    // Methode, um das aktive Element zurückzugeben
    public GameObject GetActiveVaraint()
    {
        foreach (var element in mirroredVaraints)
        {
            if (element.activeSelf)  // Prüfe, ob das GameObject aktiv ist
            {
                return element;  // Gebe das aktive GameObject zurück
            }
        }
        return null;  // Gebe null zurück, wenn kein Element aktiv ist
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
