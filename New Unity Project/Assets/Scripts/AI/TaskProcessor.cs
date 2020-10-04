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
    WaitForDuration,
    GoToRoom,
    KillClosestAgent,
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
    public List<string> CompletedTasks;
    public bool IsDead = false;

    // Start is called before the first frame update
    void Start()
    {
        QueuedTasks = new Queue<INPCTask>();

        foreach(NPCTaskMap mapItem in TasksWithParameters)
        {
            INPCTask newTask = NPCTaskFactory.CreateTaskFromTypeAndParameters(mapItem.TaskType, mapItem.TaskParameter);
            newTask.parentObject = this.gameObject;
            QueuedTasks.Enqueue(newTask);
        }

        CurrentTask = QueuedTasks.Dequeue();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDead)
        {
            return;
        }

        if (CurrentTask != null)
        {
            if (CurrentTask.IsFinished)
            {
                if (CurrentTask.FinishedState == INPCTask.FinishStates.FAILED)
                {
                    CurrentTask.Retried++;
                    if (!(CurrentTask.Retried >= CurrentTask.MaximumRetries))
                    {
                        QueuedTasks.Enqueue(CurrentTask);
                    }
                }

                if (QueuedTasks.Count > 0)
                {
                    if (CurrentTask.FinishedState != INPCTask.FinishStates.FAILED)
                        CompletedTasks.Add(CurrentTask.TaskName);

                    CurrentTask = QueuedTasks.Dequeue();
                    CurrentTask.Reset();
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
