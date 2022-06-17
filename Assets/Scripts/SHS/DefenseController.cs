using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseController : MonoBehaviour, IEntity
{
    float moveSpeed = 2.0f;
    public GameObject player;

    [SerializeField]
    private Animator anim;

    private bool isEnter = false;

    public int npcHP = 100;

    public CameraShake cameraShake;
    private bool CameraLock;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip laughSound;

    [SerializeField]
    private AudioClip zombieSound;
    
    [SerializeField]
    private AudioClip zombie_deathSound;

    [SerializeField]
    private AudioClip pumpkin_deathSound;

    void Start(){
        //this.gameObject = GetComponent<GameObject>();
        //player = GameObject.FindWithTag("Player");
        anim = this.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    } 
    
    void Update(){
        
        if(this.gameObject.activeSelf == true && !isEnter) {
            /*
            if(anim.name.Contains("pumpkin")) {
            audioSource.PlayOneShot(laughSound, 0.5f);
            
            }
            */
        
           
            /*
            if(anim.name.Contains("Zombie") && Vector3.Distance(player.transform.position, this.gameObject.transform.position) >= 13.0f) {
                //Debug.Log("런던 좀비 무야호");
                anim.SetBool("isStandUp", true);
            }
            */

            /*
            if(Vector3.Distance(player.transform.position, this.gameObject.transform.position) < 8.0f) {
                //anim.SetBool("isSprint", false);
                if(anim.name.Contains("Zombie")) {
                    //audioSource.PlayOneShot(zombieSound, 0.5f);
                    anim.SetBool("isStandUp", true);
                    anim.SetBool("isSprint", true);
                }


                //anim.SetBool("isJump", true);

            }
            */

            if(Vector3.Distance(player.transform.position, this.gameObject.transform.position) < 5.0f) {
                if(anim.name.Contains("pumpkin")) {
                    anim.SetBool("isJump", true);
                }
            }

            


            /*
            this.gameObject.transform.rotation = Quaternion.Slerp(this.gameObject.transform.rotation,
            Quaternion.LookRotation(player.transform.position - this.gameObject.transform.position), 3 * Time.deltaTime);
        
            //code for following the player
            this.gameObject.transform.position += this.gameObject.transform.forward * moveSpeed * Time.deltaTime;
            */

            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, player.transform.position, 1 * Time.deltaTime);
            this.gameObject.transform.LookAt(player.transform.position);
        }

        if(Vector3.Distance(player.transform.position, this.gameObject.transform.position) < 2.0f) {
            anim.SetBool("isZombieAttack", true);
            isEnter = true;
            Destroy(gameObject, 10.0f);
        }
    }


/*
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            Debug.Log("온트리거엔터");
            anim.SetBool("isZombieAttack", true);
            isEnter = true;
            Destroy(gameObject, 10.0f);
        }
    }
*/

    /*private void OnTriggerExit(Collider other) {
        if(other.tag == "Player") {
             Destroy(gameObject, 0.5f);
        }
    }*/

    public void ApplyDamage(int points)
    {

        Debug.Log("npcHP: " + npcHP);
        //AS.clip = m_OutchSound;
        //AS.Play();

        npcHP -= points;

        if (npcHP <= 0)  {
            anim.SetBool("isDying", true);

            if(anim.name.Contains("pumpkin")) {
                audioSource.PlayOneShot(pumpkin_deathSound, 1.0f);
            }

            else if(anim.name.Contains("Zombie")) {
                audioSource.PlayOneShot(zombie_deathSound, 1.0f);
            }

            //agent.enabled = false;
            //enemyAnimator.SetBool("isDead", true);
            Destroy(gameObject, 2.0f);
        }

        /*
        else {
            anim.SetBool("isDying", false);
        }
        */

    }

    public IEnumerator Attack() {
        
        if(anim.name.Contains("pumpkin")) {
            audioSource.PlayOneShot(laughSound, 0.5f);
        }

        else if(anim.name.Contains("Zombie")) {
            audioSource.PlayOneShot(zombieSound, 0.5f);
        }
        
        //anim.SetBool("isZombieAttack", true);
        //yield return new WaitForSeconds(0.0f);
        if(!CameraLock){
            if(Vector3.Distance(player.transform.position, this.gameObject.transform.position) < 3.0f) {
                StartCoroutine(cameraShake.Shake(0.2f, 0.2f));
            }
            CameraLock = true;
            Invoke("CameraLockFree", 1.5f);
        }

        yield return null;



    }
    public void CameraLockFree(){
        CameraLock = false;
        Debug.Log("CameraLockFree");
    }
}
