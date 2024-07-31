using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class StudyScript : MonoBehaviour
{
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
    bool tutroial_done,scene_1_done, scene_2_done;
    public Material greenBtnMaterial, redBtnMaterial;
    Color red = Color.red;
    Color green = Color.green;
    private float alphaTransperenyBtn = 0.3f;
    bool once;

    [SerializeField]
    private GameObject taskTutorial; // Right Canavas with Tasks.


    public RoundedBoxProperties btn_1;
    public RoundedBoxProperties btn_2;
    public RoundedBoxProperties btn_3;
    public RoundedBoxProperties btn_4;
    public RoundedBoxProperties btn_5;
    public RoundedBoxProperties btn_6;
    public RoundedBoxProperties btn_7;
    public RoundedBoxProperties btn_8;
    public RoundedBoxProperties btn_9;

    [SerializeField]
    Toggle tutTask_openPalm, tutTask_enablePassthrough, tutTask_ModelMover, tutTask_DebugBones, tutTask_SwitchModelVariant, tutTask_OpenAnimListPlayAnim, tutTask_recAndPlayAnim;

    [SerializeField]
    Toggle TaskScene1_SwitchModel, TaskScene1_OpenBindings, TaskScene1_BotFeetsActiv, TaskScene1_PlayClapAnim, TaskScene1_RecordClapping, TaskScene1_PlayNewClapAnim;

    [SerializeField]
    Toggle TaskScene2_EnableeRootMotion, TaskScene2_PlayJumpAnim, TaskScene2_SetBindings, TaskScene2_RecordJumpAnim, TaskScene2_PlayYourAnim;


    [SerializeField]
    GameObject nextSceneBtn;

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
    }


    private void ChangeButtonColors(int sceneSetUp) // 1,2 or 3
    {
        switch (sceneSetUp)
        {
            case 1:
                btn_1.Color = new Color(green.r, green.g, green.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_1, "UpdateMaterialPropertyBlock");
                btn_4.Color = new Color(red.r, red.g, red.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_4, "UpdateMaterialPropertyBlock");
                btn_5.Color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_5, "UpdateMaterialPropertyBlock");
                btn_8.Color = new Color(red.r, red.g, red.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_8, "UpdateMaterialPropertyBlock");
                btn_2.Color = new Color(green.r, green.g, green.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_2, "UpdateMaterialPropertyBlock");
                btn_3.Color = new Color(green.r, green.g, green.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_3, "UpdateMaterialPropertyBlock");
                btn_6.Color = new Color(green.r, green.g, green.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_6, "UpdateMaterialPropertyBlock");
                btn_7.Color = new Color(green.r, green.g, green.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_7, "UpdateMaterialPropertyBlock");
                btn_9.Color = new Color(green.r, green.g, green.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_9, "UpdateMaterialPropertyBlock");
                break;
            case 2:
                btn_1.Color = new Color(green.r, green.g, green.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_1, "UpdateMaterialPropertyBlock");
                btn_2.Color = new Color(red.r, red.g, red.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_2, "UpdateMaterialPropertyBlock");
                btn_3.Color = new Color(red.r, red.g, red.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_3, "UpdateMaterialPropertyBlock");
                btn_4.Color = new Color(green.r, green.g, green.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_4, "UpdateMaterialPropertyBlock");
                btn_5.Color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_5, "UpdateMaterialPropertyBlock");
                btn_6.Color = new Color(green.r, green.g, green.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_6, "UpdateMaterialPropertyBlock");
                btn_7.Color = new Color(green.r, green.g, green.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_7, "UpdateMaterialPropertyBlock");
                btn_8.Color = new Color(green.r, green.g, green.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_8, "UpdateMaterialPropertyBlock");
                btn_9.Color = new Color(red.r, red.g, red.b, alphaTransperenyBtn);
                InvokePrivateMethod(btn_9, "UpdateMaterialPropertyBlock");
                break;
            default:
                break;
        }     
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
        }
    }

    private void CheckTasks()
    {
        if (!tutroial_done)
        {
            if (tutTask_openPalm.isOn && tutTask_enablePassthrough.isOn && tutTask_ModelMover.isOn && tutTask_DebugBones.isOn && tutTask_SwitchModelVariant.isOn && tutTask_OpenAnimListPlayAnim.isOn && tutTask_recAndPlayAnim.isOn)
            {
                nextSceneBtn.GetComponent<MeshRenderer>().material = greenBtnMaterial;
                tutroial_done = true;
            }
        }

        if(tutroial_done && !scene_1_done)
        {
            if (TaskScene1_BotFeetsActiv.isOn && TaskScene1_OpenBindings.isOn &&  TaskScene1_PlayClapAnim.isOn && TaskScene1_RecordClapping.isOn && TaskScene1_PlayNewClapAnim.isOn && TaskScene1_SwitchModel.isOn)
            {
                nextSceneBtn.GetComponent<MeshRenderer>().material = greenBtnMaterial;
                scene_1_done = true;
            }
        }

        if (tutroial_done && scene_1_done && !scene_2_done)
        {
            if (TaskScene2_EnableeRootMotion.isOn && TaskScene2_PlayJumpAnim.isOn && TaskScene2_SetBindings.isOn && TaskScene2_RecordJumpAnim.isOn && TaskScene2_PlayYourAnim.isOn)
            {
                nextSceneBtn.GetComponent<MeshRenderer>().material = greenBtnMaterial;
                scene_2_done = true;
            }
        }

    }

    public void TutorialSetUp()
    {
        AVRGameObjectRecorder.Instance._clipName = "Tutorial_ "+ Time.deltaTime;
        TutorialSetUpGO.SetActive(true);
        taskUI_1.SetActive(false);
        taskUI_2.SetActive(false);
        ChangeButtonColors(1);
    }

    public void SetUpScene_1()
    {
        TutorialSetUpGO.SetActive(false);
        taskUI_1.SetActive(true);
        nextSceneBtn.GetComponent<MeshRenderer>().material = redBtnMaterial;
        AVRGameObjectRecorder.Instance._clipName = "StudyScene_1";
        ChangeButtonColors(2);
    }

    public void SetUpScene_2()
    {
        
        nextSceneBtn.GetComponent<MeshRenderer>().material = redBtnMaterial;
        AVRGameObjectRecorder.Instance._clipName = "StudyScene_2";
        taskUI_1.SetActive(false);
        taskUI_2.SetActive(true);
        ChangeButtonColors(2);
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

    public void HitPassthrough()
    {
        if (tutroial_done) return;
        tutTask_enablePassthrough.isOn = true;
        CheckTasks();
    }

    public void HitDebug()
    {
        if (tutroial_done) return;
        tutTask_DebugBones.isOn = true;
        CheckTasks();
    }

    public void HitOtherVariant()
    {
        if (tutroial_done) return;
        tutTask_SwitchModelVariant.isOn = true;
        CheckTasks();
    }

    public void HitMoverAndMove()
    {
        if (tutroial_done) return;
        tutTask_ModelMover.isOn = true;
        CheckTasks();
    }

    public void HitRecAndStop()
    {
        if (tutroial_done) return;
        tutTask_recAndPlayAnim.isOn = true;
        CheckTasks();
    }
    public void HitAnimWindowAndPlay()
    {
        if (tutroial_done) return;
        tutTask_OpenAnimListPlayAnim.isOn = true;
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

    public void OpenBindingsMenueTask()
    {
        if (!tutroial_done || scene_1_done) return;
        TaskScene1_OpenBindings.isOn = true;
        CheckTasks();
    }

    public void BothFeetSetActiv(bool activ)
    {
        if (!tutroial_done || scene_1_done) return;
        if (activ) TaskScene1_BotFeetsActiv.isOn = true;
        else TaskScene1_BotFeetsActiv.isOn = false;
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
    }
    public void PlayNewClapAnim()
    {
        if (!tutroial_done || scene_1_done) return;
        TaskScene1_PlayNewClapAnim.isOn = true;
        CheckTasks();
    }
    #endregion

    #region Scene 2 Tasks Functions
    public void SetRootMotion(bool activ)
    {
        if (!tutroial_done || !scene_1_done) return;
        if(activ) TaskScene2_EnableeRootMotion.isOn = true;
        else TaskScene2_EnableeRootMotion.isOn =false;
        CheckTasks();
    }

    public void PlayJumpAnim()
    {
        if (!tutroial_done || !scene_1_done) return;
        TaskScene2_PlayJumpAnim.isOn = true;
        CheckTasks();
    }

    public void SetBindingsForJumpTask(bool activ)
    {
        if (!tutroial_done || !scene_1_done) return;
        if (activ)
        {
            TaskScene2_SetBindings.isOn = true;
        }
        else TaskScene2_SetBindings.isOn = false;

        CheckTasks();
    }

    public void RecordNewJumpAnim()
    {
        if (!tutroial_done || !scene_1_done) return;
        TaskScene2_RecordJumpAnim.isOn = true;
        CheckTasks();
    }

    public void PlayYourNewJumpAnim()
    {
        if (!tutroial_done || !scene_1_done) return;
        TaskScene2_PlayYourAnim.isOn = true;
        CheckTasks();
    }

    #endregion
}
