using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PalmMenuSwitchBt : MonoBehaviour
{
    [Header("All")]
    [SerializeField]
    private GameObject _EnabledIcon;

    [SerializeField]
    private GameObject _DisabledIcon;

    [Header("Mirror Area")]
    [SerializeField][Tooltip("Gameobject with childs, which should be mirrored, can be null")] // Rechtschreibung xD
    GameObject MirroredObject;  // optional, can be Null if you not call MirroredModelLoigc
    private bool _enabled;
    //[SerializeField]
    //GameObject Manager; // <-- Should be a Manager Script no GO

    [Header("Record Area")]
    [SerializeField]
    bool isBtnRecorder;
    [SerializeField]
    private Image _ImgIndicator;
    public float indicatorBlinkSpeed = 2.0f; // Geschwindigkeit der Blinkanimation
    [SerializeField]
    private Color indicatorColor = Color.red; // Farbe des Indikators


    [SerializeField]
    private TMPro.TextMeshProUGUI recorderStatusTMPro;

    private void Start()
    {
       ToggleLogic();
      if(isBtnRecorder) StartCoroutine(FadeOutImgIndicator());
      else
        {
            //Bin Mirror Model Case


        }
    }

    public void ToggleLogic()// Should be called every time at Start to set State.
    {
        _enabled = !_enabled;
        _EnabledIcon.SetActive(_enabled);
        _DisabledIcon.SetActive(!_enabled);
    }

    IEnumerator FadeInImgIndicator()
    {
        while (AVRGameObjectRecorder.Instance._canRecord && _ImgIndicator.color.a < 1.0f)
        {
            Debug.Log("FadingActivatet");
            Color currentColor = _ImgIndicator.color;
            currentColor.a += 2.0f * Time.deltaTime; // Steuert die Geschwindigkeit des Fade-Ins
            _ImgIndicator.color = currentColor;
            yield return null;
        }
    }

    IEnumerator FadeOutImgIndicator()
    {
        while (!AVRGameObjectRecorder.Instance._canRecord && _ImgIndicator.color.a > 0.0f)
        {
            Color currentColor = _ImgIndicator.color;
            currentColor.a -= 2.0f * Time.deltaTime; // Steuert die Geschwindigkeit des Fade-Outs
            _ImgIndicator.color = currentColor;
            yield return null;
        }
    }

    // Coroutine zum Anzeigen des Speichervorgangs
   

    private void Update()
    {
        if (isBtnRecorder)
        {
            if (AVRGameObjectRecorder.Instance._canRecord)
            {
                float alpha = Mathf.PingPong(Time.time * indicatorBlinkSpeed, 1.0f);
                _ImgIndicator.color = new Color(indicatorColor.r, indicatorColor.g, indicatorColor.b, alpha);
            }

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
    }

    #region Different Btn FUnctions


    /// <summary>
    /// Eine Methode um das eigene Abild von sich als Spiegel zu aktivieren oder dekativieren
    /// </summary>
    public void MirroredModelLogic()
    {
        ToggleLogic();
        if (_enabled)
        { MirroredObject.SetActive(false); }    // Dekativate sSelbstdarstellung
        else
        { MirroredObject.SetActive(true); }     // Aktiviere Selbstdarstellung
        // Update der UI-Icons
        _EnabledIcon.SetActive(_enabled);
        _DisabledIcon.SetActive(!_enabled);
    }

    #region  RecorderFunctions

    /// Startet den Recorder und fängt an Aufzunehmen.
    public void RecorderLogic()
    {
        ToggleLogic();
        if (_enabled) // Startet und Stop Logik der Aufnahmen.
        {
            Debug.Log("Started Record");
            AVRGameObjectRecorder.Instance.StartRec(AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<AVRGameObjectRecorder>()); // The current AVRRecorder from the Target Model!
            //AnimVRigRecorder.Instance.StartRecordingTimerVar();
            //
            recorderStatusTMPro.text = "Stop Record";
            //AnimVRigRecorder.Instance.StartRecordingThreadVar(); // Thread Alternative

        }
        else
        {
            Debug.Log("Stoped Record");
            AVRGameObjectRecorder.Instance.StopRecording(AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<AVRGameObjectRecorder>()); // The current AVRRecorder from the Target Model!
            recorderStatusTMPro.text = "Start Record";
            //AnimVRigRecorder.Instance.StopRecordingThreadVar();  // Thread Alternative
        }
    }
    #endregion

#endregion

}
