using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AVR_SingleBtn : MonoBehaviour
{
    [Header("All")]
    [SerializeField]
    public GameObject _EnabledIcon;

    [SerializeField]
    public GameObject _DisabledIcon;

    public bool extraCondition;


    [SerializeField]
    private TMPro.TextMeshProUGUI _titleText; // The Text which will change dependen on the activ Symbol.

    public bool _enabled;

    [SerializeField][Header("Can be an MiniImage like a Save Symbol, can be null.")]
    private Image _ImgIndicator;

    
    private void Start()
    {
       ToggleLogic();

    }

    public void ToggleLogic()// Should be called every time at Start to set State.
    {
        //if (extraCondition)
        //{
        //    if (!StudyManager.Instance.)
        //    {
        //        return;
        //    }
        //}
        _enabled = !_enabled;
        _EnabledIcon.SetActive(_enabled);
        _DisabledIcon.SetActive(!_enabled);
    }
}
