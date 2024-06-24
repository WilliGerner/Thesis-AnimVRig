using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationKeypadManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("1 means the first Top Left, 9 mean the last Bottom Right.")]
    List<GameObject> all9AnimBtns = new List<GameObject>();

    [SerializeField]
    GameObject _curvedUIAnimList;


    // Start is called before the first frame update
    void Start()
    {
        SetCurvedUi();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Called()
    {

    }

    public void InitializeAnimKeyPadManager()
    {

    }

    public void SetCurvedUi()
    {
        if (_curvedUIAnimList.activeSelf)
        {
            _curvedUIAnimList.SetActive(false);
        }
        else _curvedUIAnimList.SetActive(true);
    }

    void DeactivateBtn(GameObject btnToDeactivateGO)
    {
        btnToDeactivateGO.GetComponentInChildren<Button>().enabled = false;
    }
}
