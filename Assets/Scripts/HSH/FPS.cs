using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    
    [SerializeField] GameManager GM;
    [SerializeField]
    float roatateSpeed;

    [SerializeField]
    private GameObject effect;

    [SerializeField]
        private AudioSource Fire;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h= Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");
        Vector3 dir = new Vector3(-v,h,0);
        transform.eulerAngles = new Vector3(this.transform.eulerAngles.x+dir.x*roatateSpeed,this.transform.eulerAngles.y+dir.y*roatateSpeed,0);
        
        if(Input.GetMouseButtonDown(0)){
            
            Fire.Play();
             RaycastHit hit; 
             Debug.Log(transform.position);
             Ray ray = new Ray(new Vector3(transform.position.x,transform.position.y,transform.position.z),transform.forward);
             Debug.DrawRay(new Vector3(transform.position.x,transform.position.y,transform.position.z), transform.forward*1000f, Color.red, 5f);


            if(Physics.Raycast(ray, out hit)){
               
                    Debug.Log(hit.collider.name);
                if(hit.collider.tag == "Item"){
                    Debug.Log("Hitted");
                    GM.currScore = GM.currScore + hit.transform.GetComponent<ItemInfo>().score;
                    hit.collider.gameObject.GetComponent<AudioSource>().Play();

                    effect.GetComponent<TextMesh>().text = "+";
                    Vector3 LookDir = this.transform.position - hit.point;
                    Quaternion rotation = Quaternion.LookRotation(LookDir, Vector3.forward);
                    GameObject a = Instantiate(effect, hit.point, rotation);

                    Destroy(hit.transform.gameObject, 0.3f);
                    Destroy(a, 0.8f);
                    
                }else if(hit.collider.tag == "Zombie") {
                    Debug.Log("Hit Zombie");
                    GM.currScore = GM.currScore + hit.transform.GetComponent<ItemInfo>().score;
                    hit.collider.gameObject.GetComponent<EnemyController_KHS>().ApplyDamage(50);    
                } else {
                }
            }
        }
        
    }
}
