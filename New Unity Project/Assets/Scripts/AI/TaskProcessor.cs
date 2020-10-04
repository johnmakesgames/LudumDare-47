using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    RoamPassively,
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

                if (CurrentTask.FinishedState != INPCTask.FinishStates.FAILED)
                    CompletedTasks.Add(CurrentTask.TaskName);

                if (QueuedTasks.Count > 0)
                {
                    CurrentTask = QueuedTasks.Dequeue();
                    CurrentTask.Reset();
                }
                else
                {
                    CurrentTask = NPCTaskFactory.CreateTaskFromTypeAndParameters(TaskTypes.RoamPassively, "");
                }
            }

            if (CurrentTask != null)
            {
                CurrentTask.RunUpdate();
            }

            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z);
        }
    }
}
