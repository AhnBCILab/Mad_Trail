using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullMove : MonoBehaviour
{
    public float angle;

    private float lerpTime = 0;
    private float speed = 2f;

    [SerializeField]
    private float leftx;

    [SerializeField]
    private float lefty;

    [SerializeField]
    private float leftz;

    [SerializeField]
    private float rightx;

    [SerializeField]
    private float righty;

    [SerializeField]
    private float rightz;
    // Update is called once per frame
    void Update()
    {
        lerpTime += Time.deltaTime * speed;
        transform.rotation = CalculateMovementOfPendulum();
    }

    Quaternion CalculateMovementOfPendulum()
    {
        return Quaternion.Lerp(Quaternion.Euler(new Vector3(leftx, lefty, leftz)* angle), Quaternion.Euler(new Vector3(-leftx, -lefty, -leftz)* angle), GetLerpTParam());
    }

    float GetLerpTParam()
    {
        return (Mathf.Sin(lerpTime) + 1) * 0.5f;
    }
}
