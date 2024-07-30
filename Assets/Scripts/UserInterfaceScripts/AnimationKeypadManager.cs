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
    [SerializeField]
    GameObject _curvedUIAnimListStudy; // Only for Study Case.
    [SerializeField]
    GameObject _curvedUIAnimListBindingVariant;

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

    public void InitializeAnimKeyPadManager()
    {
        _animator = AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<Animator>();
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

    public void SetCurvedUiStudy(GameObject modelMenu)
    {  
            _animListUI.SetUpAnimList();
    }

    public void SetCurvedUiBindingVariant()
    {
        if (_curvedUIAnimListBindingVariant.activeSelf)
        {
            _curvedUIAnimListBindingVariant.SetActive(false);
        }
        else
        {
            _animListUI.SetUpAnimList();
            _curvedUIAnimListBindingVariant.SetActive(true);
        }
    }

    public void SwitchShowCaseModel()
    {
        GameObject animModel = AVRGameObjectRecorder.Instance._objectToRecord;
        if (animModel.activeSelf)
        {
            animModel.SetActive(false);
        }
        else
        {
            animModel.SetActive(true);
        }
    }

    #region Animation Control Functions

    public void PlayNextAnimation()
    {
        if (_animator.runtimeAnimatorController != null)
        {
            var clips = _animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                _animator.enabled = true;
                currentAnimationIndex = (currentAnimationIndex + 1) % clips.Length;
                _animator.Play(clips[currentAnimationIndex].name, -1, 0f);
                Debug.Log("Playing Next Animation: " + clips[currentAnimationIndex].name + " on animator:  " + _animator);
            }
        }
    }

    public void PlayPreviousAnimation()
    {
        if (_animator.runtimeAnimatorController != null)
        {
            var clips = _animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                currentAnimationIndex = (currentAnimationIndex - 1 + clips.Length) % clips.Length;
                _animator.Play(clips[currentAnimationIndex].name, -1, 0f);
                Debug.Log("Playing Previous Animation: " + clips[currentAnimationIndex].name + " on GO = " + _animator.gameObject.name);
            }
        }
    }

    #endregion

    /// Startet den Recorder und f�ngt an Aufzunehmen.
    public void RecorderLogic(GameObject callingBtn)
    {
        if (callingBtn.GetComponent<AVR_SingleBtn>()._EnabledIcon.activeSelf) // Startet und Stop Logik der Aufnahmen.
        {
            AVRGameObjectRecorder.Instance.StopRecording(); // The current AVRRecorder from the Target Model!
        }
        else if (callingBtn.GetComponent<AVR_SingleBtn>()._DisabledIcon.activeSelf)
        {
            Debug.Log("Start Record");
            AVRGameObjectRecorder.Instance.StartRec(); // The current AVRRecorder from the Target Model!                                                                                                                                 //AnimVRigRecorder.Instance.StopRecordingThreadVar();  // Thread Alternative
        }
    }
}
