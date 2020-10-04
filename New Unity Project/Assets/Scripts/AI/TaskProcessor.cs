using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public enum TaskTypes
{
    GoToObject,
    GoToPosition,
    WaitForDuration
}


[Serializable]
public struct NPCTaskMap
{
    public TaskTypes TaskType;
    public string TaskParameter;
}

public class TaskProcessor : MonoBehaviour
{
    public List<NPCTaskMap> TasksWithParameters;
    public Queue<INPCTask> QueuedTasks;
    public INPCTask CurrentTask;
    public float MaximumRetries;

    // Start is called before the first frame update
    void Start()
    {
        QueuedTasks = new Queue<INPCTask>();

        foreach(NPCTaskMap mapItem in TasksWithParameters)
        {
            INPCTask newTask = NPCTaskFactory.CreateTaskFromTypeAndParameters(mapItem.TaskType.ToString(), mapItem.TaskParameter);
            newTask.parentObject = this.gameObject;
            QueuedTasks.Enqueue(newTask);
        }

        CurrentTask = QueuedTasks.Dequeue();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentTask != null)
        {
            if (CurrentTask.IsFinished)
            {
                if (CurrentTask.FinishedState == INPCTask.FinishStates.FAILED)
                {
                    CurrentTask.Retried++;
                    if (!(CurrentTask.Retried >= MaximumRetries))
                    {
                        QueuedTasks.Enqueue(CurrentTask);
                    }
                }

                if (QueuedTasks.Count > 0)
                {
                    CurrentTask = QueuedTasks.Dequeue();
                }
                else
                {
                    CurrentTask = null;
                }
            }

            if (CurrentTask != null)
            {
                CurrentTask.RunUpdate();
            }
        }

    }
}
