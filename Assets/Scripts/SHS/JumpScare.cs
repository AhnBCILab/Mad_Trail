using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpScare : MonoBehaviour
{

    public GameObject daegari;
    public GameObject spider;
    public GameObject Axe;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip scareSound;

    [SerializeField]
    private AudioClip spiderSound;

    [SerializeField]
    private AudioClip axeSound;

    
    [SerializeField]
    private GameObject gm;

    private bool Horror;
    private int function_caller;
    private bool isHorror = false;
    

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gm = GameObject.Find("GM");
        Invoke("Jumping", Random.Range(200.0f, 300.0f));
        Invoke("Hanging", Random.Range(30.0f, 150.0f));
        Invoke("AxeComing", Random.Range(150.0f, 200.0f));

        Invoke("Jumping", Random.Range(80.0f, 90.0f));
        Invoke("Hanging", Random.Range(50.0f, 180.0f));
        Invoke("AxeComing", Random.Range(60.0f, 120.0f));
    }

    
    void Update() {
        Horror = gm.GetComponent<GameManager>().HorrorGetter();

        if(Horror && !isHorror) {
            isHorror = true;
            //Debug.Log("랜덤 갑툭튀 출현!!!");

            function_caller = Random.Range(0, 3);

            switch(function_caller) {
                case 0:
                    Invoke("Jumping", 1.0f);
                    break;
                case 1:
                    Invoke("Hanging", 1.0f);
                    break;
                case 2:
                    Invoke("AxeComing", 1.0f);
                    break;
                
            }
        }

        if(!Horror)
            isHorror = false;
    }
    
    

    public void Jumping() {
        daegari.SetActive(true);
        audioSource.PlayOneShot(scareSound, 5.0f);
        Invoke("JumpOff", 3.0f);
    }

    public void JumpOff() {
        daegari.SetActive(false);
        audioSource.Stop();
        isHorror = false;
    }

     public void Hanging() {
        spider.SetActive(true);
        audioSource.PlayOneShot(spiderSound, 5.0f);
        Invoke("HangOff", 2.0f);
    }

    public void HangOff() {
        spider.SetActive(false);
        audioSource.Stop();
        isHorror = false;
    }

    public void AxeComing() {
        Axe.SetActive(true);
        audioSource.PlayOneShot(axeSound, 5.0f);
        Invoke("AxeOut", 1.3f);
    }

    public void AxeOut() {
        Axe.SetActive(false);
        audioSource.Stop();
        isHorror = false;
    }
}
