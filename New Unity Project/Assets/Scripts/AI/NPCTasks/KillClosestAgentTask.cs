using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KillClosestAgentTask : INPCTask
{
    public float KillRange;
    private bool hasKilled;

    public override void Reset()
    {
        FinishedState = FinishStates.FAILED;
        IsFinished = false;
        KillRange = 2;
        hasKilled = false;
        MaximumRetries = float.MaxValue;
        TaskName = $"Kill Closest Player";
    }

    public KillClosestAgentTask()
    {
        Reset();
    }

    protected override bool CanRun()
    {
        int visibleToCount = this.parentObject.gameObject.GetComponent<OtherAgentTracker>().HowManyAgentsCanSeeMe();
        if (visibleToCount > 1)
        {
            IsFinished = true;
            FinishedState = FinishStates.FAILED;
            return false;
        }

        return true;
    }

    protected override void Run()
    {
        GameObject agentToKill = this.parentObject.gameObject.GetComponent<OtherAgentTracker>().GetClosestKillableAgent();

        if (Vector3.Distance(this.parentObject.transform.position, agentToKill.transform.position) < KillRange)
        {
            agentToKill.GetComponent<TaskProcessor>().IsDead = true;
            hasKilled = true;
            TaskName = $"Killed: {agentToKill.name}";
            var splat = GameObject.Instantiate(this.parentObject.gameObject.GetComponent<OtherAgentTracker>().Splat);
            splat.transform.position = new Vector3(agentToKill.transform.position.x, -0.49f, agentToKill.transform.position.z);
            agentToKill.SetActive(false);
            this.parentObject.gameObject.GetComponent<OtherAgentTracker>().TimeSinceKill = 0;
            this.parentObject.gameObject.GetComponent<OtherAgentTracker>().SignalKillToUI(agentToKill.name);
        }
        else
        {
            NavMeshAgent agent = this.parentObject.GetComponent<NavMeshAgent>();
            agent?.SetDestination(agentToKill.transform.position);
        }
    }

    protected override bool CheckHasFinished()
    {
        if (hasKilled)
        {
            IsFinished = true;
            FinishedState = FinishStates.SUCCEEDED;
            return true;
        }

        return false;
    }
}
