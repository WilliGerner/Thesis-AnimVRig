using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ModelKeypadManager : MonoBehaviour
{
    [Header("Mirror Area")]

    [Tooltip("Gameobject with childs, which should be mirrored, can be null")] // Rechtschreibung xD
    GameObject MirroredObject;  // optional, can be Null if you not call MirroredModelLoigc

    [SerializeField]
    Transform anchorPoint; // Pannel Anchor Point
    public GameObject Btn_1;
    public GameObject Btn_2;
    public GameObject Btn_3;
    public GameObject Btn_4;
    public GameObject Btn_5;
    public GameObject Btn_6;
    public GameObject Btn_7;
    public GameObject Btn_8;
    public GameObject Btn_9;



    [SerializeField]
    GameObject _ModellListUi;
    [SerializeField]
    GameObject _VaraintListUi;
    [SerializeField]
    GameObject _CustomizerUi;
    [SerializeField]
    GameObject _LayerUi;
    [SerializeField]
    GameObject _TransformerPannel;

    #region Study Relevant
    [SerializeField]
    GameObject _ModellListUiStudy;
    [SerializeField]
    GameObject _VaraintListUiStudy;
    [SerializeField]
    GameObject _AnimListUiStudy;
    #endregion


    Avatar lastAvatar;
    bool isAvatarActive = true;

    // Start is called before the first frame update

    private static ModelKeypadManager instance;
    public static ModelKeypadManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Versuch, eine vorhandene Instanz im Spiel zu finden
                instance = FindObjectOfType<ModelKeypadManager>();
                if (instance == null)
                {
                    // Erstellen eines neuen GameObjects, wenn keine Instanz gefunden wurde
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<ModelKeypadManager>();
                    singletonObject.name = typeof(ModelKeypadManager).ToString() + " (Singleton)";

                    // Sicherstellen, dass das Singleton-GameObject nicht zerst�rt wird, wenn die Szene wechselt
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }
    /// <summary>
    /// Eine Methode um das eigene Abild von sich als Spiegel zu aktivieren oder dekativieren
    /// </summary>
    public void MirroredModelLogic()
    {
        // Should Toogle Logic From POalmMenueSIngleBtn should be set in Insepctor!
        MirroredObject = AVRGameObjectRecorder.Instance._MirroredObjectToRecord;

        if (MirroredObject.activeSelf)
        { MirroredObject.SetActive(false); }    // Dekativate sSelbstdarstellung
        else
        { MirroredObject.SetActive(true); }     // Aktiviere Selbstdarstellung
        

    }

    public void SetMirroredObject(GameObject newObject)
    {
        AVRGameObjectRecorder.Instance._objectToRecord = newObject;
        AVRGameObjectRecorder.Instance._MirroredObjectToRecord = newObject.transform.parent.gameObject;
        
    }

    public void CallTransformerModel() // bool is only for Study Case
    {
        GameObject ui_3x3;
        bool study = !this.gameObject.activeSelf;
        if (study)
        {
            ui_3x3 = AVR_PalmMenueManager.Instance.Get_UI_3x3_Study();
        }
        else
        {
            ui_3x3 = AVR_PalmMenueManager.Instance.Get_UI_3x3_Model();
        } 
           
        // Set Panel to anchorPoint;
        _TransformerPannel.transform.position = anchorPoint.position;
        _TransformerPannel.transform.rotation = anchorPoint.rotation;
        _TransformerPannel.transform.localScale = anchorPoint.localScale;

        if (_TransformerPannel.gameObject.activeSelf)
        {
            _TransformerPannel.gameObject.SetActive(false);
            ui_3x3.SetActive(true);
        }
        else
        { 
            _TransformerPannel.gameObject.SetActive(true);
            _ModellListUi.SetActive(false);
            _CustomizerUi.SetActive(false);
            _VaraintListUi.SetActive(false);
            _LayerUi.SetActive(false);
            ui_3x3.SetActive(false);
        }
    }

    public void ActivateAdditionalUI(GameObject ui)
    {
        if (ui.activeSelf)
        {
            ui.SetActive(false);
        }
        else
        {
            // Das UI ist inaktiv, deaktiviere alle anderen UIs und aktiviere das spezifizierte

            // Only for Study
            _ModellListUiStudy.SetActive(false);
            _VaraintListUiStudy.SetActive(false);
            _AnimListUiStudy.SetActive(false);
            _ModellListUi.SetActive(false);
            _CustomizerUi.SetActive(false);
            _VaraintListUi.SetActive(false);
            _TransformerPannel.SetActive(false);  
            _LayerUi.SetActive(false);  

            ui.SetActive(true);
        }
    }
    public void Switch9BtnsActivStatusStudy(bool activ)
    {
        Btn_1.SetActive(activ);
        Btn_2.SetActive(activ);
        if (StudyScript.Instance.scene_1_done) Btn_3.SetActive(activ);
        Btn_4.SetActive(activ);
        if(StudyScript.Instance.tutroial_done) Btn_5.SetActive(activ);
        Btn_6.SetActive(activ);
        Btn_7.SetActive(activ);
        if (StudyScript.Instance.tutroial_done) Btn_8.SetActive(activ);
        Btn_9.SetActive(activ);
    }

    public void ChangeAvatar()
    {
        Animator animator = AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<Animator>();

        if (isAvatarActive)
        {
            // Wenn der Avatar aktuell aktiv ist, speichere ihn und setze ihn dann auf null
            if (animator.avatar != null)
            {
                lastAvatar = animator.avatar;
                animator.avatar = null;
                isAvatarActive = false;
                Debug.Log("Avatar auf null gesetzt");
            }
        }
        else
        {
            // Wenn der Avatar inaktiv ist, stelle den letzten gespeicherten Avatar wieder her
            if (lastAvatar != null)
            {
                animator.avatar = lastAvatar;
                isAvatarActive = true;
                Debug.Log("Avatar auf: " + animator.avatar + " gesetzt.");
            }
            else
            {
                Debug.Log("Kein gespeicherter Avatar verf�gbar");
            }
        }
    }

}
