using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public GameObject taskPanel;
    public CanvasGroup taskCanvasGroup;
    public Text taskText;
    public Image taskImageToShow;
    public int currentTask = 0;
    public TaskInfo[] tasks;
    public Button taskCloseButton;
    public Button taskOpenButton;
    public GameObject taskBlockPanel;

    public UnityEvent onLevelComplete;

    private void OnEnable()
    {
        taskCloseButton.onClick.AddListener(CloseTask);
        taskOpenButton.onClick.AddListener(ShowTask);
    }

    private void OnDisable()
    {
        taskCloseButton.onClick.RemoveListener(CloseTask);
        taskOpenButton.onClick.RemoveListener(ShowTask);
    }

    private void Start()
    {
        TaskInfo currentTaskInfo = tasks[currentTask];

        if (currentTaskInfo == null)
            print("There is no task to show");

        taskImageToShow.sprite = currentTaskInfo?.taskImage;
        taskText.text = currentTaskInfo?.taskName;

        SaveDemo();
    }

    public void SaveDemo()
    {
        SaveData newSaveData = new SaveData();
        newSaveData.currentLevel = 1;
        newSaveData.playerName = "RV";

        SaveManager.SaveData(newSaveData);
        print(Application.persistentDataPath);
    }


    [ContextMenu("Open Task")]
    public void ShowTask()
    {
        TaskInfo currentTaskInfo = tasks[currentTask];

        if (currentTaskInfo == null)
            print("There is no task to show");

        taskBlockPanel.SetActive(true);

        //Set values
        taskPanel.SetActive(true);
        taskCanvasGroup.alpha = 0;
        taskImageToShow.sprite = currentTaskInfo?.taskImage;
        taskText.text = currentTaskInfo?.taskName;

        //Show time
        taskCanvasGroup.DOFade(1, 1.25f).OnComplete(() => EscapeRoomManager.instance.blockPanel.SetActive(false));
    }

    public void CloseTask()
    {
        if (currentTask == tasks.Length - 1)
        {
            TaskInfo currentTaskInfo = tasks[currentTask];

            if (currentTaskInfo == null)
                print("There is no task to show");
        }

        EscapeRoomManager.instance.blockPanel.SetActive(true);

        //Show time
        taskCanvasGroup.DOFade(0, 0.75f).OnComplete(() =>
        {
            EscapeRoomManager.instance.blockPanel.SetActive(false);
            taskBlockPanel.SetActive(false);
            taskPanel.SetActive(false);
        });
    }

    public void ShowTaskComplete()
    {
        StartCoroutine(ShowTaskCompleteRoutine());
    }

    private IEnumerator ShowTaskCompleteRoutine()
    {
        TaskInfo currentTaskInfo = tasks[currentTask];
        currentTaskInfo.isTaskComplete = true;

        taskPanel.SetActive(true);
        taskCanvasGroup.alpha = 0;
        taskCanvasGroup.DOFade(1, 0.45f);
        yield return new WaitForSeconds(0.75f);

        taskText.text = "Task Completed";
        ++currentTask;

        if (currentTask >= tasks.Length)
        {
            yield return new WaitForSeconds(0.75f);
            taskBlockPanel.SetActive(false);
            taskPanel.SetActive(false);
            LevelComplete();
            yield break;
        }

        yield return new WaitForSeconds(0.75f);
        taskImageToShow.sprite = tasks[currentTask].taskImage;
        taskText.text = tasks[currentTask].taskName;
    }

    private void LevelComplete()
    {
        onLevelComplete?.Invoke();
    }
}


[System.Serializable]
public class TaskInfo
{
    public string taskName;
    public Sprite taskImage;
    public bool isTaskComplete;
}