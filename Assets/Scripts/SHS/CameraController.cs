using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    Color startColor = new Color32(255, 0, 0, 0);
    Color endColor = new Color32(255, 255, 255, 50);
    private float time;
    private float duration = 1.0f;

    [SerializeField]
    private Image bloodyScreen;

    bool isDamage;

    void Start() {
    	bloodyScreen.color = startColor;
        isDamage = false;
        //StartCoroutine("LerpColor");
        
    }
    
    void Update() {
 
        // float t = Mathf.PingPong(time, duration) / duration;
        /*
        if(Input.GetKeyDown("space")) {
            isDamage = true;
        }
        */

        if(isDamage) {
            time += Time.deltaTime;
            if(time * 2 < 1)
            bloodyScreen.color = Color.Lerp(startColor, endColor, time*2);
            else if(time * 2 > 2){    
                time = 0;
                isDamage= false;
            }
            else
            bloodyScreen.color = Color.Lerp(startColor, endColor, 2f-time*2);
            
            //Debug.Log("time : "+time);
            // Invoke("bloodyScreenOff", 1.0f);
        }
    }
    public void GetDamage(){
        isDamage=true;
    }
    /*
    IEnumerator LerpColor() {
        time += Time.deltaTime;
        float t = Mathf.PingPong(time, duration) / duration;
        if(Input.GetKeyDown("space")) {
            bloodyScreen.color = Color.Lerp(startColor, endColor, t);
            //bloodyScreen.color = Mathf.PingPong(time, 1.0f);
            //Invoke("bloodyScreenOff", 0.5f);
            yield return new WaitForSeconds(0.5f);
        }
    }
    */
    // void bloodyScreenOff() {
        
    //     isDamage = false;
    //     bloodyScreen.color = startColor;
    //     //time = 0.0f;
    // }
    
}
