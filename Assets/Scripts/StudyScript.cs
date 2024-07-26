using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StudyScript : MonoBehaviour
{
    [SerializeField]
    private GameObject sceneSetUp_1;
    [SerializeField]
    private GameObject sceneSetUp_2;
    [SerializeField]
    private GameObject sceneSetUp_3;

    bool scene_1_done, scene_2_done, scene_3_done;

    Color red = Color.red;
    Color green = Color.green;
    private float alphaTransperenyBtn = 0.3f;

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

    private void Start()
    {
        SetUpScene_2();

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
}
