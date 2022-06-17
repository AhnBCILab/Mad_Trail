using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedGate : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private float targetspeed;
    [SerializeField]
    private float accelation;
    [SerializeField]
    int DelaytimeforReAccelation;
    [SerializeField]
    float reaccelation;
    [SerializeField]
    private float retarget;
    GameObject Player;
    
    [SerializeField]
    AudioClip SpeedAudio;
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioSource Break;
    [SerializeField]
    AudioSource zombie_createSound;
    [SerializeField]
    AudioSource pumpkin_createSound;

    [SerializeField]
    private bool break_Start;
    private bool once;

    public GameObject pumpkin;
    public GameObject zombie;
    public GameObject enemies;
    public GameObject player;

    [SerializeField]
    private GameObject gm;




    
    private void Start()
    {
        gm = GameObject.Find("GM");
        once = false;
        //player = GameObject.FindWithTag("Player");

    }
    
     private void Update() {

    }
    private void OnTriggerEnter(Collider other) {
        Debug.Log("스피드 게이트 지나침!");
        if(!once){    
              if(other.tag=="Player") {
                once=true;

                if(this.gameObject.name == "SpeedGate_7") {
                    StartCoroutine(DefenseStart(pumpkin, pumpkin_createSound));
                }

                if(this.gameObject.name == "SpeedGate_8") {
                    StartCoroutine(DefenseStart(zombie, zombie_createSound));
                    
                }

                if(this.gameObject.name == "SpeedGate_9") {
                    StartCoroutine(DefenseStart(enemies, zombie_createSound));
                    
                }

                if(SpeedAudio!=null&&audioSource!=null){
                    if(break_Start){
                        Debug.Log("BreakIN");
                    
                        Break.loop=false;
                        Break.Play();
                        Invoke("BreakStop",1);
                    
                    }
                    audioSource.clip=SpeedAudio;
                    audioSource.Play();
                }
            
                Player= other.gameObject;
                other.GetComponent<Cinemachine.SpeedChanger>().TargetSpeed=targetspeed;
                other.GetComponent<Cinemachine.SpeedChanger>().Accelation=accelation;
            
                if(DelaytimeforReAccelation>0) {
                    Invoke("ReaccelationStart",DelaytimeforReAccelation);
                }

            }   
        }
//        Debug.Log("Enter");
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player") {
            gm.GetComponent<GameManager>().isDefenseSetter(false);
        }
    }
    public void ReaccelationStart(){
        Player.GetComponent<Cinemachine.SpeedChanger>().TargetSpeed = retarget;
        Player.GetComponent<Cinemachine.SpeedChanger>().Accelation = reaccelation;

    }
    private void BreakStop(){
        Break.Stop();
    }


    public IEnumerator DefenseStart(GameObject enemy, AudioSource audio) {
        gm.GetComponent<GameManager>().isDefenseSetter(true);
        yield return new WaitForSeconds(1.0f);
        audio.Play();
        enemy.SetActive(true);
    }

}
