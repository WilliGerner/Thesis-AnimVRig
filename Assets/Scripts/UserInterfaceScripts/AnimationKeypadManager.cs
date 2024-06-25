using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationKeypadManager : MonoBehaviour
{


    [Header("Record Area")]
    [SerializeField]
    bool isBtnRecorder;

    [SerializeField]
    [Tooltip("1 means the first Top Left, 9 mean the last Bottom Right.")]
    List<GameObject> all9AnimBtns = new List<GameObject>();

    [SerializeField]
    GameObject _curvedUIAnimList;

    private static AnimationKeypadManager instance;
    public static AnimationKeypadManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Versuch, eine vorhandene Instanz im Spiel zu finden
                instance = FindObjectOfType<AnimationKeypadManager>();
                if (instance == null)
                {
                    // Erstellen eines neuen GameObjects, wenn keine Instanz gefunden wurde
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<AnimationKeypadManager>();
                    singletonObject.name = typeof(AnimationKeypadManager).ToString() + " (Singleton)";

                    // Sicherstellen, dass das Singleton-GameObject nicht zerstört wird, wenn die Szene wechselt
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCurvedUi();
    }

    // Update is called once per frame
    void Update()
    {
        if (AVRGameObjectRecorder.Instance._canRecord) // StartCoroutine Faiding
        {
            StopCoroutine("FadeOutImgIndicator");
            StartCoroutine("FadeInImgIndicator");
        }
        else // Stop Coroutine
        {
            StopCoroutine("FadeInImgIndicator");
            StartCoroutine("FadeOutImgIndicator");
        }
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

    #region  RecorderFunctions

    /// Startet den Recorder und fängt an Aufzunehmen.
    public void RecorderLogic(Image currentActivImg)
    {
        if (currentActivImg.gameObject.activeSelf) // Startet und Stop Logik der Aufnahmen.
        {
            Debug.Log("Started Record");
            AVRGameObjectRecorder.Instance.StartRec(AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<AVRGameObjectRecorder>()); // The current AVRRecorder from the Target Model!
            //AnimVRigRecorder.Instance.StartRecordingTimerVar();
            //
           // _titleText.text = "Stop Record";
            //AnimVRigRecorder.Instance.StartRecordingThreadVar(); // Thread Alternative

        }
        else
        {
            Debug.Log("Stoped Record");
            AVRGameObjectRecorder.Instance.StopRecording(AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<AVRGameObjectRecorder>()); // The current AVRRecorder from the Target Model!
           //_titleText.text = "Start Record";
            //AnimVRigRecorder.Instance.StopRecordingThreadVar();  // Thread Alternative
        }
    }
    #endregion
}
