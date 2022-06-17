using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class horrorObjectController : MonoBehaviour
{
    public GameManager GM;
    private bool Horror;
    private GameObject pumpkin;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Horror = GM.GetComponent<GameManager>().HorrorGetter();
        ObjectSetActive();
    }

    void ObjectSetActive(){
        for(int i=0;i<this.transform.childCount;i++){
            pumpkin = this.transform.GetChild(i).gameObject; 
            if(Horror) pumpkin.SetActive(true);
            else  pumpkin.SetActive(false);
        }
    }
}
