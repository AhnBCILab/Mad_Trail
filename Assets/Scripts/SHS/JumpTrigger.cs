using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject player;
    public GameObject jumpCam;
    //public GameObject flashImg;

    [SerializeField]
    private Animator anim;

    public CameraShake cameraShake;

    
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player") {
            StartCoroutine(endJump());
        }
    }


    IEnumerator endJump() {
        yield return new WaitForSeconds(2.03f);
        jumpCam.SetActive(true);
        anim.SetBool("isScream", true);
        //player.SetActive(false);
        //flashImg.SetActive(true);
        StartCoroutine(cameraShake.Shake(0.5f, 3.0f));

        yield return new WaitForSeconds(3.0f);
        anim.SetBool("isScream", false);
        //player.SetActive(true);
        jumpCam.SetActive(false);
        //flashImg.SetActive(false);
    }
}
