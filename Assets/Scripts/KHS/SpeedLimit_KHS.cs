using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLimit_KHS : MonoBehaviour
{
    public GameObject player;
    private GameObject SpeedGate;
    // Start is called before the first frame update

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            Debug.Log("Hello!");
        }
    }
}
