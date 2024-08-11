using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AVR_PalmMenueManager : MonoBehaviour
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
    GameObject _UI_3x3_Study; // 4

    [SerializeField]
    TextMeshPro modelNameTxt;
    [SerializeField]
    TextMeshPro clipelNameTxt;

    private static AVR_PalmMenueManager instance;
    public static AVR_PalmMenueManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Versuch, eine vorhandene Instanz im Spiel zu finden
                instance = FindObjectOfType<AVR_PalmMenueManager>();
                if (instance == null)
                {
                    // Erstellen eines neuen GameObjects, wenn keine Instanz gefunden wurde
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<AVR_PalmMenueManager>();
                    singletonObject.name = typeof(AVR_PalmMenueManager).ToString() + " (Singleton)";

                    // Sicherstellen, dass das Singleton-GameObject nicht zerstört wird, wenn die Szene wechselt
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Start()
    {
      //  InitializePalmMenue();
    }

    public GameObject Get_UI_3x3_Model()
    {
        return _UI_3x3_Model;
    }

    public GameObject Get_UI_3x3_Study()
    {
        return _UI_3x3_Study;
    }
    private void Update()
    {
        if (_UI_PalmKnobBtn.activeSelf == false)
        {
            Debug.Log("Deactivate Bigger Menues throuzgh knob");
            DeactivateAlBiglMenues();
        }
    }

    public void ShowAVRMenue()
    {
        /// Normal Case
        //if (_UI_1x3.activeSelf)
        //{
        //    _UI_1x3.SetActive(false);
        //    DeactivateAlBiglMenues();
        //}
        //else
        //{
        //    _UI_1x3.SetActive(true);
        //}


        // StudyCase
        if (_UI_3x3_Study.activeSelf)
        {
            _UI_3x3_Study.SetActive(false);
            DeactivateAlBiglMenues();
        }
        else _UI_3x3_Study.SetActive(true);
    }

    public void InitializePalmMenue() // Logic should work and called after new Model assigned.
    {
        SetModelAndClipNameTxt();
        _animKeypadManager.InitializeAnimKeyPadManager();
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
        switch (menueNr)
        {
            case 0:
                Debug.LogWarning("No valid Menue Selection");
                break;
            case 1:
                _UI_3x3_Anim.SetActive(!_UI_3x3_Anim.activeSelf); // Schaltet das Animation Menü um
                if (_UI_3x3_Anim.activeSelf)
                {
                    AnimationMenueCall();
                }
                break;
            case 2:Debug.Log("Case2 and the target status is: " + _UI_3x3_Model.activeSelf);
                _UI_3x3_Model.SetActive(!_UI_3x3_Model.activeSelf); // Schaltet das Model Menü um
                Debug.Log("Case2 and now: " + _UI_3x3_Model.activeSelf);
                if (_UI_3x3_Model.activeSelf)
                {
                    ModelMenueCall();
                }
                break;
            case 3:
                _UI_3x3_Settings.SetActive(!_UI_3x3_Settings.activeSelf); // Schaltet das Settings Menü um
                if (_UI_3x3_Settings.activeSelf)
                {
                    SettingsMenueCall();
                }
                break;
            case 4:
                _UI_3x3_Study.SetActive(!_UI_3x3_Study.activeSelf); // Schaltet das Settings Menü um
                if (_UI_3x3_Study.activeSelf)
                {
                    StudyMenueCall();
                }
                break;
        }


        // _UI_1x3.SetActive(false);  
    }

    void AnimationMenueCall()
    {
        Debug.Log("Called Anim Menue!");
        if (AVRGameObjectRecorder.Instance.GetCurrentClip() != null)
        {
            string currentClipNameFromRec = AVRGameObjectRecorder.Instance.GetCurrentClip().name; // Set clip Name.
            Debug.Log("Set clip Name.  --> "+  currentClipNameFromRec);
            clipelNameTxt.text = currentClipNameFromRec;
        }
        _animKeypadManager.InitializeAnimKeyPadManager();
        _UI_3x3_Model.SetActive(false);
        _UI_3x3_Settings.SetActive(false);

    }

    void StudyMenueCall()
    {
        _UI_3x3_Anim.SetActive(false);
        _UI_3x3_Settings.SetActive(false);
        _UI_3x3_Model.SetActive(false);
    }
    void ModelMenueCall()
    {
        _UI_3x3_Anim.SetActive(false);
        _UI_3x3_Study.SetActive(false);
        _UI_3x3_Settings.SetActive(false);
    }
    void SettingsMenueCall()
    {
        _UI_3x3_Anim.SetActive(false);
        _UI_3x3_Study.SetActive(false);
        _UI_3x3_Model.SetActive(false);
    }

    public void DeactivateAlBiglMenues()
    {
        _UI_3x3_Study.SetActive(false);
        _UI_3x3_Anim.SetActive(false);
        _UI_3x3_Model.SetActive(false);
        _UI_3x3_Settings.SetActive(false);
        _modelKeypadManager.miniAnimBar.HideMenu();

    }

    public void ReturnToMainMenue()
    {
        DeactivateAlBiglMenues();
        _UI_1x3.SetActive(true);
    }
}
