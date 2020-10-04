using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class GoToLocationTask : INPCTask
{
    Vector3 positionToGoTo;
    bool ableToGoToPoint = true;

    public GoToLocationTask(string positionToGoToString)
    {
        string[] splitString = positionToGoToString.Split(',');
        positionToGoTo = new Vector3(float.Parse(splitString[0]), float.Parse(splitString[1]), float.Parse(splitString[2]));
        TaskName = $"Going to location: {positionToGoTo}";
    }

    protected override bool CanRun()
    {
        return ableToGoToPoint;
    }

    protected override void Run()
    {
        NavMeshAgent agent = this.parentObject.GetComponent<NavMeshAgent>();
        agent?.SetDestination(positionToGoTo);
    }

    protected override bool CheckHasFinished()
    {
        if (Vector3.Distance(this.parentObject.transform.position, positionToGoTo) < 0.5f)
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
