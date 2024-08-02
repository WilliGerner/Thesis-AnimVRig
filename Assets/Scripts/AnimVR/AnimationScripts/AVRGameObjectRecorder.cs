using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using Oculus.Movement.Effects;
using UnityEngine.UI;

public class AVRGameObjectRecorder : MonoBehaviour
{
    [Tooltip("Your own current Rig")]
    public GameObject _ownRigModel;
    [Tooltip("The Object you want to Record")]
    public GameObject _objectToRecord;
    public GameObject _MirroredObjectToRecord;
    public GameObjectRecorder _recorder;
    AnimationClip _currentClip;
    public List<AnimationClip> allClips;
    AnimatorController _animController;
    public Animator _animatorMirrored;
    public MirroredTransformManager mirroredTransformManager;

    public List<GameObject> additionalRecordObjects;
    public List<GameObject> AllMainObjectsToRecord;

    [SerializeField]
    private GameObject _blackScreenCanvas;
    private RawImage blackScreenImage_1;
    private RawImage blackScreenImage_2;
    private RectTransform rectTransform1;
    private RectTransform rectTransform2;
    // Dauer f�r das Schlie�en und �ffnen der Augenlider
    public float blinkDuration = 0.5f;
    // Dauer, wie lange die Augen geschlossen bleiben
    public float closedDuration = 0.2f;

    public bool _canRecord = false;
    public bool recordInit = false;
    public bool recordMirroredObject = false;
    bool debugActiv = false;



    public TextMeshProUGUI countdownText; // Add this for the countdown timer

    public event Action OnMotionAdded;
    public event Action OnChangeModel;

    private static readonly Queue<System.Action> _executionQueue = new Queue<System.Action>();

    #region Inspector Variables
    public bool isActiv = false;

    [Tooltip("Must start with Assets/ and will be normaly Set at The Start to RecordObkjectNameFolder.")]
    [SerializeField] string _saveFolderLocation = "Assets/AnimationContent/";

    public string _clipName;
    [SerializeField] float _frameRate = 60f;

    [SerializeField]
    MirroredTransformManager mirroredTransfromManager; // In Study Case something else than normal ( Layer Menue Study)
    [SerializeField]
    AVR_MirrorTransformer avr_mirrorTransformer;

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
        RuntimeAnimatorController animationController = _objectToRecord.GetComponent<Animator>().runtimeAnimatorController;
        _animController = animationController as AnimatorController;
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
    }

    private void Update()
    {
        if (_canRecord)
        {
            _recorder.TakeSnapshot(Time.deltaTime);
            Debug.Log("Recording: " + _recorder.isRecording);
        }

        lock (_executionQueue)  // new
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    public GameObject GetActiveElement()
    {
        foreach (var element in AllMainObjectsToRecord)
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
        return allClips;
    }

    public void StartRec()
    {
        if (recordInit || !StudyScript.Instance.tutroial_done) return;
        CreateNewClip();
        countdownText.gameObject.SetActive(true);
        StartCoroutine(StartRecordingWithCountdown());
    }

    private IEnumerator StartRecordingWithCountdown()
    {
        float countdown = 5f; // Countdown duration in seconds

        while (countdown > 0)
        {
            countdownText.text = Mathf.Ceil(countdown).ToString(); // Update the countdown text
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }
        countdownText.text = ""; // Clear the countdown text
        countdownText.gameObject.SetActive(false);


        if (recordInit) yield break;

        if (_recorder == null)
        {    _recorder = new GameObjectRecorder(_objectToRecord);}

        recordInit = true;
        _recorder.BindComponentsOfType<Transform>(_objectToRecord, true);
        Debug.Log("Rec Target Object: " + _recorder.isRecording);
        _canRecord = true;
        InfoOverlay.Instance.ManageRecImage();
        if (recordMirroredObject)
        {
            _recorder.BindComponentsOfType<Transform>(_MirroredObjectToRecord, true);
            _canRecord = true;
            Debug.Log("Rec MirrordObject bound done.See: " + _recorder);
        }

        for (int i = 0; i < additionalRecordObjects.Count; i++)
        {
            _recorder.BindComponentsOfType<Transform>(additionalRecordObjects[i], true);
            Debug.Log("Additional Bind is done for: " + additionalRecordObjects[i]);
        }
        InfoOverlay.Instance.ShowText("Animation Recording for " + gameObject.name + " has Initialized.");
    }

    public void StopRecording()
    {
      //  StudyScript.Instance.HitRecAndStop();
        StudyScript.Instance.RecordClapTask();
        StudyScript.Instance.RecordNewJumpAnim();
        recordInit = false;
        _canRecord = false;
        InfoOverlay.Instance.ManageRecImage();
        StartCoroutine(CloseEyesAndContinue());
    }

    private IEnumerator CloseEyesAndContinue()
    {
        yield return StartCoroutine(CloseEyes());

        // Restliche Aktionen nach dem Schlie�en der Augen
        _recorder.SaveToClip(_currentClip);
        AssetDatabase.SaveAssets();
        InfoOverlay.Instance.ShowText("Clip Name: " + _clipName + " saved");
        AddMotionToAnimator();
        yield return StartCoroutine(OpenEyes());
        blackScreenImage_1.gameObject.SetActive(false);
        blackScreenImage_2.gameObject.SetActive(false);
        InfoOverlay.Instance.ShowText("Dont forget the activ bindings before playing your new Animation!");

    }

    private IEnumerator CloseEyes()
    {
        if (!blackScreenImage_1.gameObject.activeSelf)
        {
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
    }

    public void CreateNewClip()
    {
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

        AssetDatabase.CreateAsset(_currentClip, string.Format(_saveFolderLocation + "/{0}_Anim.anim", _clipName));
        InfoOverlay.Instance.ShowText("New Clip created:  " + _clipName + " at path: " + _saveFolderLocation);

        if (!allClips.Contains(_currentClip))
        {
            allClips.Add(_currentClip);
        }
    }

    void AddMotionToAnimator()
    {
        AnimatorStateMachine rootStateMachine = _animController.layers[0].stateMachine;
        AnimatorState newState = rootStateMachine.AddState(_currentClip.name);
        newState.motion = _currentClip;
        Debug.Log(_animController.name + "    has a new Animation Added to the Controller.  AnimationName: " + _currentClip.name);
        OnMotionAdded?.Invoke();
    }
    public void ActivateOtherModel(string objectName)
    {
        {
            bool found = false;
            foreach (GameObject obj in AllMainObjectsToRecord)
            {
                if (obj != null)
                {
                    if (obj.name == objectName)
                    {
                        obj.SetActive(true);
                        AVR_Related avr_related = obj.GetComponent<AVR_Related>();
                        _objectToRecord = avr_related.activeMirrored; // Set Original Model to new Model
                        _MirroredObjectToRecord = avr_related.mirroredObjects; // Set Mirror Model to new Model. Search for Parent = Get all Mirrored Objects (light, etc.)
                        _animatorMirrored = avr_related.activeMirrored.GetComponent<Animator>();
                        Animator animatorModell = _objectToRecord.GetComponent<Animator>();
                        RuntimeAnimatorController animationController = animatorModell.runtimeAnimatorController;
                        _animController = animationController as AnimatorController;

                        found = true;
                        InfoOverlay.Instance.ShowText("New Model:  " + obj);
                        StudyScript.Instance.SwitchModelTask();
                    }
                    else
                    {
                        obj.SetActive(false);
                    }
                }
            }

            if (found)
            {
                //ResetRecorder();
                SetModel();
            }
            else
            {
                Debug.LogError("Object with name " + objectName + " not found in AllMainObjectsToRecord.");
            }
        }
    }

    public void ResetRecorder()
    {
        _recorder = null;
        _currentClip = null;
        _canRecord = false;
        recordInit = false;
        Debug.Log("Recorder reset complete.");
    }

    public void ManageOwnRigRecording() // Set Recording to own model and deactivate the Mirror
    {
        if (!StudyScript.Instance.once) return;
        if (_MirroredObjectToRecord.activeSelf)
        {
            _MirroredObjectToRecord.SetActive(false);
            _recorder = new GameObjectRecorder(_ownRigModel);
            _recorder.BindComponentsOfType<Transform>(_ownRigModel, true);
            OnChangeModel?.Invoke();
            InfoOverlay.Instance.ShowText("Model set to Own Rig for recording: " + _ownRigModel.name);
        }else
        {
            _MirroredObjectToRecord.SetActive(true);
            SetModel();
        }
    }

    public void SetModel()
    {
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
        avr_mirrorTransformer.modelObject = _objectToRecord.transform; // Set new Model in Transformer.
        mirroredTransfromManager._lateMirroredObject = _objectToRecord.GetComponentInChildren<LateMirroredObject>();
        OnChangeModel?.Invoke();
        InfoOverlay.Instance.ShowText("New Model for Recorder: " + _objectToRecord + "  oldRecorder: " + _recorder);
    }

    public List<GameObject> GetChildrenWithAnimator()
    {
        List<GameObject> childrenWithAnimator = new List<GameObject>();
        if (_objectToRecord != null)
        {
            foreach (Transform child in _objectToRecord.transform)
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
        // Search for the Two Childs  "DebugBones & DebugBonesOVRSkeletonFullBody int the current MirroredObject to Record.
        if (debugActiv)
        {
            _MirroredObjectToRecord.transform.parent.GetChild(0).gameObject.SetActive(false);
            _MirroredObjectToRecord.transform.parent.GetChild(1).gameObject.SetActive(false);

            debugActiv = false;
        }
        else
        {
            _MirroredObjectToRecord.transform.parent.GetChild(0).gameObject.SetActive(true);
            //Debug.Log(_MirroredObjectToRecord.transform.parent.GetChild(0).gameObject.name);
            _MirroredObjectToRecord.transform.parent.GetChild(1).gameObject.SetActive(true);
            //Debug.Log(_MirroredObjectToRecord.transform.parent.GetChild(1).gameObject.name);
            debugActiv = true;
        }
    }
}
