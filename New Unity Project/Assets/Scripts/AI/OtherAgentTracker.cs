using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class OtherAgentTracker : MonoBehaviour
{
    public GameObject Splat;
    public GameObject Drip;
    List<GameObject> OtherAgents;
    public float TimeSinceKill = 100;

    // Start is called before the first frame update
    void Start()
    {
        OtherAgents = GameObject.FindGameObjectsWithTag("Character").Where(x => x != this.gameObject).ToList();
    }

    void FixedUpdate()
    {
        if (TimeSinceKill < 2)
        {
            float bloodDropChance = Random.Range(0, TimeSinceKill);
            bloodDropChance = Mathf.Round(bloodDropChance);
            Debug.Log($"Time: {TimeSinceKill}  |  Rand: {bloodDropChance}");
            if (bloodDropChance < 1.5f)
            {
                GameObject drop = Instantiate(Drip);
                drop.transform.position = new Vector3(this.transform.position.x, -0.49f, this.transform.position.z);
            }
        }

        TimeSinceKill += Time.deltaTime;
    }

    public int HowManyAgentsCanSeeMe()
    {
        int counter = 0;

        foreach (var agent in OtherAgents)
        {
            if (Physics.Linecast(this.transform.position, agent.transform.position, out RaycastHit hit))
            {
                if (hit.transform.name == agent.name)
                {
                    counter++;
                }
            }
        }

        return counter;
    }

    public void SignalKillToUI(string nameOfVictim)
    {
        GameObject.Find("UIController").GetComponent<UIHandler>().SignalDeath(nameOfVictim);
    }

    public GameObject GetClosestKillableAgent()
    {
        GameObject closestObject = null;
        float closestDistance = float.MaxValue;

        foreach(var agent in OtherAgents)
        {
            if (agent.gameObject.name != "Player" && !agent.GetComponent<TaskProcessor>().IsDead)
            {
                if (Vector3.Distance(this.transform.position, agent.transform.position) < closestDistance)
                {
                    closestDistance = Vector3.Distance(this.transform.position, agent.transform.position);
                    closestObject = agent;
                }
            }
        }

        return closestObject;
    }
}
