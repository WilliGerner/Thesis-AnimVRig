using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaireCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Animator>() != null)
        {
            Debug.Log("Placed at Chaire");
            StudyScript.Instance.PlaceAtChaire();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Animator>() != null)
        {
            Debug.Log("Not Placed at Chaire");
            StudyScript.Instance.NotPlacedAtChaire();
        }
    }
}
