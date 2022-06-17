using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChangeController : MonoBehaviour
{
    private GameObject Gm;
    private bool Horror;
    private bool KeepHorror;

    // Start is called before the first frame update
    void Start()
    {
        Gm=GameObject.Find("GM");
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(KeepHorror == false){
            Horror = Gm.GetComponent<GameManager>().HorrorGetter();    
            transform.GetChild(0).gameObject.SetActive(!Horror);
            transform.GetChild(1).gameObject.SetActive(Horror);
            
            if(Horror == true)
            {
                KeepHorror = true;    
                StartCoroutine(WaitChange());       
            }
        }
        
        
        
    }

    IEnumerator WaitChange()
    {
        yield return new WaitForSeconds(5.0f);
        KeepHorror = false;
    }
}
