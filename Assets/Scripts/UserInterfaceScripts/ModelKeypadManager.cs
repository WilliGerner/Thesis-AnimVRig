using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelKeypadManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("1 means the first Top Left, 9 mean the last Bottom Right.")]
    public List<GameObject> all9ModelBtns = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Called()
    {

    }

    public void InitializeModelKeyPadManager()
    {

    }

    void DeactivateBtn(GameObject btnToDeactivateGO)
    {
        btnToDeactivateGO.GetComponentInChildren<Button>().enabled = false;
    }
}
