using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationKeypadManager : MonoBehaviour
{
    public AnimationList _animListUI;
    Animator _animator;

    [Header("Record Area")]
    [SerializeField]
    bool isBtnRecorder;

    [SerializeField]
    [Tooltip("1 means the first Top Left, 9 mean the last Bottom Right.")]
    List<GameObject> all9AnimBtns = new List<GameObject>();

    [SerializeField]
    GameObject _curvedUIAnimList;

    private static AnimationKeypadManager instance;
    private int currentAnimationIndex = 0; // Aktueller Index der Animation in der Liste

    public Animation animationComponent;
    private AnimationClip[] animationClips;

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

    void Start()
    {
        //if (animationComponent == null)
        //{
        //    animationComponent = GetComponent<Animation>();
        //}

        //if (animationComponent != null)
        //{
        //    int clipCount = animationComponent.GetClipCount();
        //    animationClips = new AnimationClip[clipCount];
        //    int i = 0;

        //    foreach (AnimationState state in animationComponent)
        //    {
        //        animationClips[i] = state.clip;
        //        i++;
        //    }
        //}
        //else
        //{
        //    Debug.LogError("Keine Animation-Komponente gefunden!");
        //}
    }

    public void InitializeAnimKeyPadManager()
    {
        Debug.Log("Init AnimManager..set _animatorAnimationModel");
        _animator = AVRGameObjectRecorder.Instance._animatorAnimationModel;
    }

    public void SetCurvedUi()
    {
        if (_curvedUIAnimList.activeSelf)
        {
            _curvedUIAnimList.SetActive(false);
        }
        else
        {
            _animListUI.SetUpAnimList();
            _curvedUIAnimList.SetActive(true);
        }
    }

    #region Animation Control Functions

    //public void PlayNextAnimation()
    //{
    //    if (_animator.runtimeAnimatorController != null)
    //    {
    //        var clips = _animator.runtimeAnimatorController.animationClips;
    //        if (clips.Length > 0)
    //        {
    //            _animator.enabled = true;
    //            currentAnimationIndex = (currentAnimationIndex + 1) % clips.Length;
    //            _animator.Play(clips[currentAnimationIndex].name, -1, 0f);
    //            Debug.Log("Playing Next Animation: " + clips[currentAnimationIndex].name + " on animator:  " + _animator);
    //        }
    //    }
    //}

    //public void PlayPreviousAnimation()
    //{
    //    if (_animator.runtimeAnimatorController != null)
    //    {
    //        var clips = _animator.runtimeAnimatorController.animationClips;
    //        if (clips.Length > 0)
    //        {
    //            currentAnimationIndex = (currentAnimationIndex - 1 + clips.Length) % clips.Length;
    //            _animator.Play(clips[currentAnimationIndex].name, -1, 0f);
    //            Debug.Log("Playing Previous Animation: " + clips[currentAnimationIndex].name +" on GO = " + _animator.gameObject.name);
    //        }
    //    }
    //}

    public void PlayNextAnimation()
    {
        if (animationClips != null && animationClips.Length > 0)
        {
            currentAnimationIndex = (currentAnimationIndex + 1) % animationClips.Length;
            AnimationClip clip = animationClips[currentAnimationIndex];
            if (clip != null)
            {
                animationComponent.clip = clip;
                animationComponent.Play();
                Debug.Log("Playing Next Animation: " + clip.name);
            }
            else
            {
                Debug.LogError("Animation Clip ist null bei Index: " + currentAnimationIndex);
            }
        }
    }

    public void PlayPreviousAnimation()
    {
        if (animationClips != null && animationClips.Length > 0)
        {
            currentAnimationIndex = (currentAnimationIndex - 1 + animationClips.Length) % animationClips.Length;
            AnimationClip clip = animationClips[currentAnimationIndex];
            if (clip != null)
            {
                animationComponent.clip = clip;
                animationComponent.Play();
                Debug.Log("Playing Previous Animation: " + clip.name);
            }
            else
            {
                Debug.LogError("Animation Clip ist null bei Index: " + currentAnimationIndex);
            }
        }
    }


    public void DeleteCurrentAnimation()
    {
        Debug.LogWarning("Deleting animations at runtime is not supported by default Unity API. This function needs to be implemented based on specific project requirements.");
        // Hier müsste man eine spezielle Logik implementieren, die abhängig vom Projekt ist.
    }

    #endregion

    #region  RecorderFunctions

    /// Startet den Recorder und fängt an Aufzunehmen.
    public void RecorderLogic(GameObject callingBtn)
    {
        if (callingBtn.GetComponent<AVR_SingleBtn>()._EnabledIcon.activeSelf) // Startet und Stop Logik der Aufnahmen.
        {
            AVRGameObjectRecorder.Instance.StopRecording(); // The current AVRRecorder from the Target Model!
        }
        else if(callingBtn.GetComponent<AVR_SingleBtn>()._DisabledIcon.activeSelf)
        {
            Debug.Log("Start Record");
            AVRGameObjectRecorder.Instance.StartRec(); // The current AVRRecorder from the Target Model!                                                                                                                                 //AnimVRigRecorder.Instance.StopRecordingThreadVar();  // Thread Alternative
        }
    }
    #endregion
}
