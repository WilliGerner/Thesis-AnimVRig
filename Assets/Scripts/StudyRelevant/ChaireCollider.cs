using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaireCollider : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Animator>() != null)
        {
            Debug.Log("Placed at Chaire");
            StudyScript.Instance.PlaceAtChaire();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Animator>() != null)
        {
            Debug.Log("Not Placed at Chaire");
            StudyScript.Instance.NotPlacedAtChaire();
        }
    }
}
