using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class normalObjectController : MonoBehaviour
{
    public GameManager GM;
    private bool Horror;
    void Start()
    {
        Horror = GM.GetComponent<GameManager>().HorrorGetter();
    }

    // Update is called once per frame
    void Update()
    {
        if(Horror) this.gameObject.SetActive(false);
        else this.gameObject.SetActive(true);
    }
}
