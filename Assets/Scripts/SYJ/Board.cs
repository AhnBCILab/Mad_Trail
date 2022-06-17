using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject ScoreBoard;
    [SerializeField] GameObject EndButton;
    [SerializeField] GameObject RestartButton;
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
            Invoke("ShowScoreBoard",0.5f);
           

        }        
    }

    void ShowScoreBoard(){
        ScoreBoard.gameObject.SetActive(true);
        EndButton.gameObject.SetActive(true);
        RestartButton.gameObject.SetActive(true);

    }
}



