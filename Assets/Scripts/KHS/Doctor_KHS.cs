using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor_KHS : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] AudioSource AS;
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            anim.SetTrigger("Trigger");
            AS.Play();
            Debug.Log("Doctor Coming");
        }
    }
}
