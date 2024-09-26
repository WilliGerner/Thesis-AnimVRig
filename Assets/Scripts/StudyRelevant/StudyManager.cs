using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using Oculus.Interaction;

public class StudyManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI headerText;

    [SerializeField]
    private TextMeshProUGUI taskText;

    [SerializeField]
    private TextMeshProUGUI progressText;

    [SerializeField]
    private Image taskStatusImage;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private Color notDoneColor = Color.red;
    [SerializeField]
    private Color doneColor = Color.green;

    [SerializeField]
    private List<TaskData> sceneTasks_1;
    [SerializeField]
    private List<TaskData> sceneTasks_2;
    [SerializeField]
    private List<TaskData> sceneTasks_3;

    private List<TaskData> currentTasks;
    private int currentTaskIndex = 0;

    [SerializeField]
    private GameObject skinnedMeshRendererStartModel;
    public bool once;
    public bool canStopRecord = false;
    [SerializeField]
    InteractableUnityEventWrapper debugWrapper;
    [SerializeField]
    InteractableUnityEventWrapper closeTransformerWrapper;
    [SerializeField]
    InteractableUnityEventWrapper recorderWrapper;
    [SerializeField]
    InteractableUnityEventWrapper animListWrapper;

    [SerializeField]
    InteractableUnityEventWrapper bindingsWrapper;
    [SerializeField]
    InteractableUnityEventWrapper transformerWrapper;
    [SerializeField]
    InteractableUnityEventWrapper mirrorWrapper;
    [SerializeField]
    InteractableUnityEventWrapper variantWrapper;
    [SerializeField]
    InteractableUnityEventWrapper modelWrapper;
    [SerializeField]
    InteractableUnityEventWrapper rootMotionWrapper;
    //InteractableUnityEventWrapper animListBindingsWrapper;
    //[SerializeField]
    //InteractableUnityEventWrapper animListReturnWrapper;

    //public bool canHitDebug = false;
    public Material greenBtnMaterial, redBtnMaterial;

    [SerializeField]
    GameObject layerBindingsMenu;
    [SerializeField]
    GameObject setToSpawnPointBtn;

    [SerializeField]
    GameObject blueBigBtn;

    [SerializeField]
    CanvasGroup canvasGroup;

    public bool scene_1_done, scene_2_done, scene_3_done;


    private static StudyManager instance;
    public static StudyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StudyManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<StudyManager>();
                    singletonObject.name = typeof(StudyManager).ToString() + " (Singleton)";
                }
            }
            return instance;
        }
    }

    private void Start()
    {
        
        skinnedMeshRendererStartModel.GetComponent<SkinnedMeshRenderer>().enabled = false;
        currentTasks = sceneTasks_1;
        taskStatusImage.color = notDoneColor; 
        StartCoroutine(PlayTaskIntro(currentTasks[currentTaskIndex]));
        SetUpScene_1();
    }


    public void OnlyOnceSetMeshRenderer()
    {
        if (!once)
        {
            once = true;
            skinnedMeshRendererStartModel.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
    }

    private IEnumerator PlayTaskIntro(TaskData task)
    {
        headerText.text = task.headerDescription;
        task.ChangeButtonColors();

        // Wenn Audio vorhanden ist, warte bis es fertig ist, ansonsten sofort weitermachen
        if (task.introAudioClip != null)
        {
            audioSource.clip = task.introAudioClip;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        taskText.text = task.taskDescription;

        // Unabhängig davon, ob Audio da ist oder nicht, mach mit dem Fade-In weiter
        yield return FadeInTaskUI();
    }



    private IEnumerator PlayTaskOutro(TaskData task)
    {
        taskStatusImage.color = doneColor;

        // Wenn ein Outro-Audio vorhanden ist, spiele es ab und warte, bis es fertig ist
        if (task.outroAudioClip != null)
        {
            audioSource.clip = task.outroAudioClip;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        // Immer HandleTaskLogic aufrufen, egal ob es ein Outro gibt oder nicht
        HandleTaskLogic();
        NextTask();
    }

    private void HandleTaskLogic()
    {
        currentTasks[currentTaskIndex].isTaskCompleted = true;
        // Für Scene 1
        if (currentTasks == sceneTasks_1)
        {
            switch (currentTaskIndex)
            {
                case 0:
                    mirrorWrapper.enabled = true;
                    InfoOverlay.Instance.ShowText("Can Mirror from now");
                    break;
                case 1:
                    debugWrapper.enabled = true;
                    InfoOverlay.Instance.ShowText("Can Debug from now");
                    break;
                case 2:
                    transformerWrapper.enabled = true;
                    InfoOverlay.Instance.ShowText("Can Hit Transformer from now");
                    break;
                case 3:
                    closeTransformerWrapper.enabled = true;
                    InfoOverlay.Instance.ShowText("Can close Transformer from now");
                    break;
                case 4:
                    variantWrapper.enabled = true;
                    InfoOverlay.Instance.ShowText("Can Hit Variant from now");
                    break;
                case 6:
                    bindingsWrapper.enabled = true;
                    InfoOverlay.Instance.ShowText("Can Hit Bindings from now");
                    break;
                case 7:
                    animListWrapper.enabled = true;
                    InfoOverlay.Instance.ShowText("Can Hit Anim from now");
                    break;
                default:
                    InfoOverlay.Instance.ShowText("Unhandled Task for Scene 1");
                    break;
            }
        }
        // Für Scene 2
        else if (currentTasks == sceneTasks_2)
        {
            switch (currentTaskIndex)
            {
                case 0:
                    modelWrapper.enabled = true;
                    InfoOverlay.Instance.ShowText("Can Hit Model from now");
                    break;
                case 2:
                    recorderWrapper.enabled = true;
                    InfoOverlay.Instance.ShowText("Can Rec");
                    break;
                case 3:
                    //canStopRecord = true;
                    //InfoOverlay.Instance.ShowText("Can stop from now");
                    break;
                default:
                    InfoOverlay.Instance.ShowText("Unhandled Task for Scene 2");
                    break;
            }
        }
        // Für Scene 3 (falls es Tasks dafür gibt)
        else if (currentTasks == sceneTasks_3)
        {
            Debug.Log("Task: "+ currentTaskIndex);
            switch (currentTaskIndex)
            {
                case 0:
                    rootMotionWrapper.enabled = true;
                    InfoOverlay.Instance.ShowText("Can Hit Root Motion from now");
                    break;

                default:
                    InfoOverlay.Instance.ShowText("Unhandled Task for Scene 2");
                    break;
            }
        }
    }


    public void CompleteCurrentTask()
    {

        // Setze die Farbe des aktuellen Tasks zurück
        currentTasks[currentTaskIndex].ResetButtonColors();
        StartCoroutine(FadeOutTaskUI(currentTasks[currentTaskIndex]));
    }

    private IEnumerator FadeInTaskUI()
    {
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(true);

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutTaskUI(TaskData task)
    {
        if (taskText == null)
        {
            Debug.LogError("taskText is null. Make sure it is properly assigned.");
            yield break; // Exit the coroutine early to prevent further errors
        }

        if (canvasGroup == null)
        {
            StartCoroutine(PlayTaskOutro(task)); // Starte Outro und HandleTaskLogic auch ohne CanvasGroup
            yield break;
        }

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }

        canvasGroup.gameObject.SetActive(false);
        StartCoroutine(PlayTaskOutro(task)); // Starte Outro nach dem Fade-out
    }


    public void NextTask()
    {
        currentTaskIndex++;

        if (currentTaskIndex >= currentTasks.Count) // NO more Tasks
        {
            // End of the current scene
            Debug.Log("All tasks in the current scene are completed.");
            if (currentTasks == sceneTasks_1)
            {
                scene_1_done = true;
            }
            else if (currentTasks == sceneTasks_2)
            {
                scene_2_done = true;
            }
            else if (currentTasks == sceneTasks_3)
            {
                scene_3_done = true;             
            }
            FinishedSceneCheck();
        }
        else
        {
            taskStatusImage.color = notDoneColor;
            UpdateProgressText();
            StartCoroutine(PlayTaskIntro(currentTasks[currentTaskIndex]));
        }
    }

    public void SwitchToScene(List<TaskData> newSceneTasks)
    {
        currentTasks = newSceneTasks;
        currentTaskIndex = 0;
        taskStatusImage.color = notDoneColor;
        UpdateProgressText();
        StartCoroutine(PlayTaskIntro(currentTasks[currentTaskIndex]));
    }

    private void UpdateProgressText()
    {
        string sceneName = "";
        if (currentTasks == sceneTasks_1) sceneName = "Szene 1";
        else if (currentTasks == sceneTasks_2) sceneName = "Szene 2";
        else if (currentTasks == sceneTasks_3) sceneName = "Szene 3";
        
        //headerText.text = sceneName;
        progressText.text = $"{sceneName}, Aufgabe {currentTaskIndex + 1}/{currentTasks.Count}";
    }

    public void FinishedSceneCheck()
    {
        if (scene_1_done)
        {
           if(!scene_2_done) SetUpScene_2();
            if (scene_2_done)
            {
                if(!scene_3_done) SetUpScene_3();
                if (scene_3_done)
                {
                    Debug.Log("Scene 3 done");
                    StudyFinished();
                }
            }
        }
    }

    private void StudyFinished() // Not done Yet!!!!
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void SetUpScene_1()
    {
        ModelKeypadManager.Instance._TransformerSphereMovement.SetActive(false);
        ModelKeypadManager.Instance._TransformerSphereMovement.GetComponent<ModelTransformer>().ManageRotationSphere(false);
        ModelKeypadManager.Instance.Switch9BtnsActivStatusStudy(false);
        ModelKeypadManager.Instance.Btn_1.SetActive(false);
        ModelKeypadManager.Instance.Btn_8.SetActive(false);
        ModelKeypadManager.Instance.Btn_5.SetActive(false);
        ModelKeypadManager.Instance.Btn_3.SetActive(true);
        setToSpawnPointBtn.SetActive(false);
        SwitchToScene(sceneTasks_1);
    }

    public void SetUpScene_2()
    {
        ModelKeypadManager.Instance._TransformerSphereMovement.SetActive(false);
        ModelKeypadManager.Instance._TransformerSphereMovement.GetComponent<ModelTransformer>().ManageRotationSphere(false);
        AVRGameObjectRecorder.Instance._clipName = "StudyScene_1";
        AVRGameObjectRecorder.Instance.CreateNewClip();
        if (layerBindingsMenu.activeSelf) layerBindingsMenu.SetActive(false);
        if (ModelKeypadManager.Instance._TransformerSphereMovement.activeSelf) ModelKeypadManager.Instance._TransformerSphereMovement.SetActive(false);
        ModelKeypadManager.Instance.Switch9BtnsActivStatusStudy(true);
        ModelKeypadManager.Instance.Btn_1.SetActive(false);
        ModelKeypadManager.Instance.Btn_5.SetActive(false);
        SwitchToScene(sceneTasks_2);
        ModelKeypadManager.Instance._AnimListUiStudy.SetActive(false);
        currentTasks[currentTaskIndex].ChangeButtonColors();
    }

    public void SetUpScene_3()
    {
        ModelKeypadManager.Instance._TransformerSphereMovement.SetActive(false);
        ModelKeypadManager.Instance._TransformerSphereMovement.GetComponent<ModelTransformer>().ManageRotationSphere(false);
        AVRGameObjectRecorder.Instance._clipName = "StudyScene_2";
        AVRGameObjectRecorder.Instance.CreateNewClip();
        if (layerBindingsMenu.activeSelf) layerBindingsMenu.SetActive(false);
        ModelKeypadManager.Instance.Switch9BtnsActivStatusStudy(true);
        SwitchToScene(sceneTasks_3);
        
        Animator targetAnimator = AVRGameObjectRecorder.Instance._objectToRecord.GetComponent<Animator>();
        targetAnimator.applyRootMotion = false;
    }


    #region TasksScene_1

    public void HitAVRMenuTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 0 && currentTasks == sceneTasks_1) CompleteCurrentTask();
    }

    public void HiMirrorTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 1 && currentTasks == sceneTasks_1)
        {
            CompleteCurrentTask();
            ModelKeypadManager.Instance.Btn_2.SetActive(true);
            ModelKeypadManager.Instance.Btn_3.SetActive(true);
        }
    }
    public void HitDebugTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 2 && currentTasks == sceneTasks_1)
        {
            CompleteCurrentTask();
            StartCoroutine(WaitALittle());
           
        }
    }

    IEnumerator WaitALittle()
    {
        yield return new WaitForSeconds(1.0f);
        ModelKeypadManager.Instance.Btn_2.SetActive(true);
        ModelKeypadManager.Instance.Btn_3.SetActive(true);
        ModelKeypadManager.Instance.Btn_7.SetActive(true);
    }

    public void HitTransformerTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying&&currentTaskIndex == 3 && currentTasks == sceneTasks_1) CompleteCurrentTask();
    }

    public void HitAnchorTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (currentTaskIndex == 4 && currentTasks == sceneTasks_1)
        {
            CompleteCurrentTask();
            blueBigBtn.gameObject.SetActive(true);
            ModelKeypadManager.Instance.CallTransformerModel();
            ModelKeypadManager.Instance.Switch9BtnsActivStatusStudy(true);
            ModelKeypadManager.Instance.DeactivateSomeBtns();
            //ModelKeypadManager.Instance.Btn_7.SetActive(true);
            //ModelKeypadManager.Instance.Btn_9.SetActive(true);
            //ModelKeypadManager.Instance.Btn_2.SetActive(true);
            //ModelKeypadManager.Instance.Btn_3.SetActive(true);
        }
    }

    public void HitVariantTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying&&currentTaskIndex == 5 && currentTasks == sceneTasks_1) CompleteCurrentTask();
    }

    public void SwitchVariantTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 6 && currentTasks == sceneTasks_1) CompleteCurrentTask();
    }

    public void HitBindingsTask()
    {
        if (currentTaskIndex <= 7)
        {
            if (currentTasks[currentTaskIndex].isTaskCompleted)
            {
                Debug.Log("Task already completed, ignoring further calls.");
                return;
            }
            if (!audioSource.isPlaying && currentTaskIndex == 7 && currentTasks == sceneTasks_1) CompleteCurrentTask();
        }
    }
    public void PlayAnimWithBindingsTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 8 && currentTasks == sceneTasks_1) CompleteCurrentTask();
    }

    #endregion

    #region TasksScene_2
    public void SwitchModelTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 0 && currentTasks == sceneTasks_2) CompleteCurrentTask();
    }

    public void PlayClapAnimTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 1 && currentTasks == sceneTasks_2) CompleteCurrentTask();
    }
    public void SetFeetsTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 2 && currentTasks == sceneTasks_2)
        {
            CompleteCurrentTask();
        }
    }
    public void HitRecTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 3 && currentTasks == sceneTasks_2)
        {
            CompleteCurrentTask();
        }
    }
    public void StopRecTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 4 && currentTasks == sceneTasks_2)
        {
            CompleteCurrentTask();
        }
    }
    public void PlayStudyScene1Task()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 5 && currentTasks == sceneTasks_2) CompleteCurrentTask();
    }
    #endregion

    #region TasksScene_3
    public void PlayJumpAnimTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 0 && currentTasks == sceneTasks_3) CompleteCurrentTask();
    }

    public void SetRootMotionTask()
    {
        if (currentTaskIndex <= 1)
        {
            if (currentTasks[currentTaskIndex].isTaskCompleted)
            {
                Debug.Log("Task already completed, ignoring further calls.");
                return;
            }
            if (!audioSource.isPlaying && currentTaskIndex == 1 && currentTasks == sceneTasks_3) CompleteCurrentTask();
        }
       
    }
    public void SetArmsTask()
    {
        if (currentTaskIndex <= 3)
        {
            if (currentTasks[currentTaskIndex].isTaskCompleted)
            {
                Debug.Log("Task already completed, ignoring further calls.");
                return;
            }
            if (!audioSource.isPlaying && currentTaskIndex == 2 && currentTasks == sceneTasks_3) CompleteCurrentTask();
        }
    
    }
    public void StopRecSecondTimeTask()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 3 && currentTasks == sceneTasks_3) CompleteCurrentTask();
    }

    public void PlayStudyScene2Task()
    {
        if (currentTasks[currentTaskIndex].isTaskCompleted)
        {
            Debug.Log("Task already completed, ignoring further calls.");
            return;
        }
        if (!audioSource.isPlaying && currentTaskIndex == 4 && currentTasks == sceneTasks_3) CompleteCurrentTask();
    }
    #endregion
}


[System.Serializable]
public class TaskData
{
    public string headerDescription;
    public string taskDescription;
    public AudioClip introAudioClip;
    public AudioClip outroAudioClip;
    public RoundedBoxProperties highliteBtn;
    public Color colorBtn;
    public bool isTaskCompleted = false;

    // Speichert die ursprüngliche Farbe
    public Color originalColor;

    private void InvokePrivateMethod(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(target, null);
        }
        else
        {
            Debug.LogError($"Method {methodName} not found on {target.GetType().Name}");
        }
    }

    // Setzt die Farbe des Buttons auf die gewünschte Farbe für den aktuellen Task
    public void ChangeButtonColors()
    {
        if (highliteBtn == null) return;

        // Setze die neue Farbe
        highliteBtn.Color = new Color(colorBtn.r, colorBtn.g, colorBtn.b, colorBtn.a);
        InvokePrivateMethod(highliteBtn, "UpdateMaterialPropertyBlock");
    }

    // Setzt die Farbe des Buttons auf die ursprüngliche Farbe zurück
    public void ResetButtonColors()
    {
        if (highliteBtn == null) return;
        highliteBtn.Color = originalColor;
        InvokePrivateMethod(highliteBtn, "UpdateMaterialPropertyBlock");
    }
}
