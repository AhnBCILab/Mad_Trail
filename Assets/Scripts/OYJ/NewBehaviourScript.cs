using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    public void Start()
    {
        anim.SetBool("isHorror", false);
    }
    public void SetHorror()
    {
        anim.SetBool("isHorror", true);
    }
    public void SetNonHorror()
    {
        anim.SetBool("isHorror", false);
    }
}
