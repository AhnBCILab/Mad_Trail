using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Vector3 originPos;

    //[SerializeField]
    //private Light redLight;

    //[SerializeField]
    //private GameObject Light;
    

    Color startColor = new Color32(180, 171, 171, 255);
    Color endColor = new Color32(255, 0, 0, 255);
 
    void Start () {
        originPos = transform.localPosition;
    }
    
    void Update() {
        /*
        if(Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(Shake(0.5f, 0.5f));
        }
        */
    }
    public IEnumerator Shake(float _amount,float _duration)
    {
        float timer = 0;

        while(timer <= _duration)
        {
            //Light.SetActive(true);
            //redLight.enabled = true;
            transform.localPosition = (Vector3)Random.insideUnitCircle * _amount + originPos;

            /*
            if(timer * 2 < _duration) {
                redLight.color = Color.Lerp(startColor, endColor, timer*2);
            }
            else {
                redLight.color = Color.Lerp(startColor, endColor, _duration*2 - timer*2);
            }
            */
            timer += Time.deltaTime;
            yield return null;
        }
        //Light.SetActive(false);
        //redLight.enabled = false;
        transform.localPosition = originPos;
 
    }
}
