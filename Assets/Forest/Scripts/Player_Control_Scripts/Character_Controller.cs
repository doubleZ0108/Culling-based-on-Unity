using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character_Controller : MonoBehaviour {

    [System.Serializable]
    public class MoveSettings
    {
        public Image testImage;
        public float fowardVel = 3f;
        public float rotateVel = 100f;
        public float jumpVel = 25f;
        public float disToGround = 1.1f;
        public LayerMask Ground;
    }

    [System.Serializable]
    public class PhysSettings
    {
        public float downAccel = 0.75f;
    }

    [System.Serializable]
    public class InputSettings
    {
        public float inputDelay = 0.1f;
        public string FOWARD_AXIS = "Vertical";
        public string Turn_AXIS = "Horizontal";
        public string JUMP_AXIS = "Jump";
    }

    public MoveSettings moveSettings = new MoveSettings();
    public PhysSettings physSettings = new PhysSettings();
    public InputSettings inputSettings = new InputSettings();


	
    bool jump = false;
    bool hit = false;
    bool isRunning = false;
    private Animator sappi_animator;

    Vector3 velocity = Vector3.zero;
	Quaternion targetRotation;
	Rigidbody rBody;
    float forwardInput, turnInput, jumpInput;

	public Quaternion TargetRotation{
		get{ return targetRotation; }
	}

    bool Grounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, moveSettings.disToGround, moveSettings.Ground);   
    }

	void Start()
	{
		targetRotation = transform.rotation;
		if (GetComponent<Rigidbody>() != null)
			rBody = GetComponent<Rigidbody> ();
		else
			Debug.LogError ("the character needs a rigidbody");

        forwardInput = turnInput =jumpInput= 0f;
        sappi_animator = GetComponent<Animator>();
	}

	void GetInput()
	{
        forwardInput = Input.GetAxis (inputSettings.FOWARD_AXIS); //interpolated
        turnInput = Input.GetAxis (inputSettings.Turn_AXIS);//interpolated
        jumpInput = Input.GetAxis(inputSettings.JUMP_AXIS);//non-interpolated
        hit = Input.GetButtonDown("Fire1");
        isRunning = Input.GetKey(KeyCode.W);
        jump = Input.GetButtonDown("Jump");

	}

	void Update()
	{
		GetInput ();
        Turn();
        updateAnimation();
        //if(Input.GetButtonDown("Fire1")){
        //    sappi_animator.Play("hit01", -1, 0f);
        //}
        //if (Input.GetButtonDown("Jump"))
        //{
        //    sappi_animator.Play("jump", -1, 0f);
        //}
	}
	void FixedUpdate()
	{
        Run();
        Jump();
        rBody.velocity = transform.TransformDirection(velocity);

	}
    void Run()
	{
        if (Mathf.Abs(forwardInput) > inputSettings.inputDelay)
        {
            //move
            //rBody.velocity = transform.forward * forwardInput * moveSettings.fowardVel;
            velocity.z = moveSettings.fowardVel * forwardInput;
        }
        else
            velocity.z = 0f;
			//zero.velocity
			//rBody.velocity=Vector3.zero;
	}
	void Turn()
	{
        if (Mathf.Abs (turnInput) > inputSettings.inputDelay) {
            targetRotation *= Quaternion.AngleAxis (moveSettings.rotateVel * turnInput * Time.deltaTime, Vector3.up);
		}
		transform.rotation = targetRotation;
	}

    void Jump()
    {
        if(jumpInput>0&&Grounded()){
            //jump
            velocity.y = moveSettings.jumpVel;
        }
        else if(jumpInput == 0&&Grounded()){
            //zero out our veloctiy.y 
            velocity.y = 0;
        }
        else{
            //decrease velocity.y
            velocity.y -= physSettings.downAccel;
        }
            
    }
    void updateAnimation(){
        sappi_animator.SetFloat("moveVel",Mathf.Abs(rBody.velocity.z) );
        sappi_animator.SetBool("hit",hit);
        sappi_animator.SetBool("jump",jump);
        sappi_animator.SetFloat("high",transform.position.y);
        sappi_animator.SetBool("isRunning",isRunning);
    }

}
