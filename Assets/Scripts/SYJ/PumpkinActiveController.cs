using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinActiveController : MonoBehaviour
{
    public AudioClip[] sound;
    private AudioSource audio;

    public int npcHP = 100;
    [SerializeField] private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        audio = this.GetComponent<AudioSource>();
        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
  
        audio.clip = sound[0]; // active ;
        Debug.Log("created!"); 
        audio.Play();
        Invoke("playsd",1.0f);
    }
    public void ApplyDamage(int points)
    {
        npcHP -= points;

        if (npcHP <= 0)  
        {
            Debug.Log("die");
            anim.SetBool("isDying", true);
            audio.clip = sound[1]; // active;
            audio.Play();
            Invoke("playsd",1.0f);
            Destroy(this.gameObject, 2.0f);
        } else
        {
            anim.SetBool("isDying", false);
        }

    }
    void playsd(){
        audio.Stop();
    }   
}
