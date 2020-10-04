﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCube : MonoBehaviour
{
    public Material CollidingMat;
    public Material NotCollidingMat;
    public List<GameObject> collidingObjects;
    public Vector2 GridPosition;
    public bool GridIndexDirty = false;
    public bool IsColliding;

    private void Start()
    {
        collidingObjects = new List<GameObject>();
        this.GetComponent<MeshRenderer>().material = NotCollidingMat;
    }

    private void Update()
    {
        if (collidingObjects.Count > 0)
        {
            IsColliding = true;
            this.GetComponent<MeshRenderer>().material = CollidingMat;
        }
        else
        {
            IsColliding = false;
            this.GetComponent<MeshRenderer>().material = NotCollidingMat;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "BlockingObject")
        {
            if (!collidingObjects.Contains(other.gameObject))
            {
                collidingObjects.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "BlockingObject")
        {
            if (!collidingObjects.Contains(other.gameObject))
            {
                collidingObjects.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "BlockingObject")
        {
            collidingObjects.Remove(other.gameObject);
        }
    }
}
