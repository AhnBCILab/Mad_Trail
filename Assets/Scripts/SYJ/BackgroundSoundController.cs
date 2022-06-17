using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSoundController : MonoBehaviour
{
    
    public AudioClip[] audioList;
    private AudioSource BGM;
    void Start()
    {
        BGM = this.GetComponent<AudioSource>();
    }
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.gameObject.tag);
        if(other.transform.gameObject.tag == "SoundTrigger"){
            var index = int.Parse(other.transform.gameObject.name);
            BGM.clip = audioList[index];
            BGM.Play();
        }   
    }
}
