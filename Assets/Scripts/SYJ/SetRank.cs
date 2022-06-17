using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRank : MonoBehaviour
{
     [SerializeField] GameManager GM;
   private TextMesh rankText;
    // Start is called before the first frame update
    void Start()
    {
        rankText = this.GetComponent<TextMesh>();
        rankText.text = "00";
    }

    // Update is called once per frame
    void Update()
    {
        if(GM.currScore >= 3500) rankText.text = "A";
        else if(GM.currScore >= 3000) rankText.text = "B";
        else if(GM.currScore >= 2500) rankText.text = "C";
        else rankText.text = "D";
    }
}
