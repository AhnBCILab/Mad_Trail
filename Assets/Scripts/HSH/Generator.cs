using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    int count;
    [SerializeField]
    float yMin;
    [SerializeField]
    GameObject [] Items;
    void Start()
    {
        Vector3 temp;
        float xrange= this.transform.localScale.x/2;
        float yrange= this.transform.localScale.y/2;
        float zrange= this.transform.localScale.z/2;
        for(int i = 0 ; i< count;i++){
            temp.x = Random.Range(-1*xrange,xrange);
            temp.y= Random.Range(yMin,yrange);
            temp.z= Random.Range(-1*zrange,zrange);
            Instantiate(Items[Random.Range(0,Items.Length)],this.transform.position+temp,Quaternion.Euler(new Vector3(0,0,0)));
        }
        Destroy(this.gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
