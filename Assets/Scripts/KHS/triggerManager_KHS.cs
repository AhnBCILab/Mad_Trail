using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

/// <summary>
/// Biosemi 연동을 위해 triggerManager class를 제작하였다.
/// </summary>
public class triggerManager_KHS : MonoBehaviour
{
    SerialPort m_SerialPort = new SerialPort("COM6", 115200, Parity.None, 8, StopBits.One);
    string m_Data = null;
    byte[] test_ = new byte[1] { 0x01 };
    byte[] buffer = new byte[1];
    private IEnumerator coroutine;

    void Awake() {
        m_SerialPort.Open();    // m_SerialPort 연결
    }

    /// <summary>
    /// 트리거 정보를 Biosemi 저장한다,
    /// </summary>
    public void writeTriggerEvent(byte eventID) {
        try
            {
                if (m_SerialPort.IsOpen)
                {
                    buffer[0] = eventID;
                    m_SerialPort.Write(buffer, 0,1);
                    Debug.Log("IsSending");
                }
                else Debug.Log("Is Closed");
            }

            catch (Exception e)
            {
                Debug.Log(e);
            }
    }

    public void writeTriggerEvent() {
        try
            {
                if (m_SerialPort.IsOpen)
                {
                    m_SerialPort.Write(test_, 0,1);
                    Debug.Log("IsSending");
                }
                else Debug.Log("Is Closed");
            }

            catch (Exception e)
            {
                Debug.Log(e);
            }
    }

    /// <summary>
    /// 트리거 정보를 waitTime마다 저장한다.
    /// </summary>
    private IEnumerator writePerSec(float waitTime)
    {
        while (true)
        {
            try
            {
                if (m_SerialPort.IsOpen)
                {
                    m_SerialPort.Write(test_, 0, 1);
                    Debug.Log("IsSending");
                }
                else Debug.Log("Is Closed");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    void OnApplicationQuit()
    {
        m_SerialPort.Close();
    }
}
