using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class INPCTask
{
    public string TaskName;
    public GameObject parentObject;

    public enum FinishStates { SUCCEEDED, FAILED }

    public bool IsFinished = false;
    public FinishStates FinishedState = FinishStates.FAILED;
    public float Retried = 0;

    public void RunUpdate()
    {
        if (!IsFinished)
        {
            if (CanRun())
            {
                Run();
                IsFinished = CheckHasFinished();
            }
        }
    }

    protected virtual bool CanRun()
    {
        return true;
    }

    protected virtual void Run()
    {
        // Override this
    }

    protected virtual bool CheckHasFinished()
    {
        return false;
    }
}
