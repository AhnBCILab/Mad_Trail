using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraSwithcher : MonoBehaviour
{
    [SerializeField]
    GameObject DefaultCamera;

    [SerializeField]
    GameObject FXCamera;

    [SerializeField]
    bool DefaultNow=false;

    [SerializeField]
    private AudioClip highFrequencySound;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private CameraShake cs;

    [SerializeField]
    private GameManager gm;

    [SerializeField]
    private GameObject player;

    private int count = 0;


    void Start() {
        audioSource = DefaultCamera.GetComponent<AudioSource>();
        cs = DefaultCamera.GetComponentInChildren<CameraShake>();
        gm = GetComponentInParent<GameManager>();
        count = 0;
    }

    // Start is called before the first frame update
    public void Update(){
        //if(Input.GetKeyDown(KeyCode.Space)) // 순간이동 발동 조건
        //SwitchCamera();

        if(player.GetComponent<CinemachineDollyCart>().m_Position >= 630.0f && player.GetComponent<CinemachineDollyCart>().m_Position <= 631.0f) {
            if(count == 0) {
                count++;
                Debug.Log("닿음");
                SwitchCamera();
            }
        
        }
        /*        
        if(gm.HorrorGetter()) { // 순간이동 발동 조건 (이걸로 하면 잘 안되네..??)
            SwitchCamera();
        }
        */
    }
    public void SwitchCamera(){
        if(FXCamera.activeSelf == true) {
            audioSource = FXCamera.GetComponent<AudioSource>();
            cs = FXCamera.GetComponentInChildren<CameraShake>();
        }

        else if(DefaultCamera.activeSelf == true) {
            audioSource = DefaultCamera.GetComponent<AudioSource>();
            cs = DefaultCamera.GetComponentInChildren<CameraShake>();
        }

        audioSource.PlayOneShot(highFrequencySound, 0.5f);
        //audioSource.Play();
        StartCoroutine(cs.Shake(0.5f, 2.0f));
        Invoke("CamToggle", 2.0f);

    }

    private void CamToggle() {
        audioSource.Stop();
        DefaultCamera.SetActive(DefaultNow);
        FXCamera.SetActive(!DefaultNow);
        DefaultNow=!DefaultNow;
    }

}
