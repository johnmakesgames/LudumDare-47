using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GoToLocationTask : INPCTask
{
    Vector3 positionToGoTo;
    DRSPathfinder dRSPathfinder;
    List<Vector2> generatedPath;
    float timeSinceGeneration = 10;
    float timeSinceNodeSelection = 0;
    int currentNodeInList = 0;

    public GoToLocationTask(string positionToGoToString)
    {
        dRSPathfinder = new DRSPathfinder();
        string[] splitString = positionToGoToString.Split(',');
        positionToGoTo = new Vector3(float.Parse(splitString[0]), float.Parse(splitString[1]), float.Parse(splitString[2]));
        TaskName = $"Going to location: {positionToGoTo}";
    }

    protected override bool CanRun()
    {
        return true;
    }

    protected override void Run()
    {
        if (GameObject.Find("WorldGrid").GetComponent<WorldGrid>() != null)
        {
            dRSPathfinder.SetupPathFinder();

            Vector2 currentPos = new Vector2(this.parentObject.transform.position.x, this.parentObject.transform.position.z);
            Vector2 goalPos = new Vector2(positionToGoTo.x, positionToGoTo.z);

            if (timeSinceGeneration > 10)
            {
                if (dRSPathfinder.FindPath(currentPos, goalPos))
                {
                    generatedPath = dRSPathfinder.pathData;
                    generatedPath.Reverse();
                    timeSinceGeneration = 0;
                    currentNodeInList = 0;
                }
            }
            else
            {
                timeSinceGeneration += Time.deltaTime;
            }

            Vector3 nextLocation = GetNextPositionOnPath(new Vector2(this.parentObject.transform.position.x, this.parentObject.transform.position.z));

            Vector3 toLocation = nextLocation - this.parentObject.transform.position;
            toLocation.Normalize();

            this.parentObject.transform.position += toLocation * 10 * Time.deltaTime;
        }
    }

    Vector3 GetNextPositionOnPath(Vector2 currentPosition)
    {
        timeSinceNodeSelection += Time.deltaTime;

        Vector2 goal = currentPosition;

        if (generatedPath != null && generatedPath.Count > currentNodeInList + 1)
        {
            if (Vector2.Distance(currentPosition, generatedPath[currentNodeInList + 1]) > 0.5)
            {
                goal = generatedPath[currentNodeInList + 1];
            }
            else
            {
                currentNodeInList++;
                goal = GetNextPositionOnPath(currentPosition);
            }
        }

        return new Vector3(goal.x, 0, goal.y);
    }

    protected override bool CheckHasFinished()
    {
        if (Vector3.Distance(this.parentObject.transform.position, positionToGoTo) < 1)
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
