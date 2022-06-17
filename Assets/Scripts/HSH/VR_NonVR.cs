using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_NonVR : MonoBehaviour
{
    [SerializeField]
    private bool VR;
    [SerializeField]
    private GameObject VR_Player;
    [SerializeField]
    private GameObject Non_VR_Player;
    

    // Start is called before the first frame update
    void Start()
    {
            VR_Player.SetActive(VR);
            Non_VR_Player.SetActive(!VR);
      }

    // Update is called once per frame
    void Update()
    {
        
    }
}
