using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTaskFactory
{
    public static INPCTask CreateTaskFromTypeAndParameters(string taskType, string parameter)
    {
        if (taskType == "GoToObject")
        {
            return new GoToObjectTask(parameter);
        }
        else if (taskType == "GoToPosition")
        {
            return new GoToLocationTask(parameter);
        }
        else if (taskType == "WaitForDuration")
        {
            return new WaitForDurationTask(parameter);
        }
        else
        {
            return null;
        }
    }
}
