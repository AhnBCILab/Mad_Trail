using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingDoorSlam : MonoBehaviour
{

    private Animator anim;
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip doorOpenSound;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();
    }

    public void DoorAnimeStart() {
        anim.SetBool("isDoorSlam",true);
    }

    public void DoorAnimeEnd() {
        audioSource.Stop();
        anim.SetBool("isDoorSlam",false);
    }

    public void PlaySound() {
        audioSource.PlayOneShot(doorOpenSound, 5.0f);
    }
}
