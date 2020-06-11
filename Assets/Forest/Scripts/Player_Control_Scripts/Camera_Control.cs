using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Control : MonoBehaviour {

	public Transform target;
	public float lookSmooth = 0.09f;
	public Vector3 offsetFromTarget = new Vector3 (0f,6f,-8f);
	public float xTilet = 10;

	Vector3 destination =Vector3.zero;
	Character_Controller character_controller;
	float rotateVel =0f;

	void Start()
	{
		setCameraTarget (target);
	}
	public void setCameraTarget(Transform t)
	{
		target = t;
		if (target != null) {
			if (target.GetComponent<Character_Controller> ()) {
				character_controller = target.GetComponent<Character_Controller> ();
			} else
				Debug.LogError ("the camera's target needs a character controller");
			
		} else
			Debug.LogError ("Your Camera  needs a target");
	}

	void LateUpdate()
	{
		//moving
		MoveToTarget();
		//rotating
		LookAtTarget();

	}

	void MoveToTarget()
	{
		destination = character_controller.TargetRotation*offsetFromTarget;
		destination += target.position;
		transform.position = destination;

	}
	void LookAtTarget()
	{
		float eulerYAngle = Mathf.SmoothDampAngle (transform.eulerAngles.y, target.eulerAngles.y, ref rotateVel,lookSmooth);
		transform.rotation = Quaternion.Euler (transform.eulerAngles.x,eulerYAngle,0);
	}
}
