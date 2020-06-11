using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterCtrl : MonoBehaviour {

    private GameObject Player;
    public AudioClip[] footsounds;
    public Transform eyes;
    public GameObject deathCamera;
    public Transform CamPos;
    public Canvas gameCanvas;


    private NavMeshAgent nav;
    private AudioSource sound;
    private Animator anim;
    private string state="idle";
    private bool alive = true;
    private float wait = 0f;
    private bool highAlert = false;
    private float alertness = 20f;

	// Use this for initialization
	void Start () 
    {
        nav = GetComponent<NavMeshAgent>();
        sound = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        anim.speed = 0.5f;
        Player = GameObject.Find("Player");
	}

    public void footstep(int _num)
    {
        sound.clip = footsounds[_num];
        sound.Play();
    }
    //check if we can see the player
    public void checkSight()
    {
        if(alive)
        {
            //RaycastHit rayHit;
            //if(Physics.Linecast(eyes.position,Player.transform.position,out rayHit))
            //{
                //print("hit" + rayHit.collider.gameObject.name);
                //if(rayHit.collider.gameObject.name=="Player")
               // {
                    if(state!="kill")
                    {
                        state = "chase";
                        nav.speed = 1f;
                        anim.speed = 1f;

                    }
                //}
            //}
        }   
    }
	
	// Update is called once per frame
	void Update () 
    {
        //Debug.Log(state);
        Debug.DrawLine(eyes.position,Player.transform.position,Color.green);
        if(alive)
        {
            anim.SetFloat("velocity", nav.velocity.magnitude);
            if(state=="idle")
            {
                //pick a random place to walk to 
                Vector3 randomPos = Random.insideUnitSphere * alertness;
                NavMeshHit navHit;
                NavMesh.SamplePosition(transform.position + randomPos, out navHit, 20f,NavMesh.AllAreas);

                //go near the player
                if(highAlert)
                {
                    NavMesh.SamplePosition(Player.transform.position + randomPos, out navHit, 20f, NavMesh.AllAreas);
                    //each time.lose awareness of player general position
                    alertness += 5f;
                    if (alertness > 20f)
                    {
                        highAlert = false;
                        nav.speed = 0.5f;
                        anim.speed = 0.5f;
                    }
                }

                nav.SetDestination(navHit.position);
                state = "walk";
            }

            //walk
            if(state=="walk")
            {
                if(nav.remainingDistance<=nav.stoppingDistance&&!nav.pathPending)
                {
                    state = "search";
                    wait = 5f;
                }
            }

            //search
            if(state=="search")
            { 
                if (wait > 0f)
                {
                    wait -= Time.deltaTime;
                    transform.Rotate(0f, 120f * Time.deltaTime, 0f);
                }
                else
                    state = "idle";
            }

            //chase
            if(state=="chase")
            {
                nav.destination = Player.transform.position;
                float distance = Vector3.Distance(transform.position, Player.transform.position);
                if (distance > 10f)
                {
                    state = "hunt";
                }
                //kill the player
                else if(nav.remainingDistance <= (nav.stoppingDistance + 2f) && !nav.pathPending)
                {
                    //game over,inistate the end canvas
                    Debug.Log("game over");
                    if(Player.GetComponent<Player>().alive){
                        state = "kill";
                        Player.GetComponent<Player>().alive = false;
                        Player.GetComponent<Character_Controller>().enabled = false;
                        deathCamera.SetActive(true);
                        deathCamera.transform.position = Camera.main.transform.position;
                        deathCamera.transform.rotation = Camera.main.transform.rotation;
                        Camera.main.gameObject.SetActive(false);
                        nav.speed = 0f;
                        Invoke("reset", 0.5f);
                    }

                }
            }
            //hunt
            if(state=="hunt")
            {
                if (nav.remainingDistance <= nav.stoppingDistance && !nav.pathPending)
                {
                    state = "search";
                    wait = 5f;
                    highAlert = true;
                    alertness = 5f;
                    checkSight();
                }
            }
            //kill
            if(state=="kill")
            {
                deathCamera.transform.position = Vector3.Slerp(deathCamera.transform.position, CamPos.position, 10f * Time.deltaTime);
                deathCamera.transform.rotation = Quaternion.Slerp(deathCamera.transform.rotation, CamPos.rotation, 10f * Time.deltaTime); 
                nav.SetDestination(deathCamera.transform.position);
            }

            //nav.SetDestination(Player.transform.position);  
        }

	}
    //reset
    void reset()
    {
        //inistate the end canvas
        gameCanvas.gameObject.SetActive(true);
    }


}
