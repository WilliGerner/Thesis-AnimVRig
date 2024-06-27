using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelKeypadManager : MonoBehaviour
{
    [Header("Mirror Area")]
    [SerializeField]
    [Tooltip("Gameobject with childs, which should be mirrored, can be null")] // Rechtschreibung xD
    GameObject MirroredObject;  // optional, can be Null if you not call MirroredModelLoigc


    [SerializeField]
    [Tooltip("1 means the first Top Left, 9 mean the last Bottom Right.")]
    public List<GameObject> all9ModelBtns = new List<GameObject>();

    [SerializeField]
    GameObject ModellListUi;
    [SerializeField]
    GameObject CustomizerUi;

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

                    // Sicherstellen, dass das Singleton-GameObject nicht zerstört wird, wenn die Szene wechselt
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


        if (MirroredObject.activeSelf)
        { MirroredObject.SetActive(false); }    // Dekativate sSelbstdarstellung
        else
        { MirroredObject.SetActive(true); }     // Aktiviere Selbstdarstellung
        

    }

    public void SetMirroredObject(GameObject newObject)
    {
        MirroredObject = newObject;
        AVRGameObjectRecorder.Instance._MirroredObjectToRecord = MirroredObject;
        
    }

    public void ActivateAdditionalUI(GameObject ui)
    {
        // Deaktiviere beide UI-Elemente
        ModellListUi.SetActive(false);
        CustomizerUi.SetActive(false);

        // Aktiviere nur das spezifizierte UI
        ui.SetActive(true);
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
