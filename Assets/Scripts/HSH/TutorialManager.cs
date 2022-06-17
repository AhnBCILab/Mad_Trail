using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private float range;
    [SerializeField]
    GameObject [] Items;
    [SerializeField]
    private List<GameObject> Checker;
    [SerializeField]
    private GameObject StartSpeedGate;
    [SerializeField]
    private List<AudioClip> Introduce;
    private AudioSource AS;
    private int flow;

    [SerializeField] private GameObject _2DVisualizer_Obj;

    // Start is called before the first frame update
    void Start()
    {
        flow=0;
        AS=this.GetComponent<AudioSource>();
        AS.clip=Introduce[0];
        AS.Play();
    }

    // Update is called once per frame
    void Update()
    {
        int DeletedCount=0;
        switch(flow){
            case 0:
                if(!AS.isPlaying){
                    for(int i = 0 ; i< Items.Length;i++){
                        Vector3 temp;
                        temp.x = Random.Range(-1*range,range);
                        temp.y= Random.Range(0,range/2);
                        temp.z= Random.Range(-1*range,range);
                        Checker[i]=Instantiate(Items[i],this.transform.position+temp,Quaternion.Euler(new Vector3(0,0,0)));
                    }
                flow++;
                }
            break;
            case 1:
                for(int i=0;i<Checker.Count;i++){
                    if(Checker[i]==null)
                        DeletedCount++;
                }
                
                if(DeletedCount>2){
                    //GameObject.Find("GM").GetComponent<GameManager>().HorrorSet(true);
                }
                DeletedCount=0;
                for(int i = 0; i< Checker.Count;i++){
                   if(Checker[i]!=null)break;
                    else    DeletedCount++;
                }
                 if(DeletedCount>Checker.Count-1){
                    //GameObject.Find("GM").GetComponent<GameManager>().HorrorSet(false);
                    flow++;
                 } 
                 
            break;
            case 2:
                AS.clip=Introduce[1];
                AS.Play();
                flow++;
            break;
            case 3:
                if(!AS.isPlaying){
                    GameObject.Find("GM").GetComponent<GameFlowController>().flowSetter(1);
                    StartSpeedGate.SetActive(true);
                    _2DVisualizer_Obj.SetActive(true);
                }
            break;
        }
    }
}
