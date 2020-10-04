using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GoToRoomTask : INPCTask
{
    GameObject roomToGoTo;

    public GoToRoomTask(string RoomToGoTo)
    {
        var rooms = GameObject.FindGameObjectsWithTag("RoomMarkers");
        roomToGoTo = rooms.Where(x => x.name == RoomToGoTo).FirstOrDefault();
        TaskName = $"Going to room: {RoomToGoTo}";
    }

    protected override bool CanRun()
    {
        return (roomToGoTo != null);
    }

    protected override void Run()
    {
        NavMeshAgent agent = this.parentObject.GetComponent<NavMeshAgent>();
        agent?.SetDestination(roomToGoTo.transform.position);
    }

    protected override bool CheckHasFinished()
    {
        if (Vector3.Distance(this.parentObject.transform.position, roomToGoTo.transform.position) < 3)
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
