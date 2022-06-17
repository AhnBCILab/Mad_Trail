using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonToPlayer : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("DollyCart1");
    }

    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        if(other.transform.name == "BalloonToPlayer")
        {
            this.gameObject.transform.rotation = Quaternion.Slerp(this.gameObject.transform.rotation,
            Quaternion.LookRotation(player.transform.position - this.gameObject.transform.position), 3 * Time.deltaTime);
            Debug.Log(this.transform.name + " : Check & Roatation");
        }
    }
    
    
}
