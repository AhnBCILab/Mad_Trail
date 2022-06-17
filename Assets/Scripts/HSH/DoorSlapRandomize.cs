using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlapRandomize : MonoBehaviour
{
    private GameObject TeleportedPosition;
    private Animator ani;
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip creakingDoorSound;
    float DelayTime;
    bool DoorSlam;
    // Start is called before the first frame update
    void Start()
    {
        TeleportedPosition=GameObject.Find("TeleportedPosition");
        audioSource = this.GetComponent<AudioSource>();
        //DelayTime=Random.Range(0.3f,0.7f);
        DelayTime = 10.0f;
        ani=this.GetComponent<Animator>();
        DoorSlam=false;
    }

    void Update() {
        DoorAnimeStart();
        //DoorAnimeStop();
    }

    void DoorAnimeStart(){
        ani.SetBool("SlapOn",true);
        //DoorAnimeStop();
        Invoke("DoorAnimeStop",DelayTime);
    }
    void DoorAnimeStop(){
        ani.SetBool("SlapOn",false);
        if(DoorSlam)
        Invoke("DoorAnimeStart",DelayTime);
    }

    public void PlaySound() {
        audioSource.PlayOneShot(creakingDoorSound, 1.0f);
    }   

/*
    // Update is called once per frame
    void Update()
    {
        if(TeleportedPosition.GetComponent<TeleportedFlowController>().Flow == 1 && !DoorSlam){
            DoorSlam = true;
            Debug.Log("DoorWorking");
            Invoke("DoorAnimeStart",DelayTime);
        }
        else if(TeleportedPosition.GetComponent<TeleportedFlowController>().Flow != 1){
            ani.SetBool("SlapOn",false);
            DoorSlam = false;
        }
    }
    
*/
}
