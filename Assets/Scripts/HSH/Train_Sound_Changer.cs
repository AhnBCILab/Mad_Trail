using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train_Sound_Changer : MonoBehaviour
{
    [SerializeField]
    Cinemachine.CinemachineDollyCart cart;
    [SerializeField]
    Cinemachine.SpeedChanger speed;
    
    [SerializeField]
    AudioClip High;
    
    [SerializeField]
    AudioClip Normal;
    
    [SerializeField]
    AudioClip Slow;
    AudioSource audioSource;

    [SerializeField]
    AudioSource BreakSource;
    [SerializeField]
    float StopSpeed;
    [SerializeField]
    float SlowSpeed;
    [SerializeField]
    float NormalSpeed;
    bool isBreaking;
    int exState;
    
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource=this.GetComponent<AudioSource>();
        isBreaking=false;
        audioSource.Stop();
        exState=-1;
    }

    // Update is called once per frame
    void Update()
    {

                if(cart.m_Speed<=StopSpeed&&exState!=0){
                    
                    Debug.Log("Slow");
                    audioSource.Stop();
                    exState=0;
                }
                else if(cart.m_Speed>StopSpeed&&cart.m_Speed<SlowSpeed&&exState!=1){
                    Debug.Log("Slow");
                    audioSource.clip=Slow;
                   audioSource.Play();
                   exState=1;
                }
                else if(cart.m_Speed>=SlowSpeed&&cart.m_Speed<NormalSpeed&&exState!=2){
                     Debug.Log("Normal");
                    audioSource.clip=Normal;
                    audioSource.Play();
                    exState=2;
                }
                else if(NormalSpeed<=cart.m_Speed&&exState!=3){
                    audioSource.clip=High;
                     Debug.Log("High");
                    audioSource.Play();
                    exState=3;
                }
            //}
        
    }
}
