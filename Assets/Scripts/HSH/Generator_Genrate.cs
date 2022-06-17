using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine{



public class Generator_Genrate : MonoBehaviour
{
      public CinemachineSmoothPath m_Path;
      [SerializeField]
      bool  [] TargetGenPoint;
      [SerializeField]
      int length;
      [SerializeField]
      GameObject Generator;

    // Start is called before the first frame update
    void Start()
    { 
        length=m_Path.m_Waypoints.Length;
        for(int i = 0; i<m_Path.m_Waypoints.Length;i++){
          if(TargetGenPoint[i]){
            Instantiate(Generator,m_Path.m_Waypoints[i].position,Quaternion.Euler(new Vector3(0,0,0)));
          }
        }
//       Debug.Log(m_Path.m_Waypoints[m_Path.m_Waypoints.Length-1].position);
     }

    // Update is called once per frame
    void Update()
    {
        
    }
}
}
