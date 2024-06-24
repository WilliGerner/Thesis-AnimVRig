using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using UnityEditor.Animations;
using System.Threading;
using System.Timers;
using System.Collections;
using UnityEngine.UI;

public class AnimVRigRecorder : MonoBehaviour
{
    [Tooltip("The imported Model Rig to be recorded.")]
    public GameObject originalRig;

    private GameObjectRecorder _recorder;
    public AnimationClip _currentClip;
    private Thread _recordingThread;
    private Stopwatch _stopwatch; // deltaTime not in Thread available
    private System.Timers.Timer _recordingTimer;

    [SerializeField]
    private string _saveFolderLocation = "Assets/Animations/";
    [SerializeField]
    private string _clipName;
    [SerializeField]
    private float _frameRate = 30f;
    [SerializeField]
    private float _accumulatedTime = 0f;  // Zeit seit der letzten Aufnahme

    public bool _isRecording = false;

    [SerializeField][Tooltip("The Target Img can be found at the VR Rig -> Palm Menue -> ScollablePanel")]
    private Image saveIndicator; // UI-Image für den Speicherindikator
    public float saveDisplayDuration = 0.5f; // Dauer, wie lange das Image sichtbar bleiben soll

    //private static AnimVRigRecorder instance;

    //public static AnimVRigRecorder Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            // Versuch, eine vorhandene Instanz im Spiel zu finden
    //            instance = FindObjectOfType<AnimVRigRecorder>();
    //            if (instance == null)
    //            {
    //                // Erstellen eines neuen GameObjects, wenn keine Instanz gefunden wurde
    //                GameObject singletonObject = new GameObject();
    //                instance = singletonObject.AddComponent<AnimVRigRecorder>();
    //                singletonObject.name = typeof(AnimVRigRecorder).ToString() + " (Singleton)";

    //                // Sicherstellen, dass das Singleton-GameObject nicht zerstört wird, wenn die Szene wechselt
    //                DontDestroyOnLoad(singletonObject);
    //            }
    //        }
    //        return instance;
    //    }
    //}

    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //        DontDestroyOnLoad(this.gameObject);
    //    }
    //    else if (instance != this)
    //    {
    //        // Zerstören dieses GameObjects, wenn bereits eine Instanz existiert
    //        Destroy(gameObject);
    //    }
    //}

    //private void Start()
    //{
    //    saveIndicator.gameObject.SetActive(false); // Aktiviere das Image

    //    StartRecordingTimerVar();
    //    StartCoroutine(StoPRecordafter5Sec());
    //}

    //IEnumerator StoPRecordafter5Sec()
    //{
    //    yield return new WaitForSeconds(5.0f);

    //    StopRecordingTimerVar();
    //}

    //public void Update()
    //{
    //    if (_isRecording)
    //    {
    //        UnityEngine.Debug.Log($"Recorder is set up: {_recorder != null}, Clip has data: {_currentClip.length > 0}");

    //        // Record a snapshot if the recorder is set up
    //        if (_recorder != null)
    //        {
    //            _recorder.TakeSnapshot(Time.deltaTime);
    //            UnityEngine.Debug.Log($"Recorder is recording: {_recorder.isRecording}, Clip has data: {_currentClip.length > 0}");
    //        }
    //    }
    //}

    //private void LateUpdate()
    //{
    //    //if (_isRecording)
    //    //{
    //    //   if(_recorder ==null) _recorder = new GameObjectRecorder(gameObject);
    //    //    // Bind all Transform components of originalRig and its children
    //    //    _recorder.BindComponentsOfType<Transform>(originalRig, true);
    //    //    UnityEngine.Debug.Log($"Recorder is recording: {_recorder.isRecording}, Clip has data: {_currentClip.length > 0}");
    //    //  //  UnityEngine.Debug.Log("TakeSnapshot at LATEUpdate time: " + e.SignalTime);
    //    //    _recorder.TakeSnapshot(Time.deltaTime);
    //    //}
    //}

    //public void StartRecordingTimerVar()
    //{

    //    if (_recorder == null)
    //    {
    //        _recorder = new GameObjectRecorder(gameObject);
    //        _recorder.BindComponentsOfType<Transform>(originalRig, true);
    //    }

    //    _currentClip = new AnimationClip() { frameRate = _frameRate };
    //    _currentClip.name = string.IsNullOrEmpty(_clipName) ? gameObject.name + "_Animation" : _clipName;

    //    _isRecording = true;
    //    //UnityEngine.Debug.Log("Recording started in Recorder");
    //    //if (_isRecording)
    //    //{
    //    //    UnityEngine.Debug.LogWarning("Recording is already in progress.");
    //    //    return;
    //    //}

    //    //_recorder = new GameObjectRecorder(gameObject);
    //    //_recorder.BindComponentsOfType<Transform>(originalRig, true);
    //    //UnityEngine.Debug.Log("Parts bound of: " + originalRig.name + ", active status: " + originalRig.activeInHierarchy);

    //    //_currentClip = new AnimationClip() { frameRate = _frameRate };
    //    //_currentClip.name = string.IsNullOrEmpty(_clipName) ? gameObject.name + "_Animation" : _clipName;

    //    //_recordingTimer = new System.Timers.Timer(1000 / _frameRate);
    //    //_recordingTimer.Elapsed += OnTimedEvent;
    //    //_recordingTimer.AutoReset = true;
    //    //_recordingTimer.Enabled = true;

    //    //_isRecording = true;
    //}

    //private void OnTimedEvent(object sender, ElapsedEventArgs e)
    //{
    //    UnityEngine.Debug.Log($"Recorder is recording: {_recorder.isRecording}, Clip has data: {_currentClip.length > 0}");
    //    UnityEngine.Debug.Log("TakeSnapshot at time: " + e.SignalTime);
    //    _recorder.TakeSnapshot(Time.deltaTime);  // Verwende einen festen DeltaTime-Wert
    //    _accumulatedTime += (1 / _frameRate);
    //}


    //public void StopRecordingTimerVar()
    //{
    //    if (!_isRecording)
    //    {
    //        UnityEngine.Debug.LogWarning("Recording has not been started.");
    //        return;
    //    }

    //    _isRecording = false;
    //    //_recordingTimer.Stop();
    //    //_recordingTimer.Dispose();

    //    SaveClip();
    //    UnityEngine.Debug.Log("Recording stopped and file saved.");
    //}

  

    //private void SaveClip()
    //{
    //    UnityEngine.Debug.Log("Try to save new clip:" + _currentClip);
    //    StartCoroutine(ShowSaveIndicator());
    //    _recorder.SaveToClip(_currentClip);
    //    string path = _saveFolderLocation + _currentClip.name + ".anim";
    //    if (!System.IO.Directory.Exists(_saveFolderLocation))
    //    {
    //        System.IO.Directory.CreateDirectory(_saveFolderLocation);
    //    }

    //    AssetDatabase.CreateAsset(_currentClip, path);
    //    AssetDatabase.SaveAssets();
    //    UnityEngine.Debug.Log("Animation saved to: " + path);
    //}

    //IEnumerator ShowSaveIndicator()
    //{
    //    saveIndicator.gameObject.SetActive(true); // Aktiviere das Image

    //    // Blinken des Images
    //    int blinkCount = 5; // Anzahl der Blinkvorgänge
    //    float blinkDuration = saveDisplayDuration / blinkCount; // Dauer eines einzelnen Blinkens

    //    for (int i = 0; i < blinkCount; i++)
    //    {
    //        saveIndicator.color = new Color(saveIndicator.color.r, saveIndicator.color.g, saveIndicator.color.b, 1.0f);
    //        yield return new WaitForSeconds(blinkDuration / 2);
    //        saveIndicator.color = new Color(saveIndicator.color.r, saveIndicator.color.g, saveIndicator.color.b, 0.0f);
    //        yield return new WaitForSeconds(blinkDuration / 2);
    //    }

    //    // Langsames Ausblenden
    //    float fadeOutDuration = 1.0f; // Dauer des Ausblendens
    //    float currentTime = 0;
    //    while (currentTime < fadeOutDuration)
    //    {
    //        float alpha = Mathf.Lerp(1.0f, 0.0f, currentTime / fadeOutDuration);
    //        saveIndicator.color = new Color(saveIndicator.color.r, saveIndicator.color.g, saveIndicator.color.b, alpha);
    //        currentTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    saveIndicator.gameObject.SetActive(false); // Deaktiviere das Image nach dem Ausblenden
    //}

    //private void OnDisable()
    //{
    //    if (_isRecording)
    //    {
    //        //StopRecordingThreadVar();
    //    }
    //}

    #region Archiv with THREAD & Stopwatch-Variante
    //public void StartRecordingThreadVar()
    //{
    //    if (_isRecording)
    //    {
    //        UnityEngine.Debug.LogWarning("Recording is already in progress.");
    //        return;
    //    }

    //    if (_recorder == null)
    //    {
    //        _recorder = new GameObjectRecorder(gameObject);
    //        _recorder.BindComponentsOfType<Transform>(originalRig, true);
    //    }

    //    if (_currentClip == null)
    //    {
    //        _currentClip = new AnimationClip() { frameRate = _frameRate };
    //        if (!string.IsNullOrEmpty(_clipName))
    //        {
    //            _currentClip.name = _clipName;
    //        }
    //        else
    //        {
    //            _currentClip.name = gameObject.name + "_Animation";
    //        }
    //    }

    //    _isRecording = true;
    //    _recordingThread = new Thread(RecordAnimation);
    //    _recordingThread.Start();
    //}

    //public void StopRecordingThreadVar()
    //{
    //    if (!_isRecording)
    //    {
    //        UnityEngine.Debug.LogWarning("Recording has not been started.");
    //        return;
    //    }

    //    _isRecording = false;
    //    _recordingThread.Join(); // Wait for the recording thread to finish

    //    SaveClip();
    //    UnityEngine.Debug.Log("Recording stopped and file saved.");
    //}


    //private void RecordAnimation()
    //{
    //    _stopwatch = new Stopwatch(); //Alternative zur deltaTime Berechnung, da diese im Thread nicht verfügbar ist.
    //    _stopwatch.Start();
    //    long lastFrameTime = 0;

    //    while (_isRecording)
    //    {
    //        long currentFrameTime = _stopwatch.ElapsedMilliseconds;
    //        if (currentFrameTime >= lastFrameTime + (1000 / _frameRate))
    //        {
    //            // Nimm die aktuelle Systemzeit als Ersatz für Time.deltaTime
    //            float deltaTime = (currentFrameTime - lastFrameTime) / 1000f;
    //            _recorder.TakeSnapshot(deltaTime);
    //            lastFrameTime = currentFrameTime;
    //        }
    //        Thread.Sleep(1); // Verhindere, dass dieser Thread den Prozessor überlastet
    //    }
    //    _stopwatch.Stop();
    //}

    #endregion

}