using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Looxid.Link
{
    public enum LooxidLinkMessageType
    {
        CoreDisconnected = 0,
        HubDisconnected = 1,
        SensorOff = 2,
        NoiseSignal = 3
    }

    public class LooxidLinkMessage : MonoBehaviour
    {
        public CanvasGroup disconnectedPanel;   // 연결이 끊겼을 때 UI
        public CanvasGroup noiseSingalPanel;    // noise가 많을 때 UI
        public CanvasGroup sensorOffPanel;      // sensor가 꺼졌을 때 UI

        private float disconnectedWindowAlpha = 0.0f;       
        private float disconnectedWindowTargetAlpha = 0.0f;
        private float noiseSingalWindowAlpha = 0.0f;
        private float noiseSingalWindowTargetAlpha = 0.0f;
        private float sensorOffWindowAlpha = 0.0f;
        private float sensorOffWindowTargetAlpha = 0.0f;

        private LooxidLinkMessageType messageType;

        public void ShowMessage(LooxidLinkMessageType messageType)
        {
            this.messageType = messageType;
            if (messageType == LooxidLinkMessageType.CoreDisconnected || messageType == LooxidLinkMessageType.HubDisconnected)
            {
                if (disconnectedWindowAlpha <= 0.0f)
                {
                    disconnectedWindowAlpha = 0.0f;
                    disconnectedWindowTargetAlpha = 0.82f;
                }
            }
            else if (messageType == LooxidLinkMessageType.NoiseSignal)
            {
                if (noiseSingalWindowAlpha <= 0.0f && sensorOffWindowAlpha <= 0.0f)
                {
                    noiseSingalWindowAlpha = 0.0f;
                    noiseSingalWindowTargetAlpha = 0.82f;
                }
            }
            else if (messageType == LooxidLinkMessageType.SensorOff)
            {
                if (sensorOffWindowAlpha <= 0.0f)
                {
                    sensorOffWindowAlpha = 0.0f;
                    sensorOffWindowTargetAlpha = 0.82f;
                    noiseSingalWindowTargetAlpha = -0.02f;
                }
            }
        }

        public void HideMessage(LooxidLinkMessageType messageType)
        {
            this.messageType = messageType;
            if (messageType == LooxidLinkMessageType.CoreDisconnected || messageType == LooxidLinkMessageType.HubDisconnected)
            {
                if (disconnectedWindowAlpha > 0.0f) // 완전히 투명이 되지 않았다면
                {
                    disconnectedWindowTargetAlpha = -0.02f; // 서서히 값을 떨어뜨려준다.
                }
            }
            else if (messageType == LooxidLinkMessageType.NoiseSignal)
            {
                if (noiseSingalWindowAlpha > 0.0f)
                {
                    noiseSingalWindowTargetAlpha = -0.02f;
                }
            }
            else if (messageType == LooxidLinkMessageType.SensorOff)
            {
                if (sensorOffWindowAlpha > 0.0f)
                {
                    sensorOffWindowTargetAlpha = -0.02f;
                }
            }
        }

        void Update()
        {   // Math.Lerp를 이용하여 0.2 배수만큼 Alpha 값을 줄인다. 
            disconnectedWindowAlpha = Mathf.Lerp(disconnectedWindowAlpha, disconnectedWindowTargetAlpha, 0.2f);
            noiseSingalWindowAlpha = Mathf.Lerp(noiseSingalWindowAlpha, noiseSingalWindowTargetAlpha, 0.2f);
            sensorOffWindowAlpha = Mathf.Lerp(sensorOffWindowAlpha, sensorOffWindowTargetAlpha, 0.2f);
            // 각각의 Panel의 투명도를 조절하여, 원하는 정보를 표시해준다.
            disconnectedPanel.alpha = disconnectedWindowAlpha; 
            noiseSingalPanel.alpha = noiseSingalWindowAlpha;
            sensorOffPanel.alpha = sensorOffWindowAlpha;
        }
    }
}