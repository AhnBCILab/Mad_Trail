using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine{
    public class SpeedChanger : MonoBehaviour
    {

        public float TargetSpeed;
        public float Accelation;
        private bool BreakAccelation;
        public GameObject test;
        public CinemachineDollyCart cart;
        // Start is called before the first frame update
        void Start()
        {
            BreakAccelation =false;
            StartCoroutine("SpeedChange");
        }

        private void OnEnable() {
            BreakAccelation =false;
            StartCoroutine("SpeedChange");
        }

        // Update is called once per frame
        IEnumerator SpeedChange(){
            Debug.Log(TargetSpeed+"/"+cart.m_Speed);
            while(true){
            if(Accelation>0){
                BreakAccelation =false;
                if(TargetSpeed>cart.m_Speed&&Accelation>0){
                    cart.m_Speed+=Accelation*Time.deltaTime;
                }
            }else{
                if(TargetSpeed<cart.m_Speed&&Accelation<0){
                    cart.m_Speed+=Accelation*Time.deltaTime;
                }
                if(cart.m_Speed<0){
                    if(!BreakAccelation){
                        BreakAccelation = true;//잠시 뒤로 쏠리는 느낌 연출 
                        yield return new WaitForSeconds(1);   
                    }else{
                        cart.m_Speed=0;//완전정지
                    }
                }
            }
            yield return null;
            }
        }
        
    }   
}