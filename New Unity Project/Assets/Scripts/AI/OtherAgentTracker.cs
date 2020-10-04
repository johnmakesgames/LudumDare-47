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

    void Update()
    {
        if (TimeSinceKill < 5)
        {
            float bloodDropChance = Random.Range(0, TimeSinceKill);
            if (bloodDropChance < 1)
            {
                GameObject drop = Instantiate(Drip);
                drop.transform.position = new Vector3(this.transform.position.x, 0.01f, this.transform.position.z);
                drop.transform.rotation = this.transform.rotation;
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
