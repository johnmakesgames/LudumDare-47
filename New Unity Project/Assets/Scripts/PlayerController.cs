using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MovementSpeed;
    public float RotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            move += this.transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move -= this.transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Rotate(new Vector3(0, -RotationSpeed, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Rotate(new Vector3(0, RotationSpeed, 0));
        }

        if (!Physics.Linecast(this.transform.position, this.transform.position + (move * MovementSpeed * Time.deltaTime) + new Vector3(0.5f, 0.5f, 0.5f)))
        {
            if (!Physics.Linecast(this.transform.position, this.transform.position + (move * MovementSpeed * Time.deltaTime) - new Vector3(0.5f, 0.5f, 0.5f)))
            {
                this.transform.position += move * MovementSpeed * Time.deltaTime;
            }
        }
    }
}
