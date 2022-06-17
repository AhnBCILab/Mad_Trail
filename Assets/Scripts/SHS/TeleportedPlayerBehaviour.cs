using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportedPlayerBehaviour : MonoBehaviour
{
    private bool isEnter = false;
    private bool isEnd = false;

    [SerializeField]
    private EndingArmBehaviour eaBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isEnter) {
            this.transform.position += new Vector3(0.0f, 0.0f, 0.05f);
        }

        else if(eaBehaviour.endingCam.activeSelf == true && !isEnd) {
            this.transform.position += new Vector3(0.0f, 0.0f, 0.3f);
        }

        else if(isEnter) {
            StartCoroutine(eaBehaviour.armGrowing(3.0f));
        }
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.name == "EndTeleport") {
            isEnter = true;
        }

        else if(other.name == "StopPoint") {
            isEnd = true;
        }
    }
}
