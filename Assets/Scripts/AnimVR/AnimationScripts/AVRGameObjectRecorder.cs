using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using Oculus.Movement.Effects;
using UnityEngine.UI;
using System.IO;

public class AVRGameObjectRecorder : MonoBehaviour
{
    [Tooltip("Your own current Rig")]
    public GameObject _ownRigModel;
    [Tooltip("The Object you want to Record")]
    public GameObject _objectToRecord;
    public GameObject _MirroredObjectToRecord;
#if UNITY_EDITOR
    AnimatorController _animController;
    public GameObjectRecorder _recorder;
#endif
    AnimationClip _currentClip;
    public List<AnimationClip> allClips;
    public Animator _animatorMirrored;
    public LayerTransformPairChanger mirroredTransformManager;

    public List<GameObject> additionalRecordObjects;
    public List<GameObject> allMainObjectsToRecord;
    private List<GameObject> currentVariantsToRecord;

    [SerializeField]
    LayerMaskManager _layerMaskManager;
    [SerializeField]
    private GameObject _blackScreenCanvas;
    private RawImage blackScreenImage_1;
    private RawImage blackScreenImage_2;
    private RectTransform rectTransform1;
    private RectTransform rectTransform2;
    // Dauer f�r das Schlie�en und �ffnen der Augenlider
    public float blinkDuration = 0f;
    // Dauer, wie lange die Augen geschlossen bleiben
    public float closedDuration = 0.2f;

    public bool _canRecord = false;
    public bool recordInit = false;
    public bool recordMirroredObject = false;
    bool debugActiv = false;

    //[SerializeField]
    //private AnimationList _animListLayer;
    [SerializeField]
    private AnimationList _animList;
    private Coroutine recordingCoroutine;
    public TextMeshProUGUI countdownText; // Add this for the countdown timer

    [SerializeField]
    private StudyManager _studyManager;

    public event Action OnMotionAdded;
    public event Action OnChangeModel;

    private static readonly Queue<System.Action> _executionQueue = new Queue<System.Action>();

    #region Inspector Variables
    public bool isActiv = false;

    [Tooltip("Must start with Assets/ and will be normaly Set at The Start to RecordObkjectNameFolder.")]
    [SerializeField] string _saveFolderLocation = "Assets/AnimationContent/";
    private string _savePath;

    public string _clipName;
    [SerializeField] float _frameRate = 60f;

    [SerializeField]
    LayerTransformPairChanger mirroredTransfromManager; // In Study Case something else than normal ( Layer Menue Study)
    [SerializeField]
    ModelTransformer avr_mirrorTransformer;

    //[SerializeField]
    //AnimationClipSerializer animClipSerializer;

    #endregion

    private static AVRGameObjectRecorder instance;
    public static AVRGameObjectRecorder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AVRGameObjectRecorder>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<AVRGameObjectRecorder>();
                    singletonObject.name = typeof(AVRGameObjectRecorder).ToString() + " (Singleton)";          
                }
            }
            return instance;
        }
    }

    private void Start()
    {
#if UNITY_EDITOR
        // Determine the save path depending on the platform
        if (Application.isEditor)
        {
            _savePath = _saveFolderLocation;
        }
        else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // Assuming the application is running on a Quest 3 or other Android/iOS device
            _savePath = Path.Combine(Application.persistentDataPath, "AnimationContent");

            // Ensure the directory exists
            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }
        }
        else
        {
            // Fallback to persistent data path for other platforms
            _savePath = Path.Combine(Application.persistentDataPath, "AnimationContent");
        }

        // Example log for debugging
        Debug.Log("Animation save path: " + _savePath);
        RuntimeAnimatorController animationController = _objectToRecord.GetComponent<Animator>().runtimeAnimatorController;
        _animController = animationController as AnimatorController;
#endif
        _canRecord = false;
        CreateNewClip();
        ActivateOtherModel(GetActiveElement().name);
        AVR_PalmMenueManager.Instance.InitializePalmMenue();
        countdownText.gameObject.SetActive(false);
        mirroredTransformManager.Initialize(); // Clear the Pairs after saving
        blackScreenImage_1 = _blackScreenCanvas.transform.GetChild(0).gameObject.GetComponent<RawImage>();
        blackScreenImage_2 = _blackScreenCanvas.transform.GetChild(1).gameObject.GetComponent<RawImage>();
        rectTransform1 = blackScreenImage_1.GetComponent<RectTransform>();
        rectTransform2 = blackScreenImage_2.GetComponent<RectTransform>();

        // Initiale Skalierung und Positionierung
        rectTransform1.anchorMin = new Vector2(0, 0.5f);
        rectTransform1.anchorMax = new Vector2(1, 1);
        rectTransform1.offsetMin = Vector2.zero;
        rectTransform1.offsetMax = Vector2.zero;
        rectTransform1.anchoredPosition = new Vector2(0, rectTransform1.rect.height);

        rectTransform2.anchorMin = new Vector2(0, 0);
        rectTransform2.anchorMax = new Vector2(1, 0.5f);
        rectTransform2.offsetMin = Vector2.zero;
        rectTransform2.offsetMax = Vector2.zero;
        rectTransform2.anchoredPosition = new Vector2(0, -rectTransform2.rect.height);

        //StartCoroutine(CloseEyesTest());
    }



    private void Update()
    {
#if UNITY_EDITOR
        if (_canRecord)
        {
            _recorder.TakeSnapshot(Time.deltaTime);
           Debug.Log("Recording: " + _recorder.isRecording);
        }
#endif
    }

    public GameObject GetActiveElement()
    {
        foreach (var element in allMainObjectsToRecord)
        {
            if (element.activeSelf)  // Pr�fe, ob das GameObject aktiv ist
            {
                return element;  // Gebe das aktive GameObject zur�ck
            }
        }
        return null;  // Gebe null zur�ck, wenn kein Element aktiv ist
    }

    public string ReturnSafePath()
    {
        return _saveFolderLocation;
    }

    public AnimationClip GetCurrentClip()
    {
        return _currentClip;
    }

    public List<AnimationClip> GetAllClips(string rootObjectName)
    {
#if UNITY_EDITOR
        allClips = new List<AnimationClip>();
        string path = _saveFolderLocation;
        string[] searchedAsset = AssetDatabase.FindAssets("t:AnimationClip", new[] { path });
        if (searchedAsset.Length >= 1)
        {
            for (int i = 0; i < searchedAsset.Length; i++)
            {
                path = AssetDatabase.GUIDToAssetPath(searchedAsset[i]);
                if (path.Contains(".anim"))
                {
                    AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                    allClips.Add(clip);
                }
            }
        }
#endif
        return allClips;
    }

    public void StartRec()
    {
#if UNITY_EDITOR
        if (recordInit /* !StudyScript.Instance.tutroial_done*/) return;

        countdownText.gameObject.SetActive(true);
        recordingCoroutine = StartCoroutine(StartRecordingWithCountdown());
//#else
//        ModelKeypadManager.Instance.Btn_5.SetActive(false);
#endif
    }

    private IEnumerator StartRecordingWithCountdown()
    {
#if UNITY_EDITOR
        float countdown = 5f; // Countdown duration in seconds

        while (countdown > 0)
        {
            countdownText.text = Mathf.Ceil(countdown).ToString(); // Update the countdown text
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        countdownText.text = ""; // Clear the countdown text
        countdownText.gameObject.SetActive(false);

        // Reset the recorder and start fresh
        ResetRecorder();

        if (_recorder == null)
        {
            _recorder = new GameObjectRecorder(_objectToRecord);
        }

        recordInit = true;
        _recorder.BindComponentsOfType<Transform>(_objectToRecord, true);
        Debug.Log("Should bind:  " + _objectToRecord +"    See Recorder: " +_recorder);
        _canRecord = true;
        InfoOverlay.Instance.ManageRecImage();

        if (recordMirroredObject)
        {
            _recorder.BindComponentsOfType<Transform>(_MirroredObjectToRecord, true);
            _canRecord = true;
          Debug.Log("Rec MirroredObject bound done. See: " + _recorder +"  and: " + _MirroredObjectToRecord);
        }
        else Debug.Log("Faild to Bound MirroredObject.");
        for (int i = 0; i < additionalRecordObjects.Count; i++)
        {
            _recorder.BindComponentsOfType<Transform>(additionalRecordObjects[i], true);
            //howText("Additional Bind is done for: " + additionalRecordObjects[i]);
        }
     
#else
        yield return new WaitForSeconds(1f);
#endif
    }

    // Method to reset the recorder to make sure it starts from scratch
    private void ResetRecorder()
    {
#if(UNITY_EDITOR)
        if (_recorder != null)
        {
            _recorder.ResetRecording(); // Reset any previous recordings
        }
#endif
    }

    public void StopRecording()
    {
        if (recordingCoroutine != null)
        {
            // Stop the countdown coroutine
            StopCoroutine(recordingCoroutine);
            if (_studyManager != null)
            {
                _studyManager.StopRecTask();
                _studyManager.StopRecSecondTimeTask();
            }
            recordingCoroutine = null;
            countdownText.gameObject.SetActive(false);
        }

        if (recordInit)
        {
            //StudyScript.Instance.RecordClapTask();
            //StudyScript.Instance.RecordNewJumpAnim();
            recordInit = false;
            _canRecord = false;
            InfoOverlay.Instance.ManageRecImage();
            StartCoroutine(CloseEyesAndContinue());      
        }
    }

    public void CreateNewClip()
    {
#if UNITY_EDITOR
        AnimationClip newClip = new AnimationClip();
        newClip.frameRate = _frameRate;
        _currentClip = newClip;
        _currentClip.name = _clipName;

        for (int i = 0; i < allClips.Count; i++)
        {
            if (allClips[i] != null && allClips[i].name == _clipName + "_Anim")
            {
                allClips.RemoveAt(i);
                Debug.Log("Clip with same Name already exist! Not more, we deleted for you :)");
                return;
            }
        }

        string savePath = string.Format(_savePath + "/{0}.anim", _clipName);

        AssetDatabase.CreateAsset(_currentClip, savePath);

        if (!allClips.Contains(_currentClip))
        {
            allClips.Add(_currentClip);
        }

        InfoOverlay.Instance.ShowText("New Anim: "+ _currentClip.name);
#endif
    }

    void AddMotionToAnimator()
    {
#if UNITY_EDITOR
        AnimatorStateMachine rootStateMachine = _animController.layers[0].stateMachine;

        // �berpr�fen, ob ein State mit dem gleichen Namen bereits existiert
        AnimatorState existingState = null;
        foreach (var state in rootStateMachine.states)
        {
            if (state.state.name == _currentClip.name)
            {
                existingState = state.state;
                break;
            }
        }

        // Wenn ein State mit dem gleichen Namen existiert
        if (existingState != null)
        {
            // Wenn der State keine Motion hat, die aktuelle Motion zuweisen
            if (existingState.motion == null)
            {
                existingState.motion = _currentClip;
                InfoOverlay.Instance.ShowText(_currentClip.name + " set as Motion.");
            }
            // Wenn der State eine Motion hat, diese durch die neue Motion ersetzen
            else
            {
                existingState.motion = _currentClip;
                InfoOverlay.Instance.ShowText(_currentClip.name + " eingef�gt und altes ersetzt..");
            }
        }
        else
        {
            // Neuen State erstellen und die Motion zuweisen
            AnimatorState newState = rootStateMachine.AddState(_currentClip.name);
            newState.motion = _currentClip;
            InfoOverlay.Instance.ShowText(_currentClip.name + " as new State.");
        }
        OnMotionAdded?.Invoke();
#endif
    }

    public void ActivateOtherVariant(string variantName)
    {
        foreach (GameObject obj in currentVariantsToRecord)
        {
            if (obj != null)
            {
                if (obj.name == variantName)
                {
                    obj.SetActive(true);
                    AVR_Related avr_related = obj.transform.parent.transform.parent.GetComponent<AVR_Related>();
                    _objectToRecord = avr_related.GetActiveVaraint(); // Neues Modell setzen
                    _MirroredObjectToRecord = avr_related.mirroredObjects;
                    _animatorMirrored = avr_related.activeMirrored.GetComponent<Animator>();

                    // Recorder nach dem Variantwechsel zur�cksetzen und neu binden
                    ResetRecorder();
                    _recorder = new GameObjectRecorder(_objectToRecord);
                    _recorder.BindComponentsOfType<Transform>(_objectToRecord, true);

                    // Eventuelle zus�tzliche Bindungen
                    if (recordMirroredObject && _MirroredObjectToRecord != null)
                    {
                        _recorder.BindComponentsOfType<Transform>(_MirroredObjectToRecord, true);
                    }

                    InfoOverlay.Instance.ShowText("Variant wurde ge�ndert zu:  " + obj);
                }
                else
                {
                    obj.SetActive(false);
                }
            }
        }
    }


    public void ActivateOtherModel(string objectName)
    {
            foreach (GameObject obj in allMainObjectsToRecord)
            {
                if (obj != null)
                {
                    if (obj.name == objectName)
                    {
                        obj.SetActive(true);
                        AVR_Related avr_related = obj.GetComponent<AVR_Related>();
                        _objectToRecord = avr_related.GetActiveVaraint(); // Set Original Model to new Model
                        _MirroredObjectToRecord = avr_related.mirroredObjects; // Set Mirror Model to new Model. Search for Parent = Get all Mirrored Objects (light, etc.)
                        _animatorMirrored = avr_related.activeMirrored.GetComponent<Animator>();
                        currentVariantsToRecord = avr_related.mirroredVaraints;
                        Animator animatorModell = _objectToRecord.GetComponent<Animator>();
                        SetModel();
                        _animList.UpdateAvatar();
#if UNITY_EDITOR
                        RuntimeAnimatorController animationController = animatorModell.runtimeAnimatorController;
                        _animController = animationController as AnimatorController;
#endif
                        _animList.SetUpAnimList();
                        CreateNewClip();                    
                    //StudyScript.Instance.SwitchModelTask();
                    }
                    else
                    {
                        obj.SetActive(false);
                    }
                }
            }
    }

    public void ManageOwnRigRecording() // Set Recording to own model and deactivate the Mirror
    {
#if UNITY_EDITOR
        if (_studyManager != null)
        {
            if (!_studyManager.once) return;
        }
       
        if (_MirroredObjectToRecord.activeSelf)
        {
            _MirroredObjectToRecord.SetActive(false);
            _recorder = new GameObjectRecorder(_ownRigModel);
            _recorder.BindComponentsOfType<Transform>(_ownRigModel, true);
            OnChangeModel?.Invoke();
            InfoOverlay.Instance.ShowText("Model set to Own Rig: " + _ownRigModel.name);

            ModelKeypadManager.Instance.Switch9BtnsActivStatusStudy(false);
            ModelKeypadManager.Instance.Btn_3.SetActive(true);
            ModelKeypadManager.Instance.Btn_2.SetActive(true);
            CreateNewClip();
        }
        else
        {
            _MirroredObjectToRecord.SetActive(true);
            ModelKeypadManager.Instance.Switch9BtnsActivStatusStudy(false);
            ModelKeypadManager.Instance.Btn_3.SetActive(true);
            ModelKeypadManager.Instance.Btn_2.SetActive(true);
            ModelKeypadManager.Instance.Btn_4.SetActive(true);
            ModelKeypadManager.Instance.Btn_6.SetActive(true);
            ModelKeypadManager.Instance.Btn_8.SetActive(true);
            ModelKeypadManager.Instance.Btn_9.SetActive(true);
            if (_studyManager == null)
            {
                ModelKeypadManager.Instance.Btn_1.SetActive(true);
                ModelKeypadManager.Instance.Btn_5.SetActive(true);
                ModelKeypadManager.Instance.Btn_7.SetActive(true);
            }
            SetModel();
        }
#endif
    }

    public void SetModel()
    {
#if UNITY_EDITOR
        _recorder = new GameObjectRecorder(_objectToRecord);
        _recorder.BindComponentsOfType<Transform>(_objectToRecord, true);

        if (recordMirroredObject && _MirroredObjectToRecord != null)
        {
            _recorder.BindComponentsOfType<Transform>(_MirroredObjectToRecord, true);
        }

        foreach (GameObject additionalObj in additionalRecordObjects)
        {
            if (additionalObj != null)
            {
                _recorder.BindComponentsOfType<Transform>(additionalObj, true);
            }
        }
#endif
        avr_mirrorTransformer.transformModel = _objectToRecord.transform; // Set new Model in Transformer.
        avr_mirrorTransformer.modelAnimator = _objectToRecord.GetComponent<Animator>(); // Set new Model in Transformer.
        mirroredTransfromManager._lateMirroredObject = _objectToRecord.GetComponentInChildren<LateMirroredObject>();
        OnChangeModel?.Invoke();
        //InfoOverlay.Instance.ShowText("New Model for Recorder: " + _objectToRecord);
    }

    public List<GameObject> GetChildrenWithAnimator()
    {
        List<GameObject> childrenWithAnimator = new List<GameObject>();
        if (_MirroredObjectToRecord != null)
        {
            foreach (Transform child in _MirroredObjectToRecord.transform)
            {
                if (child.GetComponent<Animator>() != null)
                {
                    childrenWithAnimator.Add(child.gameObject);
                }
            }
        }
        return childrenWithAnimator;
    }

    public void ToggleDebugSettings()
    {
        // Suche nach den zwei Kindobjekten "DebugBones & DebugBonesOVRSkeletonFullBody" im aktuellen _MirroredObjectToRecord
        GameObject debugBones = _MirroredObjectToRecord.transform.parent.GetChild(0).gameObject;
        GameObject debugSkeletonFullBody = _MirroredObjectToRecord.transform.parent.GetChild(1).gameObject;

        // Pr�fen, ob das erste Kindobjekt aktiv ist
        if (debugBones.activeSelf)
        {
            // Beide deaktivieren, wenn sie aktiv sind
            debugBones.SetActive(false);
            debugSkeletonFullBody.SetActive(false);
        }
        else
        {
            // Beide aktivieren, wenn sie inaktiv sind
            debugBones.SetActive(true);
            debugSkeletonFullBody.SetActive(true);
        }

        // Optional: Status von debugActiv aktualisieren, falls du diese Variable noch behalten m�chtest
        debugActiv = debugBones.activeSelf; 
    }

    private IEnumerator CloseEyesAndContinue()
    {
        yield return StartCoroutine(CloseEyes());

#if UNITY_EDITOR
        _recorder.SaveToClip(_currentClip);
        AssetDatabase.SaveAssets();
        AddMotionToAnimator();
        InfoOverlay.Instance.ShowText(_currentClip.name +" Saved.");
#endif

        yield return StartCoroutine(OpenEyes());
    }

    private IEnumerator CloseEyes()
    {
        if (!blackScreenImage_1.gameObject.activeSelf)
        {
            // Hol die RectTransform des Elternobjekts
            RectTransform parentRectTransform = rectTransform1.parent as RectTransform;

            if (parentRectTransform != null)
            {
                // Setze die Breite der BlackScreenImages auf die Breite des Elternobjekts
                rectTransform1.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRectTransform.rect.width);
                rectTransform2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRectTransform.rect.width);
            }

            blackScreenImage_1.gameObject.SetActive(true);
            blackScreenImage_2.gameObject.SetActive(true);
        }

        float halfDuration = blinkDuration / 2f;
        Vector2 initialPos1 = rectTransform1.anchoredPosition;
        Vector2 initialPos2 = rectTransform2.anchoredPosition;
        Vector2 targetPos1 = new Vector2(initialPos1.x, 0);
        Vector2 targetPos2 = new Vector2(initialPos2.x, 0);

        // Augenlider schlie�en
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / halfDuration;
            rectTransform1.anchoredPosition = Vector2.Lerp(initialPos1, targetPos1, normalizedTime);
            rectTransform2.anchoredPosition = Vector2.Lerp(initialPos2, targetPos2, normalizedTime);
            yield return null;
        }

        rectTransform1.anchoredPosition = targetPos1;
        rectTransform2.anchoredPosition = targetPos2;

        // Augen geschlossen halten
        yield return new WaitForSeconds(closedDuration);
    }

    private IEnumerator OpenEyes()
    {
        //_layerMaskManager.ToggleEverything();
        float halfDuration = blinkDuration / 2f;
        Vector2 initialPos1 = rectTransform1.anchoredPosition;
        Vector2 initialPos2 = rectTransform2.anchoredPosition;
        Vector2 targetPos1 = new Vector2(initialPos1.x, rectTransform1.rect.height);
        Vector2 targetPos2 = new Vector2(initialPos2.x, -rectTransform2.rect.height);

        // Augenlider �ffnen
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / halfDuration;
            rectTransform1.anchoredPosition = Vector2.Lerp(initialPos1, targetPos1, normalizedTime);
            rectTransform2.anchoredPosition = Vector2.Lerp(initialPos2, targetPos2, normalizedTime);
            yield return null;
        }

        rectTransform1.anchoredPosition = targetPos1;
        rectTransform2.anchoredPosition = targetPos2;
        ModelKeypadManager.Instance.Btn_5.SetActive(false);
    }

}
