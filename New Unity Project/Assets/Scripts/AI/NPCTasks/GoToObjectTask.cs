using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToObjectTask : INPCTask
{
    GameObject objectToGoTo;
    public GoToObjectTask(string ObjectToGoToName)
    {
        objectToGoTo = GameObject.Find(ObjectToGoToName);
        TaskName = $"Going to objective: {ObjectToGoToName}";
    }

    protected override bool CanRun()
    {
        return (objectToGoTo != null);
    }

    protected override void Run()
    {
        NavMeshAgent agent = this.parentObject.GetComponent<NavMeshAgent>();
        agent?.SetDestination(objectToGoTo.transform.position);
    }

    protected override bool CheckHasFinished()
    {
        if (Vector3.Distance(this.parentObject.transform.position, objectToGoTo.transform.position) < 1)
        {
            FinishedState = FinishStates.SUCCEEDED;
            return true;
        }
        else
        {
            return false;
        }
    }
}
