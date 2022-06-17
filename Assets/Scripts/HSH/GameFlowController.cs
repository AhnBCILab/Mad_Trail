using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    [SerializeField]
    private int flow;
    [SerializeField]
    private string flowName;
    
    // Start is called before the first frame update
    [SerializeField]
    private GameObject EEGController;
    [SerializeField]
    private GameObject TutorialManager;
    [SerializeField]
    private GameObject Results;
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private List<Cinemachine.Generator_Genrate> tracks;

    void Start()
    {

        flow=0;
        if(EEGController!=null)
        EEGController.SetActive(false);
        if(Results!=null)
        Results.SetActive(false);
        if(TutorialManager!=null)
        TutorialManager.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //flow 0 is Tutorial
        //flow 1 is gameplay
        //flow 2 is result
        switch(flow){
            case 0:
                if(EEGController!=null)
                    EEGController.SetActive(false);
                if(TutorialManager!=null)
                    TutorialManager.SetActive(true);
                flowName="Tutorial";
                Player.GetComponent<Cinemachine.CinemachineDollyCart>().m_Speed=0;
                break;
            case 1:
                if(EEGController!=null)
                    EEGController.SetActive(true);
                for(int i=0;i<tracks.Count;i++){
                    tracks[i].enabled=true;
                }
                flowName="Game";
               // Player.GetComponent<Cinemachine.CinemachineDollyCart>().m_Speed=3;
                break;
            case 2:
                flowName="Results";
                EEGController.SetActive(false);
                if(Results!=null)
                    Results.SetActive(true);                 
                break;
        }
    }
    public void flowSetter(int input){
        flow=input;
    }
}
