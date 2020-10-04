using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaitForDurationTask : INPCTask
{
    float durationToWaitFor;
    float waitingDuration = 0;

    public WaitForDurationTask(string positionToGoToString)
    {
        durationToWaitFor = float.Parse(positionToGoToString);
        TaskName = $"Wait for duration: {durationToWaitFor}";
    }

    protected override bool CanRun()
    {
        return true;
    }

    protected override void Run()
    {
        if (waitingDuration < durationToWaitFor)
        {
            waitingDuration += Time.deltaTime;
        }
    }

    protected override bool CheckHasFinished()
    {
        if (waitingDuration > durationToWaitFor)
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
