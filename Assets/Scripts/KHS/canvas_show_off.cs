using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvas_show_off : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
            if (Input.GetKeyDown(KeyCode.Alpha0))   // 뇌파 수치 데이터 표시
            {
                isActive = !isActive;
            }

            if (isActive){
                canvas.SetActive(true);
            } else
                canvas.SetActive(false);
    }
}
