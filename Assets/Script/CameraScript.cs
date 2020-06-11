using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float moving_factor = 100.0f;
    public float rotate_factor = 15.0f;

    void Update()
    {
		if (Input.GetKey(KeyCode.W))
		{
            this.transform.Translate(0.0f, 0.0f, Time.deltaTime * 1.0f * moving_factor, Space.Self);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0.0f, Time.deltaTime * 1.0f * rotate_factor, 0.0f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(0.0f, 0.0f, Time.deltaTime * -1.0f * moving_factor, Space.Self);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0.0f, Time.deltaTime * -1.0f * rotate_factor, 0.0f);
        }
    }
}
