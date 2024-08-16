using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudyScript : MonoBehaviour
{
    [SerializeField]
    private GameObject ExtraInfoUI;

    [SerializeField]
    private TextMeshProUGUI _infoHeadeTxt;
    [SerializeField]
    private TextMeshProUGUI _infoBodyTxt;

    [SerializeField]
    private GameObject TutorialSetUpGO;
    [SerializeField]
    private GameObject taskUI_1;
    [SerializeField]
    private GameObject taskUI_2;
    [SerializeField]
    private GameObject finish_UI;
    [SerializeField]
    private GameObject skinnedMeshRendererStartModel;
    public bool tutroial_done, scene_1_done, scene_2_running, scene_2_done;
    public Material greenBtnMaterial, redBtnMaterial;
    public bool once;

    [SerializeField]
    private GameObject taskTutorial; // Right Canavas with Tasks.

    [SerializeField]
    Toggle tutTask_openPalm, tutTask_ModelMover, tutTask_Mirror, tutTask_DebugBones, tutTask_SwitchModelVariant, tutTask_OpenBindings, tutTask_NothingBind, tutTask_EverythingBinded;

    [SerializeField]
    Toggle TaskScene1_SwitchModel, TaskScene1_BotFeetsActiv, TaskScene1_PlayClapAnim, TaskScene1_RecordClapping, TaskScene1_PlayNewClapAnim;

    [SerializeField]
    Toggle TaskScene2_EnableeRootMotion, TaskScene2_PlayJumpAnim, TaskScene2_SetBindings, TaskScene2_RecordJumpAnim, TaskScene2_PlayYourAnim;

    [SerializeField]
    GameObject layerBindingsMenu;
    [SerializeField]
    GameObject nextSceneBtn;
    [SerializeField]
    GameObject SetToSpawnPointBtn;

    bool onceRootMotionExplain = false;
    bool onceRecordClapInfo = false;

    private static StudyScript instance;
    public static StudyScript Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StudyScript>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<StudyScript>();
                    singletonObject.name = typeof(StudyScript).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }
    private void Start()
    {
        skinnedMeshRendererStartModel.GetComponent<SkinnedMeshRenderer>().enabled = false;
        TutorialSetUp();
        ExtraInfoUI.SetActive(false);
    }

    public void SetExtraUIText(string header, string body)
    {
        _infoHeadeTxt.text = header;
        _infoBodyTxt.text = body;
        ExtraInfoUI.SetActive(true);
    }

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

    public void OnlyOnceSetMeshRenderer()
    {
        if (!once)
        {
            once = true;
            skinnedMeshRendererStartModel.GetComponent<SkinnedMeshRenderer>().enabled = true;
            PokeMirror();
        }
    }

    private void CheckTasks()
    {
        if (!tutroial_done)
        {
            if (tutTask_openPalm.isOn && tutTask_Mirror.isOn && tutTask_ModelMover.isOn && tutTask_DebugBones.isOn && tutTask_SwitchModelVariant.isOn && tutTask_NothingBind.isOn && tutTask_OpenBindings.isOn && tutTask_EverythingBinded.isOn)
            {
                nextSceneBtn.GetComponent<MeshRenderer>().material = greenBtnMaterial;
                tutroial_done = true;
                ModelKeypadManager.Instance.Switch9BtnsActivStatusStudy(false);
                SetExtraUIText("Tutorial abgeschlossen", " Drücke auf den grünen Button um mit der ersten richtigen Aufgabe zu beginnen.");
            }        
        }

        if (tutroial_done && !scene_1_done)
        {
            if (TaskScene1_BotFeetsActiv.isOn && TaskScene1_PlayClapAnim.isOn && TaskScene1_RecordClapping.isOn && TaskScene1_PlayNewClapAnim.isOn && TaskScene1_SwitchModel.isOn)
            {
                nextSceneBtn.GetComponent<MeshRenderer>().material = greenBtnMaterial;
                scene_1_done = true;
                ModelKeypadManager.Instance.Switch9BtnsActivStatusStudy(false);
                SetExtraUIText(" Perfekt erste Szene ist damit abgeschlossen", " Drücke auf den grünen Button um mit der zweiten Szene und deren Aufgaben zu beginnen.");
            }
        }

        if (tutroial_done && scene_1_done && scene_2_running)
        {
            if (TaskScene2_EnableeRootMotion.isOn && TaskScene2_PlayJumpAnim.isOn && TaskScene2_SetBindings.isOn && TaskScene2_RecordJumpAnim.isOn && TaskScene2_PlayYourAnim.isOn)
            {
                nextSceneBtn.GetComponent<MeshRenderer>().material = greenBtnMaterial;
                BigButtonHit();
                scene_2_done = true;
            }
        }

    }

    public void TutorialSetUp()
    {
        ModelKeypadManager.Instance._TransformerSphereMovement.SetActive(false);
        ModelKeypadManager.Instance._TransformerSphereMovement.GetComponent<ModelTransformer>().ManageRotationSphere(false);
        TutorialSetUpGO.SetActive(true);
        taskUI_1.SetActive(false);
        taskUI_2.SetActive(false);
        ModelKeypadManager.Instance.Switch9BtnsActivStatusStudy(false);
        ModelKeypadManager.Instance.Btn_1.SetActive(false);
        ModelKeypadManager.Instance.Btn_8.SetActive(false);
        ModelKeypadManager.Instance.Btn_5.SetActive(false);
        ModelKeypadManager.Instance.Btn_3.SetActive(true);
        SetToSpawnPointBtn.SetActive(false);
    }

    public void SetUpScene_1()
    {
        ModelKeypadManager.Instance._TransformerSphereMovement.SetActive(false);
        ModelKeypadManager.Instance._TransformerSphereMovement.GetComponent<ModelTransformer>().ManageRotationSphere(false);
        TutorialSetUpGO.SetActive(false);
        taskUI_1.SetActive(true);
        nextSceneBtn.GetComponent<MeshRenderer>().material = redBtnMaterial;
        AVRGameObjectRecorder.Instance._clipName = "StudyScene_1";
        AVRGameObjectRecorder.Instance.CreateNewClip();
        if (layerBindingsMenu.activeSelf) layerBindingsMenu.SetActive(false);
        if (ModelKeypadManager.Instance._TransformerSphereMovement.activeSelf) ModelKeypadManager.Instance._TransformerSphereMovement.SetActive(false);
        ModelKeypadManager.Instance.Switch9BtnsActivStatusStudy(true);
        ModelKeypadManager.Instance.Btn_1.SetActive(false);
    }

    public void SetUpScene_2()
    {
        ModelKeypadManager.Instance._TransformerSphereMovement.SetActive(false);
        ModelKeypadManager.Instance._TransformerSphereMovement.GetComponent<ModelTransformer>().ManageRotationSphere(false);
        scene_2_running = true;
     //   nextSceneBtn.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.transform.parent.gameObject.SetActive(false);
        AVRGameObjectRecorder.Instance._clipName = "StudyScene_2";
        AVRGameObjectRecorder.Instance.CreateNewClip();
        taskUI_1.SetActive(false);
        taskUI_2.SetActive(true);
        if (layerBindingsMenu.activeSelf) layerBindingsMenu.SetActive(false);
        ModelKeypadManager.Instance.Switch9BtnsActivStatusStudy(true);
        //ModelKeypadManager.Instance.Btn_1.SetActive(true);
        //ModelKeypadManager.Instance.Btn_5.SetActive(true);
    }

    public void ShowFinishScreen()
    {
        taskUI_1.SetActive(false);
        taskUI_2.SetActive(false);
        finish_UI.SetActive(true);
    }

    public void BigButtonHit()
    {
        if (tutroial_done)
        {
            Debug.Log("Tutorial done");
            SetUpScene_1();
            if (scene_1_done)
            {
                Debug.Log("Scene 1 done");
                SetUpScene_2();
                if (scene_2_done)
                {
                    Debug.Log("Scene 2 done");
                    ShowFinishScreen();
                }
            }
        }
    }

    #region Tutorial Task Functions
    public void HitPalmMenue()
    {
        if (tutroial_done) return;
        tutTask_openPalm.isOn = true;
        CheckTasks();
    }

    public void HitDebug()
    {
        if (tutroial_done) return;
        tutTask_DebugBones.isOn = true;
        ModelKeypadManager.Instance.Btn_9.SetActive(true);
        CheckTasks();
    }

    void PokeMirror()
    {
        if (tutroial_done) return;
        tutTask_Mirror.isOn = true;
        ModelKeypadManager.Instance.Btn_7.SetActive(true);
        CheckTasks();
    }

    public void HitOtherVariant()
    {
        if (tutroial_done) return;
        tutTask_SwitchModelVariant.isOn = true;
        ModelKeypadManager.Instance.Btn_4.SetActive(true);
        ModelKeypadManager.Instance.Btn_6.SetActive(true);
        CheckTasks();
    }

    public void HitMoverAndMove()
    {
        if (tutroial_done) return;
        tutTask_ModelMover.isOn = true;
        ModelKeypadManager.Instance.Btn_2.SetActive(true);
        SetToSpawnPointBtn.SetActive(true);
        CheckTasks();
    }

    public void OpenBindingsMenueTask()
    {
        if (tutroial_done) return;
        tutTask_OpenBindings.isOn = true;
        CheckTasks();
        SetExtraUIText("Bindings verstehen", " Aktiv (grün): Animationen richten sich nach dem Körperteil. Inaktiv (rot): Animationen richten sich nicht nach dem Körperteil.");
    } 

    public void BindEverythingTask( )
    {
        if (tutroial_done) return;
        tutTask_EverythingBinded.isOn = true;
    
        CheckTasks();
    }

    public void BindNothingTask()
    {
        if (tutroial_done) return;
        tutTask_NothingBind.isOn = true;
       
        CheckTasks();
    }
    #endregion

    #region Scene 1 Task Functions
    public void SwitchModelTask()
    {
        if (!tutroial_done||scene_1_done) return;
        TaskScene1_SwitchModel.isOn = true;
        CheckTasks();
    }



    public void BothFeetSetActiv(bool activ)
    {
        if (!tutroial_done || scene_1_done) return;
        if (activ) TaskScene1_BotFeetsActiv.isOn = true;
        //else TaskScene1_BotFeetsActiv.isOn = false;
        CheckTasks();
    }

    public void PlayClapAnimTask()
    {
        if (!tutroial_done || scene_1_done) return;
        TaskScene1_PlayClapAnim.isOn = true;
        CheckTasks();
    }

    public void RecordClapTask()
    {
        if (!tutroial_done || scene_1_done) return;
        TaskScene1_RecordClapping.isOn = true;
        CheckTasks();
        if (onceRecordClapInfo)
        {
            onceRecordClapInfo = false;
            SetExtraUIText("Klatschen erfolgreich aufgenommen", " Um deine neue Animation abzuspielen musst du zuerst deine Bindings alle entfernen (Klick auf Body), ansonsten richtet sich die neue Animation beim abspielen nach den Bindings und es sieht komisch aus.");
        }
    }
    public void PlayNewClapAnim()
    {
        if (tutroial_done && !scene_1_done)
        {
            TaskScene1_PlayNewClapAnim.isOn = true;
            CheckTasks();
           // SetExtraUIText("Springen erfolgreich aufgenommen", " Um deine neue Animation abzuspielen musst du zuerst deine Bindings alle entfernen (Klick auf Body), ansonsten richtet sich die neue Animation beim abspielen nach den Bindings und es sieht komisch aus.");
        }
    
    }
    #endregion

    #region Scene 2 Tasks Functions
    public void SetRootMotion(bool activ)
    {
        if (!tutroial_done && !scene_1_done) return;
        if(activ) TaskScene2_EnableeRootMotion.isOn = true;
        else TaskScene2_EnableeRootMotion.isOn =false;
        CheckTasks();
       if (onceRootMotionExplain)
        {
            SetExtraUIText("Root Motion, was ist das?", " Wenn Root Motion bei einer Sprunganimation aktiviert ist, wird die Bewegung des Charakters durch die Animation selbst gesteuert, sodass der Charakter hochspringt. Wenn Root Motion deaktiviert ist, bleibt der Charakter an seiner Position und die Animation zeigt nur die Sprungbewegung ohne tatsächliche Änderung der Position im Raum.");
            onceRootMotionExplain = false;
        }
    }

    public void PlayJumpAnim()
    {
        if (!tutroial_done && !scene_1_done) return;
        TaskScene2_PlayJumpAnim.isOn = true;
        CheckTasks();
    }

    public void SetBindingsForJumpTask(bool activ)
    {
        if (!tutroial_done && !scene_1_done) return;
        if (activ)
        {
            TaskScene2_SetBindings.isOn = true;
        }
        //else TaskScene2_SetBindings.isOn = false;

        CheckTasks();
    }

    public void RecordNewJumpAnim()
    {
        if (tutroial_done && scene_1_done && scene_2_running)
        {
            TaskScene2_RecordJumpAnim.isOn = true;
            CheckTasks();
        } 
    }

    public void PlayYourNewJumpAnim()
    {
        if (!tutroial_done && !scene_1_done) return;
        TaskScene2_PlayYourAnim.isOn = true;
        CheckTasks();
    }

    #endregion
}
