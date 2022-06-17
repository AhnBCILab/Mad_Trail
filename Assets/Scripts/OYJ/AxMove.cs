using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxMove : MonoBehaviour
{
    [SerializeField]
    private int movespeed;
    // Start is called before the first frame update
    private Vector3 pointCube;

    [SerializeField]
    private AudioSource axeAudio;
    public int set;
    private float eulerZ;

    private bool isWork;
    void Start()
    {
        isWork = false;
        pointCube = this.transform.GetChild(1).position;
        Debug.Log("½ÃÀÛ " + transform.rotation.eulerAngles.z);

    }

    // Update is called once per frame
    void Update()
    {
        if (isWork == true)
        {
            eulerZ = transform.rotation.eulerAngles.z;

            if (eulerZ > set || eulerZ == 0)
            {
                if (eulerZ < 300 && eulerZ != 0)
                {
                    movespeed = 12;
                }
                transform.RotateAround(pointCube, new Vector3(-1, 0, 1.45f), 10 * Time.fixedDeltaTime * movespeed);
            }
            else
            {
                isWork = false;
            }
        }
        
        
        //transform.RotateAround(pointCube, new Vector3(-1, 0, 0), 10 * Time.fixedDeltaTime * movespeed);
        //Debug.Log(transform.rotation.eulerAngles.z);


    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            isWork = true;
            axeAudio.Play();
        }
    }
}
