using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;
using System.Collections.Generic;
using static OVRPlugin;

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
    //[SerializeField]
    public AnimatorController _animCotnroller;
    public Animator _animatorMirrored;
    public Animator _animatorAnimationModel;

    public List<GameObject> additionalRecordObjects;

    public bool _canRecord = false;
    public bool recordInit = false;
    public bool recordMirroredObject = false;

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
                // Versuch, eine vorhandene Instanz im Spiel zu finden
                instance = FindObjectOfType<AVRGameObjectRecorder>();
                if (instance == null)
                {
                    // Erstellen eines neuen GameObjects, wenn keine Instanz gefunden wurde
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<AVRGameObjectRecorder>();
                    singletonObject.name = typeof(AVRGameObjectRecorder).ToString() + " (Singleton)";

                    // Sicherstellen, dass das Singleton-GameObject nicht zerstört wird, wenn die Szene wechselt
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Start()
    {
        _canRecord = false;
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

    public string ReturnSafePath()
    {
        return _saveFolderLocation;
    }
    public AnimationClip GetCurrentClip() // Realy needed!?
    {return _currentClip;    }

    /// <summary>
    /// Get all .anim-Files from Path with the Name of the TargetObject and returns as List.
    /// </summary>
    /// <param name="rootObjectName">The Objects Name for which we Record.</param>
    /// <returns></returns>
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

    public void StartRec( ) // Only in Playmode
    {
        if (recordInit) return;

        if (_recorder == null)
        { _recorder = new GameObjectRecorder(_objectToRecord); } // `?

        recordInit = true;

        
        _recorder.BindComponentsOfType<Transform>(_objectToRecord, true);
        Debug.Log("Rec Target Object: " + _recorder.isRecording );
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

    IEnumerator StopRecordCoroutine() // Coroutine ist hier überflüssig. IM AUGE BEHALTEN! 
    {
        if (!recordInit || _recorder == null) yield return null;
        recordInit = false;
        _canRecord = false;
        _recorder.SaveToClip(_currentClip);
        AssetDatabase.SaveAssets();
        Debug.Log("Should Save: ClipWithName: " + _currentClipName + "  at path: " + _saveFolderLocation);
        //Debug.Log("Following Clip filled with recording Data: " + currentRecorder._currentClip);
        AddMotionToAnimator();
        Debug.Log("Animation Recording for " + _objectToRecord.name + " has Finished and Stopped");
        yield return null;
    }

    public void DeleteRecording(AVRGameObjectRecorder currentRecorder)
    {
        //if (currentRecorder._canRecord)
        //{ Debug.LogWarning("Cannot delete when recording!");return;}

        //if (!AssetDatabase.Contains(currentRecorder._currentClip))
        //{
        //    Debug.LogWarning("Clip Has not been saved yet.");
        //    return;
        //}
        //else
        //{
        //    Debug.Log("ClipLocation:  " + currentRecorder._saveFolderLocation + currentRecorder._currentClipName + ".anim");
        //    AssetDatabase.DeleteAsset(currentRecorder._saveFolderLocation + currentRecorder._currentClipName + ".anim");
        //    Debug.Log("Clip has been DELETED");
        //}
    }
    public void CreateNewClip( )
    {
        AnimationClip newClip = new AnimationClip();

        newClip.frameRate = _frameRate;
        _currentClip = newClip;
        _currentClip.name = _clipName;
        _currentClipName = _clipName;

        for (int i = 0; i < allClips.Count; i++) // SearchFOrExistingClip and Delte it,  if sp.
        {
            if (allClips[i] != null)
            {
                if (allClips[i].name == _clipName + "_Anim")
                {
                    allClips.RemoveAt(i);// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    Debug.Log("Clip with same Name alrdy exist! Not more, we deleted for you :)");
                    return;
                }
            }
        }
        AssetDatabase.CreateAsset(_currentClip, string.Format(_saveFolderLocation+"/{0}_Anim.anim", _clipName));
        Debug.LogWarning("New Clip created:  " + _clipName + "at path: " + _saveFolderLocation);       
        if (!allClips.Contains(_currentClip))
        {
            allClips.Add(_currentClip);          
          //  AddMotionToAnimator(gameObject.GetComponent<AnimatorController>());
        }

    }

    void AddMotionToAnimator()
    {
        AnimatorStateMachine rootStateMachine = _animCotnroller.layers[0].stateMachine;
        AnimatorState newState = rootStateMachine.AddState(_currentClip.name);
        newState.motion = _currentClip;
        Debug.Log(_animCotnroller.name + "    has a new Animation Added to the Controller.  AnimationName: " + _currentClip.name);


        //string path = "";
        //string[] searchedAsset = AssetDatabase.FindAssets(gameObject.name, new[] { "Assets/Test/" + gameObject.name });
        //if (searchedAsset.Length >= 1)
        //{
        //    path = AssetDatabase.GUIDToAssetPath(searchedAsset[0]);
        //    animConti = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
        //}
        //else if (searchedAsset.Length <= 1)
        //{
        //    Debug.Log(searchedAsset + " is to short. Please check the path. Cant find asset");
        //}
        // Erstelle einen neuen State in der StateMachine
       

        // Weise diesem State eine AnimationClip zu
       
        //AnimatorStateMachine rootStateMachine = animCotnroller.layers[0].stateMachine;
        //animCotnroller.AddMotion(_currentClip);

        //rootStateMachine.defaultState.motion = _currentClip;
    }

    #region Animator Functions (Play and Stop AnimPlayback=
    public void PlayAnimation(Animator animator, AnimationClip clip)
    {
        clip.wrapMode = WrapMode.Once;
        var runtimeController = animator.runtimeAnimatorController;

        animator.Play(clip.name, 0, 0);
        animator.Update(Time.deltaTime);
        Debug.Log("Animation " + clip.name + " Play");
    }

    public void StopPlayback(Animator animator)
    {
        animator.StopPlayback();
        Debug.Log(animator.gameObject.name + "     Stoped Playback");
    }

    public void StartPlayback(Animator animator)
    {
        Debug.Log("Anim is: " + animator + "   Animator is in Playback Mode");
        animator.StartPlayback();
        // animator.Play(clip.name, -1, 0)
    }
    #endregion

}
