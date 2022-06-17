using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotelDefenceTrigger : MonoBehaviour
{
    //private 
    public GameObject [] Mob;
    private float time = 0.0f;
    [SerializeField] float delayTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform.tag == "Player"){

                Invoke("ActiveEnemy0",2f);
                Invoke("ActiveEnemy1",4f);
                Invoke("ActiveEnemy2",7f);
                Invoke("ActiveEnemy3",10f);
                Invoke("ActiveEnemy4",13f);
        }        
    }

    void ActiveEnemy0(){Mob[0].gameObject.SetActive(true); }
    void ActiveEnemy1(){Mob[1].gameObject.SetActive(true); }
    void ActiveEnemy2(){Mob[2].gameObject.SetActive(true); }
    void ActiveEnemy3(){Mob[3].gameObject.SetActive(true); }
    void ActiveEnemy4(){Mob[4].gameObject.SetActive(true); }
    
}
