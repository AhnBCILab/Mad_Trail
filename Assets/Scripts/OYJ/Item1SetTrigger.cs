using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item1SetTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject Gm;
    [SerializeField]
    Animator anim;

    private bool Horror;
    void Start()
    {
        Gm = GameObject.Find("GM");
        anim.SetBool("isHorror", false);
    }

    // Update is called once per frame
    void Update()
    {
        Horror = Gm.GetComponent<GameManager>().HorrorGetter();
        anim.SetBool("isHorror", Horror);
    }
}
