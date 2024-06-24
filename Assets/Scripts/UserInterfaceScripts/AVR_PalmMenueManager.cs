using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AVR_PalmMenueExtreme : MonoBehaviour
{
    [SerializeField]
    AnimationKeypadManager _animKeypadManager;
    [SerializeField]
    ModelKeypadManager _modelKeypadManager;
    [SerializeField]
    SettingsKeypadManager _settingsKeypadManager;


    [SerializeField]
    GameObject _UI_PalmKnobBtn;
    [SerializeField]
    GameObject _UI_1x3;
    [SerializeField]
    GameObject _UI_3x3_Anim; // 1
    [SerializeField]
    GameObject _UI_3x3_Model; // 2
    [SerializeField]
    GameObject _UI_3x3_Settings; // 3

    [SerializeField]
    TextMeshPro modelNameTxt;
    [SerializeField]
    TextMeshPro clipelNameTxt;

    int menueNr = 0; // Default, 1 for Anim, 2 for Model, 3 for Settings


    private void Start()
    {   
      
    }

    void InitializePalmMenue() // Logic should work and called after new Model assigned.
    {
        SetModelAndClipNameTxt();
        _animKeypadManager.InitializeAnimKeyPadManager();
        _modelKeypadManager.InitializeModelKeyPadManager();
        _settingsKeypadManager.InitializeSettingsKeyPadManager();
    }

    void SetModelAndClipNameTxt()
    {
        string currentClipNameFromRec = AVRGameObjectRecorder.Instance.GetCurrentClip().name;
        clipelNameTxt.text = currentClipNameFromRec;

        string currentModelNameFromRec = AVRGameObjectRecorder.Instance._objectToRecord.name;
        modelNameTxt.text = currentModelNameFromRec;

        Debug.Log("Set Model and Clip Name.  --> Model:  " + currentModelNameFromRec + "   Clip:" + currentClipNameFromRec );
    }

    public void MenueSelection(int menueNr)
    {
        DeactivateAlBiglMenues();

        switch (menueNr)
        {
            case 0: Debug.LogWarning("No valid Menue Selection");
                break;
            case 1: _UI_3x3_Anim.SetActive(true); // Animation Menue
                AnimationMenueCall();
                break;
            case 2: _UI_3x3_Model.SetActive(true); // Model Menue
                ModelMenueCall();
                break;
            case 3: _UI_3x3_Settings.SetActive(true); // Settings Menue
                SettingsMenueCall();
                break;
        }

       // _UI_1x3.SetActive(false);  
    }

    void AnimationMenueCall()
    {
        Debug.Log("Called Anim Menue!");

        string currentClipNameFromRec = AVRGameObjectRecorder.Instance.GetCurrentClip().name; // Set clip Name.
        Debug.Log("Set clip Name.  --> "+  currentClipNameFromRec);
        clipelNameTxt.text = currentClipNameFromRec;
        _animKeypadManager.Called();

    }

    void ModelMenueCall()
    {
        Debug.Log("Called Model Menue!");
        _modelKeypadManager.Called();
    }
    void SettingsMenueCall()
    {
        Debug.Log("Called Settings Menue!");
    }

    public void DeactivateAlBiglMenues()
    {
        _UI_3x3_Anim.SetActive(false);
        _UI_3x3_Model.SetActive(false);
        _UI_3x3_Settings.SetActive(false);
    }
}
