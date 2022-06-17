using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    private GameObject Gm;
    public int score;
    [SerializeField]
    bool IsZombie;
    private bool Horror;
    private void Start() {
        Gm=GameObject.Find("GM");
    }
    private void Update() {
        if(!IsZombie){
            Horror = Gm.GetComponent<GameManager>().HorrorGetter();
            transform.GetChild(0).gameObject.SetActive(!Horror);
            transform.GetChild(1).gameObject.SetActive(Horror);
        }
    }
}
