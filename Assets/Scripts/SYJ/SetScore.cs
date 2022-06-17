using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScore : MonoBehaviour
{
    [SerializeField] GameManager GM;
    private TextMesh scoreText;
    // Start is called before the first frame update
    void Start()
    {
        scoreText = this.GetComponent<TextMesh>();
        scoreText.text = "00";
    }

    // Update is called once per frame
    void Update()
    {
       scoreText.text = GM.currScore.ToString();
    }
}
