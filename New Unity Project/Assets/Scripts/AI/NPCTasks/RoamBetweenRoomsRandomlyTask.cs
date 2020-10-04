using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamBetweenRoomsRandomlyTask : INPCTask
{
    GoToRoomTask task;
    string[] possibleRooms = { "Start, Parlour, Restaurant, Bar, Kitchen, Lobby, Library" };
    public RoamBetweenRoomsRandomlyTask()
    {
        task = new GoToRoomTask(possibleRooms[Random.Range(0, possibleRooms.Length)]);
    }

    public override void RunUpdate()
    {
        task.RunUpdate();
    }
}
