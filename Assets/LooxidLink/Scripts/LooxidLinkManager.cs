/**
 * @author   Looxidlabs
 * @version  1.0
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Looxid.Link
{
    public class LooxidLinkManager : MonoBehaviour
    {
        #region Singleton

        private static LooxidLinkManager _instance; // 자기 자신을 가리키는 private class
        public static LooxidLinkManager Instance    // 자기 자신을 가리키는 public class, 생성자
        {
            get // 속성 값, 인덱스 요소를 반환 또는 속성 또는 인덱스의 get 메서드를 정의
            {
                if (_instance == null)  // _instance가 존재하지 않으면
                {
                    _instance = FindObjectOfType(typeof(LooxidLinkManager)) as LooxidLinkManager;   // Object 중에 LooxidLinkManager 형을 찾는다.
                    if (_instance == null)  // Scene _instance가 존재하지 않다면
                    {
                        _instance = new GameObject("LooxidLinkManager").AddComponent<LooxidLinkManager>();
                        DontDestroyOnLoad(_instance.gameObject); // Scene 바뀌어도 오브젝트 파괴 않되게 함.
                    }
                }
                return _instance;
            }
        }

        #endregion



        #region Variables

        private NetworkManager networkManager;

        private LinkCoreStatus linkCoreStatus = LinkCoreStatus.Disconnected;
        private LinkHubStatus linkHubStatus = LinkHubStatus.Disconnected;

        private bool isInitialized = false;

        [HideInInspector]
        public bool isLinkCoreConnected = false;
        [HideInInspector]
        public bool isLinkHubConnected = false;

        public static System.Action OnLinkCoreConnected;
        public static System.Action OnLinkCoreDisconnected;
        public static System.Action OnLinkHubConnected;
        public static System.Action OnLinkHubDisconnected;

        private LooxidLinkMessage messageUI;

        public EEGSensor sensorStatusData;
        private List<int> sensorScoreList;
        private List<int> noiseScoreList;

        private bool isDisplayDisconnectedMessage = true;
        private bool isDisplaySensorOffMessage = true;
        private bool isDisplayNoiseSignalMessage = true;

        private bool sensorOffMessage = false;
        private bool noiseSignalMessage = false;

        public static System.Action OnShowSensorOffMessage;
        public static System.Action OnHideSensorOffMessage;
        public static System.Action OnShowNoiseSignalMessage;
        public static System.Action OnHideNoiseSignalMessage;

        // Colors
        public static Color32 linkColor = new Color32(124, 64, 254, 255);

        #endregion


        #region MonoBehavior Life Cycle

        // 
        void OnEnable()
        {   
            LooxidLinkData.OnReceiveEEGSensorStatus += OnReceivedEEGSensorStatus;   //  Received EEGSensor 센서 정보 함수 등록 
            NetworkManager.OnLinkCoreConnected += OnNetworkLinkCoreConnected;       
            NetworkManager.OnLinkCoreDisconnected += OnNetworkLinkCoreDisconnected; 
            NetworkManager.OnLinkHubConnected += OnNetworkLinkHubConnected;         
            NetworkManager.OnLinkHubDisconnected += OnNetworkLinkHubDisconnected;   

            sensorScoreList = new List<int>();
            noiseScoreList = new List<int>();

            if (isInitialized)
            {
                StartCoroutine(DetectSensor()); 

                if (linkCoreStatus == LinkCoreStatus.Disconnected)  
                {
                    StartCoroutine(AutoConnection());   
                }
            }
        }
        void OnDisable()
        {
            LooxidLinkData.OnReceiveEEGSensorStatus -= OnReceivedEEGSensorStatus;
            NetworkManager.OnLinkCoreConnected -= OnNetworkLinkCoreConnected;
            NetworkManager.OnLinkCoreDisconnected -= OnNetworkLinkCoreDisconnected;
            NetworkManager.OnLinkHubConnected -= OnNetworkLinkHubConnected;
            NetworkManager.OnLinkHubDisconnected -= OnNetworkLinkHubDisconnected;
        }

        void OnApplicationQuit()
        {
            isInitialized = false;
        }

        void OnNetworkLinkCoreConnected()
        {
            if (OnLinkCoreConnected != null) OnLinkCoreConnected.Invoke();
        }
        void OnNetworkLinkCoreDisconnected()
        {
            if (OnLinkCoreDisconnected != null) OnLinkCoreDisconnected.Invoke();
        }
        void OnNetworkLinkHubConnected()
        {
            if (OnLinkHubConnected != null) OnLinkHubConnected.Invoke();
        }
        void OnNetworkLinkHubDisconnected()
        {
            if (OnLinkHubDisconnected != null) OnLinkHubDisconnected.Invoke();
        }

        void OnReceivedEEGSensorStatus(EEGSensor sensorStatusData)
        {
            this.sensorStatusData = sensorStatusData;
        }

        void Update()
        {
            if (networkManager == null) return;

            linkCoreStatus = networkManager.LinkCoreStatus;
            isLinkCoreConnected = (linkCoreStatus == LinkCoreStatus.Connected);

            linkHubStatus = networkManager.LinkHubStatus;
            isLinkHubConnected = (linkHubStatus == LinkHubStatus.Connected);
        }

        //
        IEnumerator DetectSensor()
        {
            while (this.gameObject.activeSelf && isInitialized)
            {
                if (isLinkHubConnected && isLinkCoreConnected && sensorStatusData != null)
                {
                    // Sensor Connection
                    int sensorScore = 0;
                    int numSensor = System.Enum.GetValues(typeof(EEGSensorID)).Length;
                    for (int i = 0; i < numSensor; i++)
                    {
                        int score = sensorStatusData.IsSensorOn((EEGSensorID)i) ? 1 : 0;    // 센서가 켜져있으면 1을 반환
                        sensorScore += score;
                    }
                    sensorScoreList.Add(sensorScore);   
                    if (sensorScoreList.Count > 20) sensorScoreList.RemoveAt(0);    // 가장 먼저 들어온 것 부터 삭제한다.
                    if (sensorScoreList.Count >= 20 && sensorScoreList.Sum() < 110) // 20번 센서 스코어 점수가 들어오지만 Score의 합이 110을 안 넘길 때 
                    {
                        ShowMessage(LooxidLinkMessageType.SensorOff);   // 센서가 꺼진 것으로 간주한다.
                    }
                    else
                    {
                        HideMessage(LooxidLinkMessageType.SensorOff);   // 센서가 켜진 것으로 간주한다. 
                    }

                    // Noise Signal
                    int noiseScore = 0;
                    for (int i = 0; i < numSensor; i++)
                    {
                        int score = sensorStatusData.IsNoiseSignal((EEGSensorID)i) ? 0 : 1;     // NoiseSignal이면 0을 반환, 정상신호면 1을 반환
                        noiseScore += score;
                    }
                    noiseScoreList.Add(noiseScore);
                    if (noiseScoreList.Count > 20) noiseScoreList.RemoveAt(0);
                    if (noiseScoreList.Count >= 20 && noiseScoreList.Sum() < 110)   // Noise Signal인 경우 110보다 작다.
                    {
                        ShowMessage(LooxidLinkMessageType.NoiseSignal);
                    }
                    else
                    {
                        HideMessage(LooxidLinkMessageType.NoiseSignal);
                    }
                }
                else
                {   // 센서와 노이즈 문제 없으니 UI화면을 흐리게 한다.
                    HideMessage(LooxidLinkMessageType.SensorOff);   
                    HideMessage(LooxidLinkMessageType.NoiseSignal);
                }
                // 0.1초마다 체크한다.
                yield return new WaitForSeconds(0.1f);
            }
        }

        #endregion



        #region Initialize

        public bool Initialize()
        {
            if (isInitialized) return true;

            if (networkManager == null) // networkManager가 없을 때
            {
                networkManager = gameObject.AddComponent<NetworkManager>(); // 컴포넌트를 추가한다.
                networkManager.CreateUserData();
            }
            LooxidLinkData.Instance.Initialize();

            isInitialized = networkManager.Initialize();    // networkManager가 Initialize되면 true 반환
            if (isInitialized)
            {
                StartCoroutine(AutoConnection());
                StartCoroutine(DetectSensor());
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Terminate()
        {
            isInitialized = false;
            if (messageUI != null) {
                if (messageUI.gameObject != null)
                {
                    Destroy(messageUI.gameObject);  // messageUI를 삭제한다.
                }
                messageUI = null;
            }
            networkManager.DisconnectMessage();
        }

        #endregion



        #region Connect & Disconnect
        /// <summary>
        /// LinkCore가 Disconnected 했을 때를 대비하여 상태를 체크하고, 재 연결을 시도한다.
        /// </summary>
        IEnumerator AutoConnection()
        {
            while (true)
            {   // initialize가 되고 linkCoreStatus가 연결되지 않았다면
                if (isInitialized && linkCoreStatus == LinkCoreStatus.Disconnected)
                {
                    networkManager.ClearNetwork();  // Network 초기화
                    networkManager.Connect(OnLinkCoreStatus, OnOnLinkHubStatus);    // 재연결을 시도한다.
                }
                yield return new WaitForSeconds(3.0f);  // 3초마다 한 번씩 체크한다.
            }
        }


        void OnLinkCoreStatus(LinkCoreStatus coreStatus)
        {
            if (!isInitialized) return;

            if (coreStatus == LinkCoreStatus.Connected) // LinkCore가 Connected 됬다면
            {
                HideMessage(LooxidLinkMessageType.CoreDisconnected);
            }
            else if (coreStatus == LinkCoreStatus.Disconnected)
            {
                HideMessage(LooxidLinkMessageType.SensorOff);
                HideMessage(LooxidLinkMessageType.NoiseSignal);
                ShowMessage(LooxidLinkMessageType.CoreDisconnected);
            }
        }
        void OnOnLinkHubStatus(LinkHubStatus hubStatus)
        {
            if (!isInitialized) return;

            if (hubStatus == LinkHubStatus.Connected)
            {
                HideMessage(LooxidLinkMessageType.CoreDisconnected);
            }
            else if (hubStatus == LinkHubStatus.Disconnected)
            {
                HideMessage(LooxidLinkMessageType.SensorOff);
                HideMessage(LooxidLinkMessageType.NoiseSignal);
                ShowMessage(LooxidLinkMessageType.CoreDisconnected);
            }
        }

        #endregion


        #region Settings

        public void SetDebug(bool isDebug)
        {
            LXDebug.isDebug = isDebug;
        }

        public void SetDisplayDisconnectedMessage(bool isDisplay)
        {
            this.isDisplayDisconnectedMessage = isDisplay;

            if( !isDisplay )
            {
                HideMessage(LooxidLinkMessageType.CoreDisconnected);
            }
        }

        public void SetDisplaySensorOffMessage(bool isDisplay)
        {
            this.isDisplaySensorOffMessage = isDisplay;

            if (!isDisplay)
            {
                HideMessage(LooxidLinkMessageType.SensorOff);
            }
        }

        public void SetDisplayNoiseSignalMessage(bool isDisplay)
        {
            this.isDisplayNoiseSignalMessage = isDisplay;

            if (!isDisplay)
            {
                HideMessage(LooxidLinkMessageType.NoiseSignal);
            }
        }

        #endregion


        #region Link Message

        private void InstantiateMessageUI()
        {
            GameObject messageObj = Resources.Load("Prefabs/LooxidLinkMessage") as GameObject;
            if (messageObj != null)
            {
                if (Camera.main == null)
                {
                    Camera mainCamera = GameObject.FindObjectOfType<Camera>();
                    if (mainCamera != null)
                    {
                        GameObject newMessageObj = Instantiate(messageObj, mainCamera.transform) as GameObject;
                        newMessageObj.name = "LooxidLinkMessage";
                        messageUI = newMessageObj.GetComponent<LooxidLinkMessage>();
                    }
                }
                else
                {
                    GameObject newMessageObj = Instantiate(messageObj, Camera.main.transform) as GameObject;
                    newMessageObj.name = "LooxidLinkMessage";
                    messageUI = newMessageObj.GetComponent<LooxidLinkMessage>();
                }
            }
        }

        private void ShowMessage(LooxidLinkMessageType messageType)
        {
            if (messageType == LooxidLinkMessageType.NoiseSignal)
            {
                if (!noiseSignalMessage)
                {
                    networkManager.WriteLog("INFO", "Show MessageUI: Noise Signal");
                    if (OnShowNoiseSignalMessage != null) OnShowNoiseSignalMessage.Invoke();
                }
                noiseSignalMessage = true;
            }
            if (messageType == LooxidLinkMessageType.SensorOff)
            {
                if (!sensorOffMessage)
                {
                    networkManager.WriteLog("INFO", "Show MessageUI: Sensor Off");
                    if (OnShowSensorOffMessage != null) OnShowSensorOffMessage.Invoke();
                }
                sensorOffMessage = true;
            }

            if (messageType == LooxidLinkMessageType.CoreDisconnected || messageType == LooxidLinkMessageType.HubDisconnected)
            {
                if (this.isDisplayDisconnectedMessage)
                {
                    if(messageUI == null)
                    {
                        InstantiateMessageUI();
                    }
                    if(messageUI != null) messageUI.ShowMessage(messageType);
                }
            }
            else if (messageType == LooxidLinkMessageType.SensorOff)
            {
                if (this.isDisplaySensorOffMessage)
                {
                    if (messageUI == null)
                    {
                        InstantiateMessageUI();
                    }
                    if (messageUI != null) messageUI.ShowMessage(messageType);
                }
            }
            else if (messageType == LooxidLinkMessageType.NoiseSignal)
            {
                if (this.isDisplayNoiseSignalMessage)
                {
                    if (messageUI == null)
                    {
                        InstantiateMessageUI();
                    }
                    if (messageUI != null) messageUI.ShowMessage(messageType);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void HideMessage(LooxidLinkMessageType messageType)
        {
            if (messageType == LooxidLinkMessageType.NoiseSignal)
            {
                if (noiseSignalMessage)
                {
                    networkManager.WriteLog("INFO", "Hide MessageUI: Noise Signal");
                    if (OnHideNoiseSignalMessage != null) OnHideNoiseSignalMessage.Invoke();
                }
                noiseSignalMessage = false;
            }
            if (messageType == LooxidLinkMessageType.SensorOff)
            {
                if (sensorOffMessage)
                {
                    networkManager.WriteLog("INFO", "Hide MessageUI: Sensor Off");
                    if (OnHideSensorOffMessage != null) OnHideSensorOffMessage.Invoke();
                }
                sensorOffMessage = false;
            }

            if (messageUI != null)
            {
                messageUI.HideMessage(messageType); // messageUI에 표기하기
            }
        }

        #endregion
    }
}