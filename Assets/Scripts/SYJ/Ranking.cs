using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranking : MonoBehaviour
{
    [SerializeField] GameObject Board;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EndPoint"){
            print("rank");
            Invoke("ShowEndBoard",5);
        }        
    }

    void ShowEndBoard(){
        Board.gameObject.SetActive(true);
    }
}
