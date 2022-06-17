using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingArmBehaviour : MonoBehaviour
{
    public GameObject endingCam;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject originCam;

    [SerializeField]
    private CameraSwithcher cs;

    [SerializeField]
    private EndingDoorSlam endnigDoor;

    /*
    void Update() {
        if(originCam.activeSelf == true && player.activeSelf == true) {
            //StartCoroutine(armGrowing(3.0f));
        }
    }
    */

    public IEnumerator armGrowing(float waitingTime) {
        yield return new WaitForSeconds(waitingTime);
        endnigDoor.DoorAnimeStart();
        this.transform.localScale += new Vector3(0.1f, 0.0f, 0.0f);
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            Debug.Log("충돌!");
            originCam.SetActive(false);
            //this.gameObject.SetActive(false);
            Destroy(this.gameObject);
            endingCam.SetActive(true);
            endnigDoor.DoorAnimeEnd();
            cs.SwitchCamera();
            
        }
    }
}
