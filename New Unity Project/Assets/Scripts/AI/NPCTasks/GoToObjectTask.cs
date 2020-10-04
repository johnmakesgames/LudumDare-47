using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        return true;
    }

    protected override void Run()
    {
        Vector3 toLocation = objectToGoTo.transform.position - this.parentObject.transform.position;
        toLocation.Normalize();

        this.parentObject.transform.position += toLocation * 10 * Time.deltaTime;
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
