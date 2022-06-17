using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorLightController : MonoBehaviour
{
    private GameObject Gm;
    private bool Horror;
    private Color normalColor = new Color(205/255f,169/255f,132/255f);
    private Color horrorColor = Color.red;
    private GameObject child;
    private Light lt;
    void Start()
    {
        Gm=GameObject.Find("GM");
    }
    void Update()
    {
        Horror = Gm.GetComponent<GameManager>().HorrorGetter();
        if(Horror){
            NormaltoHorror();
        }
        
        else{
            HorrortoNormal();
        }
    }

    void ChangeLight(Color currentColor, Color changeColor){
        for(int i=0;i<this.transform.childCount;i++){
            child = this.transform.GetChild(i).gameObject; 
            lt = child.GetComponent<Light>();
            //lt.color = Color.Lerp(currentColor, changeColor, 3f);  
            lt.color = changeColor;
            //lt.color = Color.red;
        }
    }

    void NormaltoHorror(){
        for(int i=0;i<this.transform.childCount;i++){
            child = this.transform.GetChild(i).gameObject; 
            lt = child.GetComponent<Light>();
            lt.color = Color.red;
            lt.intensity = 1.48f;
        }
    }
    void HorrortoNormal(){
        for(int i=0;i<this.transform.childCount;i++){
            child = this.transform.GetChild(i).gameObject; 
            lt = child.GetComponent<Light>();
            lt.color = normalColor;
            lt.intensity = 1.48f;
        }
    }
}
