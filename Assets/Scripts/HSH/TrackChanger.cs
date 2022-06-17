using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackChanger : MonoBehaviour//이 코드는 DollyCart가 가지는 컴포넌트이다.
{
    [SerializeField]
    Cinemachine.CinemachinePathBase Default;//기존에 달리고 있던 트랙 컴포넌트
    [SerializeField]
    Cinemachine.CinemachinePathBase second;//바꿀 컴포넌트
    
    [SerializeField]
    private bool typeofTrack_Start;

    [SerializeField]
    private bool track;

    // Start is called before the first frame update
    void Start()
    {
        if(Default!=null&&second!=null)
        this.GetComponent<Cinemachine.CinemachineDollyCart>().m_Path=Default;//처음에
        
      //  Invoke("ChangetheTrack",1);
    }

    // Update is called once per frame
    void Update()
    {
        if(Default!=null&&second!=null)
            ChangeTrack(typeofTrack_Start);

    }
    private void ChangeTrack(bool isStartTrack) {//갈림길 구간 진입시
        if(track)
            this.GetComponent<Cinemachine.CinemachineDollyCart>().m_Path=second;//경로를 바꾸어주는 코드이다.
        else if(!track){
            this.GetComponent<Cinemachine.CinemachineDollyCart>().m_Path=Default;//경로를 바꾸어주는 코드이다.
        }
        if(isStartTrack){
          this.GetComponent<Cinemachine.CinemachineDollyCart>().m_Position=0;//만약 변경되는 경로가 복붙을 통해 만들어진게 아니라 해당 갈림저에서 시작하는 경우 0으로 초기화해준다.
        }
    }
}
