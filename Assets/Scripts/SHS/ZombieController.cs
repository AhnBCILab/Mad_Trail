using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private AudioSource screamSound;

    [SerializeField]
    private GameObject endingZombie;

    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other) {

        if(other.gameObject.tag == "Player") {
            endingZombie.SetActive(true);
            anim.SetBool("isScream", true);
            screamSound.Play();
        }
        
    }

    private void OnTriggerExit(Collider other) {
        anim.SetBool("isScream", false);
    }


}
