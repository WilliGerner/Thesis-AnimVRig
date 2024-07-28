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
    private GameObject sceneSetUp_1;
    [SerializeField]
    private GameObject sceneSetUp_2;
    [SerializeField]
    private GameObject sceneSetUp_3;
    [SerializeField]
    private GameObject finish_UI;

    bool tutroial_done,scene_1_done, scene_2_done, scene_3_done;
    public Material greenBtnMaterial;
    Color red = Color.red;
    Color green = Color.green;
    private float alphaTransperenyBtn = 0.3f;

    [SerializeField]
    private GameObject taskTutorial; // Right Canavas with Tasks.
    [SerializeField]
    private GameObject taskUI_1;
    [SerializeField]
    private GameObject taskUI_2;
    [SerializeField]
    private GameObject taskUI_3;

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
    GameObject nextSceneBtn, blueBtn;

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
        TutorialSetUp();
    }

    private void CheckTutTasks()
    {
        if (tutTask_openPalm.isOn && tutTask_enablePassthrough.isOn && tutTask_ModelMover.isOn && tutTask_DebugBones.isOn && tutTask_SwitchModelVariant.isOn && tutTask_OpenAnimListPlayAnim.isOn && tutTask_recAndPlayAnim.isOn)
        {
            nextSceneBtn.GetComponent<MeshRenderer>().material = greenBtnMaterial;
            tutroial_done = true;
        }
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
            case 3:
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

    public void ChangeToNextTask()
    {
        if (taskUI_1.activeSelf)
        {
            taskUI_1.SetActive(false);
            taskUI_2.SetActive(true);
        }else if(taskUI_2.activeSelf)
        {
            taskUI_2.SetActive(false);
            taskUI_3.SetActive(true);
        }else if (taskUI_3.activeSelf)
        {
            Debug.Log("No Next Task!");
        }
    }

    public void TutorialSetUp()
    {
        AVRGameObjectRecorder.Instance._clipName = "Tutorial_ "+ Time.deltaTime;
        TutorialSetUpGO.SetActive(true);
        taskUI_1.SetActive(false);
        taskUI_2.SetActive(false);
        taskUI_3.SetActive(false);
        ChangeButtonColors(1);
    }

    public void SetUpScene_1()
    {
        AVRGameObjectRecorder.Instance._clipName = "StudyScene_1";
        taskUI_2.SetActive(false);
        taskUI_3.SetActive(false);
        ChangeButtonColors(1);
    }

    public void SetUpScene_2()
    {
        AVRGameObjectRecorder.Instance._clipName = "StudyScene_2";
        taskUI_1.SetActive(false);
        taskUI_2.SetActive(true);
        ChangeButtonColors(2);
    }

    public void SetUpScene_3()
    {
        AVRGameObjectRecorder.Instance._clipName = "StudyScene_3";
        taskUI_2.SetActive(false);
        taskUI_3.SetActive(true);
        ChangeButtonColors(3);
    }

    public void ShowFinishScreen()
    {
        sceneSetUp_1.SetActive(false);
        sceneSetUp_2.SetActive(false);
        sceneSetUp_3.SetActive(false);
        finish_UI.SetActive(true);
    }

    public void CheckIfTaskDone()
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
                    SetUpScene_3();
                    if (scene_3_done)
                    {
                        Debug.Log("Scene 3 done");
                        ShowFinishScreen();
                    }
                }
            }
        }
    }

    #region Tutorial Task Functions

    public void HitPalmMenue()
    {
        if (tutroial_done) return;
        tutTask_openPalm.isOn = true;
        CheckTutTasks();
    }

    public void HitPassthrough()
    {
        if (tutroial_done) return;
        tutTask_enablePassthrough.isOn = true;
        CheckTutTasks();
    }

    public void HitDebug()
    {
        if (tutroial_done) return;
        tutTask_DebugBones.isOn = true;
        CheckTutTasks();
    }

    public void HitOtherVariant()
    {
        if (tutroial_done) return;
        tutTask_SwitchModelVariant.isOn = true;
        CheckTutTasks();
    }

    public void HitMoverAndMove()
    {
        if (tutroial_done) return;
        tutTask_ModelMover.isOn = true;
        CheckTutTasks();
    }

    public void HitRecAndPlay()
    {
        if (tutroial_done) return;
        tutTask_recAndPlayAnim.isOn = true;
        CheckTutTasks();
    }
    public void HitAnimWindowAndPlay()
    {
        if (tutroial_done) return;
        tutTask_OpenAnimListPlayAnim.isOn = true;
        CheckTutTasks();
    }
    #endregion
}
