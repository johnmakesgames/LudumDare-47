using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTaskFactory
{
    public static INPCTask CreateTaskFromTypeAndParameters(TaskTypes taskType, string parameter)
    {
        switch (taskType)
        {
            case TaskTypes.GoToObject:
                return new GoToObjectTask(parameter);
            case TaskTypes.GoToPosition:
                return new GoToLocationTask(parameter);
            case TaskTypes.WaitForDuration:
                return new WaitForDurationTask(parameter);
            case TaskTypes.GoToRoom:
                return new GoToRoomTask(parameter);
            case TaskTypes.KillClosestAgent:
                return new KillClosestAgentTask();
            case TaskTypes.RoamPassively:
                return new RoamBetweenRoomsRandomlyTask();
            default:
                return null;
        }
    }
}
