using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Newer Version from AVR_Recorder
/// </summary>
public class AVRGameObjectRecorder : MonoBehaviour
{
    [Tooltip("The Object you want to Record")]
    public GameObject _objectToRecord;
    public GameObject _MirroredObjectToRecord;
    public GameObjectRecorder _recorder;
    AnimationClip _currentClip;
    public List<AnimationClip> allClips;
    AnimatorController _animController;
    public Animator _animatorMirrored;

    public List<GameObject> additionalRecordObjects;
    public List<GameObject> AllMainObjectsToRecord;

    public bool _canRecord = false;
    public bool recordInit = false;
    public bool recordMirroredObject = false;
    bool debugActiv =false;

    public event Action OnMotionAdded;

    #region Inspector Variables
    public bool isActiv = false;

    [Tooltip("Must start with Assets/ and will be normaly Set at The Start to RecordObkjectNameFolder.")]
    [SerializeField] string _saveFolderLocation = "Assets/AnimationContent/";

    [SerializeField] string _clipName;
    private string _currentClipName;
    [SerializeField] float _frameRate = 60f;

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
                    DontDestroyOnLoad(singletonObject);
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
    }

    private void Update()
    {
        if (_canRecord)
        {
            _recorder.TakeSnapshot(Time.deltaTime);
            Debug.Log("Recording: " + _recorder.isRecording);
        }
    }

    public GameObject GetActiveElement()
    {
        foreach (var element in AllMainObjectsToRecord)
        {
            if (element.activeSelf)  // Prüfe, ob das GameObject aktiv ist
            {
                return element;  // Gebe das aktive GameObject zurück
            }
        }
        return null;  // Gebe null zurück, wenn kein Element aktiv ist
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
        if (recordInit) return;

        if (_recorder == null)
        {
            _recorder = new GameObjectRecorder(_objectToRecord);
        }

        recordInit = true;
        _recorder.BindComponentsOfType<Transform>(_objectToRecord, true);
        Debug.Log("Rec Target Object: " + _recorder.isRecording);
        _canRecord = true;
        Debug.Log("Rec TargetObject bound done.See: " + _objectToRecord.name);

        if (recordMirroredObject)
        {
            Debug.Log("Rec Mirrored Object: true");
            _recorder.BindComponentsOfType<Transform>(_MirroredObjectToRecord, true);
            _canRecord = true;
            Debug.Log("Rec MirrordObject bound done.See: " + _recorder);
        }

        for (int i = 0; i < additionalRecordObjects.Count; i++)
        {
            _recorder.BindComponentsOfType<Transform>(additionalRecordObjects[i], true);
            Debug.Log("Additional Bind is done for: " + additionalRecordObjects[i]);
        }

        Debug.Log("Animation Recording for " + gameObject.name + " has Initialized.");
    }

    public void StopRecording()
    {
        StartCoroutine(StopRecordCoroutine());
    }

    IEnumerator StopRecordCoroutine()
    {
        if (!recordInit || _recorder == null) yield return null;
        recordInit = false;
        _canRecord = false;
        _recorder.SaveToClip(_currentClip);
        AssetDatabase.SaveAssets();
        Debug.Log("Should Save: ClipWithName: " + _currentClipName + "  at path: " + _saveFolderLocation);
        AddMotionToAnimator();
        Debug.Log("Animation Recording for " + _objectToRecord.name + " has Finished and Stopped");
        yield return null;
    }

    public void DeleteRecording(AVRGameObjectRecorder currentRecorder)
    {
        // Implement deletion logic if needed
    }

    public void CreateNewClip()
    {
        AnimationClip newClip = new AnimationClip();
        newClip.frameRate = _frameRate;
        _currentClip = newClip;
        _currentClip.name = _clipName;
        _currentClipName = _clipName;

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
        Debug.LogWarning("New Clip created:  " + _clipName + " at path: " + _saveFolderLocation);

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

    #region Animator Functions (Play and Stop AnimPlayback)
    //public void PlayAnimation(Animator animator, AnimationClip clip)
    //{
    //    clip.wrapMode = WrapMode.Once;
    //    var runtimeController = animator.runtimeAnimatorController;
    //    animator.Play(clip.name, 0, 0);
    //    animator.Update(Time.deltaTime);
    //    Debug.Log("Animation " + clip.name + " Play");
    //}

    //public void StopPlayback(Animator animator)
    //{
    //    animator.StopPlayback();
    //    Debug.Log(animator.gameObject.name + "     Stopped Playback");
    //}

    //public void StartPlayback(Animator animator)
    //{
    //    Debug.Log("Anim is: " + animator + "   Animator is in Playback Mode");
    //    animator.StartPlayback();
    //}
    #endregion

    #region New Functions
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
                        Debug.LogWarning("New Model:  " + obj +"  and string Name was: "+ objectName);
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
        _currentClipName = string.Empty;
        _canRecord = false;
        recordInit = false;
        Debug.Log("Recorder reset complete.");
    }

    public void SetModel()
    {
        Debug.LogWarning("New Model for Recorder: " + _objectToRecord +"  oldRecorder: " + _recorder);
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

        Debug.Log("Model set for recording: " + _objectToRecord.name);
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
            _MirroredObjectToRecord.transform.parent.transform.parent.GetChild(0).gameObject.SetActive(false); 
            _MirroredObjectToRecord.transform.parent.transform.parent.GetChild(1).gameObject.SetActive(false);
            debugActiv = false;
        }
        else
        {
            _MirroredObjectToRecord.transform.parent.transform.parent.GetChild(0).gameObject.SetActive(true);
            _MirroredObjectToRecord.transform.parent.transform.parent.GetChild(1).gameObject.SetActive(true);
            debugActiv = true;
        }
    }
    #endregion
}
