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
    private TextMeshProUGUI taskText;

    [SerializeField]
    private Image taskStatusImage;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private Color notDoneColor = Color.red;
    [SerializeField]
    private Color doneColor = Color.green;

    [SerializeField]
    private List<TaskData> scene1Tasks;
    [SerializeField]
    private List<TaskData> scene2Tasks;
    [SerializeField]
    private List<TaskData> scene3Tasks;

    private List<TaskData> currentTasks;
    private int currentTaskIndex = 0;

    private void Start()
    {
        currentTasks = scene1Tasks;
        taskStatusImage.color = notDoneColor;
        StartCoroutine(PlayTaskIntro(currentTasks[currentTaskIndex]));
    }

    private IEnumerator PlayTaskIntro(TaskData task)
    {
        taskText.text = task.taskDescription;
        task.ChangeButtonColors();
        if (task.introAudioClip != null)
        {
            audioSource.clip = task.introAudioClip;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        StartCoroutine(FadeInTaskUI());
    }

    private IEnumerator PlayTaskOutro(TaskData task)
    {
        if (task.outroAudioClip != null)
        {
            audioSource.clip = task.outroAudioClip;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        taskStatusImage.color = doneColor;
        yield return new WaitForSeconds(1f); // A small delay before proceeding to the next task

        NextTask();
    }

    public void CompleteCurrentTask()
    {
        StartCoroutine(FadeOutTaskUI(currentTasks[currentTaskIndex]));
    }

    private IEnumerator FadeInTaskUI()
    {
        CanvasGroup canvasGroup = taskText.GetComponentInParent<CanvasGroup>();
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
        CanvasGroup canvasGroup = taskText.GetComponentInParent<CanvasGroup>();

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }

        canvasGroup.gameObject.SetActive(false);
        StartCoroutine(PlayTaskOutro(task));
    }

    private void NextTask()
    {
        currentTaskIndex++;
        
        if (currentTaskIndex >= currentTasks.Count)
        {
            // End of the current scene
            // Manually trigger next scene or other logic
            Debug.Log("All tasks in the current scene are completed.");
        }
        else
        {
            taskStatusImage.color = notDoneColor;
            StartCoroutine(PlayTaskIntro(currentTasks[currentTaskIndex]));
        }
    }

    public void SwitchToScene(List<TaskData> newSceneTasks)
    {
        currentTasks = newSceneTasks;
        currentTaskIndex = 0;
        taskStatusImage.color = notDoneColor;
        StartCoroutine(PlayTaskIntro(currentTasks[currentTaskIndex]));
    }



}

[System.Serializable]
public class TaskData
{
    public string taskDescription;
    public AudioClip introAudioClip;
    public AudioClip outroAudioClip;
    public RoundedBoxProperties highliteBtn;
    public float alphaTransperenyBtn;
    //public RoundedBoxProperties btn_2;
    //public RoundedBoxProperties btn_3;
    //public RoundedBoxProperties btn_4;
    //public RoundedBoxProperties btn_5;
    //public RoundedBoxProperties btn_6;
    //public RoundedBoxProperties btn_7;
    //public RoundedBoxProperties btn_8;
    //public RoundedBoxProperties btn_9;
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

    public void ChangeButtonColors() // 1,2 or 3
    {
                highliteBtn.Color = new Color(Color.black.r, Color.black.g, Color.black.b, alphaTransperenyBtn);
                InvokePrivateMethod(highliteBtn, "UpdateMaterialPropertyBlock");       
    }
}


///



///
