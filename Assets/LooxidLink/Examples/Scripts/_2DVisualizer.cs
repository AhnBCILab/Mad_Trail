using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System;

namespace Looxid.Link
{
    public enum Tab2DVisualizer
    {
        SENSOR_STATUS = 0,
        MIND_INDEX = 1,
        FEATURE_INDEX = 2,
        RAW_SIGNAL = 3
    }

    public class _2DVisualizer : MonoBehaviour
    {
        [Header("Tabs")]
        public Tab2DVisualizer SelectTab = Tab2DVisualizer.SENSOR_STATUS;
        public GameObject[] Tabs;
        public GameObject[] Panels;
        public triggerManager_KHS triggerManager;

        [SerializeField]
        private float TriggerSIze;

        //TriggerData
        public double OneSecondAttnetion;
        public double OneSecondRelaxation;
        public double TenSecondAttnetion;
        public double TenSecondRelaxation;
        public float AttentionTriggerPercentage;
        public float RelaxationTriggerPercentage;
        public double FearStatus;
        public bool DataAutoSaving;        


        [Header("Message")]
        public bool displayLinkMessage;
        public CanvasGroup DisconnecetdPanel;
        public CanvasGroup SensorOffPanel;
        public CanvasGroup NoiseSignalPanel;

        [Header("Sensor Status")]
        public Image AF3SensorImage;
        public Image AF4SensorImage;
        public Image Fp1SensorImage;
        public Image Fp2SensorImage;
        public Image AF7SensorImage;
        public Image AF8SensorImage;

        [Header("Mind Index")]
        public BarIndicator LeftActivityIndicator;
        public BarIndicator RightActivityIndicator;
        public BarIndicator AttentionIndicator;
        public BarIndicator RelaxationIndicator;

        [Header("Feature Index")]
        public EEGSensorID SelectChannel;
        public BarIndicator DeltaIndicator;
        public BarIndicator ThetaIndicator;
        public BarIndicator AlphaIndicator;
        public BarIndicator BetaIndicator;
        public BarIndicator GammaIndicator;
        public Toggle[] ChannelToggles;

        [Header("Raw Signal")]
        public LineChart AF3Chart;
        public LineChart AF4Chart;
        public LineChart Fp1Chart;
        public LineChart Fp2Chart;
        public LineChart AF7Chart;
        public LineChart AF8Chart;

        private Color32 BackColor = new Color32(255, 255, 255, 255);
        private Color32 TextColor = new Color32(10, 10, 10, 255);

        private float disconnectedWindowAlpha = 0.0f;
        private float disconnectedWindowTargetAlpha = 0.0f;
        private float noiseSingalWindowAlpha = 0.0f;
        private float noiseSingalWindowTargetAlpha = 0.0f;
        private float sensorOffWindowAlpha = 0.0f;
        private float sensorOffWindowTargetAlpha = 0.0f;

        private LooxidLinkMessageType messageType;

        private EEGSensor sensorStatusData;
        private EEGRawSignal rawSignalData;

        private LinkDataValue leftActivity;
        private LinkDataValue rightActivity;
        private LinkDataValue attention;
        private LinkDataValue relaxation;
        public float relax;
        public float atten;

        private LinkDataValue delta;
        private LinkDataValue theta;
        private LinkDataValue alpha;
        private LinkDataValue beta;
        private LinkDataValue gamma;

        private List<string[]> rowData = new List<string[]>();
        private List<string[]> rowRawData = new List<string[]>();
        private List<string[]> rowIndexData = new List<string[]>();
        private GameObject GM;
        private GameObject CV;

        # region 세이브 기능
        private double start_time = 0f;
        private double end_time = 0f;
        private int count = 0;
        private Dictionary<double, double> triggerRawDic;  // timestamp, eventID
        private Dictionary<double, double> triggerFeatureDic;  // timestamp, eventID
        private Dictionary<double, double> triggerIndexDic;  // timestamp, eventID
        private List<double[]> triggerList = new List<double[]>();
        List<double> triggerEventTimeList = new List<double>();
        List<double> triggerEventIDList = new List<double>();
        # endregion

        #region 트리거 기능
        [SerializeField] private float TriggerTimer;
        [SerializeField] private float TriggerDelay;
        [SerializeField] private bool IsTriggerON;
        
        
        #endregion

        void Awake(){
            CV=GameObject.Find("Canvas_BW");
            
        }
        
        void Start()
        {
            GM=GameObject.Find("GM");
            
            LooxidLinkManager.Instance.SetDebug(true);
            LooxidLinkManager.Instance.Initialize();

            LooxidLinkManager.Instance.SetDisplayDisconnectedMessage(displayLinkMessage);
            LooxidLinkManager.Instance.SetDisplayNoiseSignalMessage(displayLinkMessage);
            LooxidLinkManager.Instance.SetDisplaySensorOffMessage(displayLinkMessage);

            leftActivity = new LinkDataValue();
            rightActivity = new LinkDataValue();
            attention = new LinkDataValue();
            relaxation = new LinkDataValue();

            delta = new LinkDataValue();
            theta = new LinkDataValue();
            alpha = new LinkDataValue();
            beta = new LinkDataValue();
            gamma = new LinkDataValue();

            triggerRawDic = new Dictionary<double, double>();   
            triggerFeatureDic = new Dictionary<double, double>();   
            triggerIndexDic = new Dictionary<double, double>();   
            
        }

        void OnEnable()
        {
            LooxidLinkManager.OnLinkCoreConnected += OnLinkCoreConncetd;
            LooxidLinkManager.OnLinkHubConnected += OnLinkHubConnected;
            LooxidLinkManager.OnLinkCoreDisconnected += OnLinkCoreDisconncetd;
            LooxidLinkManager.OnLinkHubDisconnected += OnLinkHubDisconnected;
            LooxidLinkManager.OnShowSensorOffMessage += OnShowSensorOffMessage;
            LooxidLinkManager.OnHideSensorOffMessage += OnHideSensorOffMessage;
            LooxidLinkManager.OnShowNoiseSignalMessage += OnShowNoiseSignalMessage;
            LooxidLinkManager.OnHideNoiseSignalMessage += OnHideNoiseSignalMessage;

            LooxidLinkData.OnReceiveEEGSensorStatus += OnReceiveEEGSensorStatus;
            LooxidLinkData.OnReceiveEEGRawSignals += OnReceiveEEGRawSignals;
            LooxidLinkData.OnReceiveMindIndexes += OnReceiveMindIndexes;
            LooxidLinkData.OnReceiveEEGFeatureIndexes += OnReceiveEEGFeatureIndexes;

            StartCoroutine(DisplayData());

            if(DataAutoSaving){
                SetTab(Tab2DVisualizer.MIND_INDEX);

                // save time 표기
                start_time = Time.time;
                Debug.Log("Save Start");
                triggerManager.writeTriggerEvent(0x01);
                
            }
        }
        void OnDisable()
        {
            LooxidLinkManager.OnLinkCoreConnected -= OnLinkCoreConncetd;
            LooxidLinkManager.OnLinkHubConnected -= OnLinkHubConnected;
            LooxidLinkManager.OnLinkCoreDisconnected -= OnLinkCoreDisconncetd;
            LooxidLinkManager.OnLinkHubDisconnected -= OnLinkHubDisconnected;
            LooxidLinkManager.OnShowSensorOffMessage -= OnShowSensorOffMessage;
            LooxidLinkManager.OnHideSensorOffMessage -= OnHideSensorOffMessage;
            LooxidLinkManager.OnShowNoiseSignalMessage -= OnShowNoiseSignalMessage;
            LooxidLinkManager.OnHideNoiseSignalMessage -= OnHideNoiseSignalMessage;

            LooxidLinkData.OnReceiveEEGSensorStatus -= OnReceiveEEGSensorStatus;
            LooxidLinkData.OnReceiveEEGRawSignals -= OnReceiveEEGRawSignals;
            LooxidLinkData.OnReceiveMindIndexes -= OnReceiveMindIndexes;
            LooxidLinkData.OnReceiveEEGFeatureIndexes -= OnReceiveEEGFeatureIndexes;
        }

        void OnLinkCoreConncetd()
        {
            if( !displayLinkMessage )
            {
                HideMessage(LooxidLinkMessageType.CoreDisconnected);
            }
        }
        void OnLinkHubConnected()
        {
            if (!displayLinkMessage)
            {
                HideMessage(LooxidLinkMessageType.HubDisconnected);
            }
        }
        void OnLinkCoreDisconncetd()
        {
            if (!displayLinkMessage)
            {
                ShowMessage(LooxidLinkMessageType.CoreDisconnected);
            }
        }
        void OnLinkHubDisconnected()
        {
            if (!displayLinkMessage)
            {
                ShowMessage(LooxidLinkMessageType.HubDisconnected);
            }
        }
        void OnShowSensorOffMessage()
        {
            if (!displayLinkMessage)
            {
                ShowMessage(LooxidLinkMessageType.SensorOff);
            }
        }
        void OnHideSensorOffMessage()
        {
            if (!displayLinkMessage)
            {
                HideMessage(LooxidLinkMessageType.SensorOff);
            }
        }
        void OnShowNoiseSignalMessage()
        {
            if (!displayLinkMessage)
            {
                ShowMessage(LooxidLinkMessageType.NoiseSignal);
            }
        }
        void OnHideNoiseSignalMessage()
        {
            if (!displayLinkMessage)
            {
                HideMessage(LooxidLinkMessageType.NoiseSignal);
            }
        }

        // Data Subscription
        void OnReceiveEEGSensorStatus(EEGSensor sensorStatusData)
        {
            this.sensorStatusData = sensorStatusData;
        }
        void OnReceiveMindIndexes(MindIndex mindIndexData)
        {
            leftActivity.target = double.IsNaN(mindIndexData.leftActivity) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.leftActivity);
            rightActivity.target = double.IsNaN(mindIndexData.rightActivity) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.rightActivity);
            attention.target = double.IsNaN(mindIndexData.attention) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.attention);
            relaxation.target = double.IsNaN(mindIndexData.relaxation) ? 0.0f : (float)LooxidLinkUtility.Scale(LooxidLink.MIND_INDEX_SCALE_MIN, LooxidLink.MIND_INDEX_SCALE_MAX, 0.0f, 1.0f, mindIndexData.relaxation);
        }
        // rawSignalData를 가져오도록
        void OnReceiveEEGRawSignals(EEGRawSignal rawSignalData)
        {
            AF3Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF3));
            AF4Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF4));
            Fp1Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.Fp1));
            Fp2Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.Fp2));
            AF7Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF7));
            AF8Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF8));
        }
        void OnReceiveEEGFeatureIndexes(EEGFeatureIndex featureIndexData)
        {
            double deltaValue = featureIndexData.Delta(SelectChannel);
            double thetaValue = featureIndexData.Theta(SelectChannel);
            double alphaValue = featureIndexData.Alpha(SelectChannel);
            double betaValue = featureIndexData.Beta(SelectChannel);
            double gammaValue = featureIndexData.Gamma(SelectChannel);

            delta.target = (double.IsInfinity(deltaValue) || double.IsNaN(deltaValue)) ? 0.0f : LooxidLinkUtility.Scale(delta.min, delta.max, 0.0f, 1.0f, deltaValue);
            theta.target = (double.IsInfinity(thetaValue) || double.IsNaN(thetaValue)) ? 0.0f : LooxidLinkUtility.Scale(theta.min, theta.max, 0.0f, 1.0f, thetaValue);
            alpha.target = (double.IsInfinity(alphaValue) || double.IsNaN(alphaValue)) ? 0.0f : LooxidLinkUtility.Scale(alpha.min, alpha.max, 0.0f, 1.0f, alphaValue);
            beta.target = (double.IsInfinity(betaValue) || double.IsNaN(betaValue)) ? 0.0f : LooxidLinkUtility.Scale(beta.min, beta.max, 0.0f, 1.0f, betaValue);
            gamma.target = (double.IsInfinity(gammaValue) || double.IsNaN(gammaValue)) ? 0.0f : LooxidLinkUtility.Scale(gamma.min, gamma.max, 0.0f, 1.0f, gammaValue);
        }

        void SaveData(double time)
        {

                    List<EEGFeatureIndex> featureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData((float)time);
                    Debug.Log("Save Featured Data....!!");
                    
                    if (featureIndexList.Count > 0)
                    {
                        List<double> deltaScaleDataList = new List<double>();
                        List<double> thetaScaleDataList = new List<double>();
                        List<double> alphaScaleDataList = new List<double>();
                        List<double> betaScaleDataList = new List<double>();
                        List<double> gammaScaleDataList = new List<double>();

                        /*
                        for (int i = 0; i < featureIndexList.Count; i++)
                        {
                            double deltaValue = featureIndexList[i].Delta(SelectChannel);
                            double thetaValue = featureIndexList[i].Theta(SelectChannel);
                            double alphaValue = featureIndexList[i].Alpha(SelectChannel);
                            double betaValue = featureIndexList[i].Beta(SelectChannel);
                            double gammaValue = featureIndexList[i].Gamma(SelectChannel);

                            
                            if ((double.IsInfinity(deltaValue)) && double.IsNaN(deltaValue)) || (double.IsInfinity(thetaValue) && double.IsNaN(thetaValue)) || (double.IsInfinity(alphaValue) && double.IsNaN(alphaValue)) || (double.IsInfinity(betaValue) && double.IsNaN(betaValue)) || (double.IsInfinity(gammaValue) && double.IsNaN(gammaValue)) 
                                continue();
                            
                            if (!double.IsInfinity(deltaValue) && !double.IsNaN(deltaValue)) deltaScaleDataList.Add(deltaValue);
                            if (!double.IsInfinity(thetaValue) && !double.IsNaN(thetaValue)) thetaScaleDataList.Add(thetaValue);
                            if (!double.IsInfinity(alphaValue) && !double.IsNaN(alphaValue)) alphaScaleDataList.Add(alphaValue);
                            if (!double.IsInfinity(betaValue) && !double.IsNaN(betaValue)) betaScaleDataList.Add(betaValue);
                            if (!double.IsInfinity(gammaValue) && !double.IsNaN(gammaValue)) gammaScaleDataList.Add(gammaValue);
                            
                        }
                        */

                        string[] rowDataTemp = new string[9];
                        rowDataTemp[0] = "Timestamp";
                        rowDataTemp[1] = "delta";
                        rowDataTemp[2] = "theta";
                        rowDataTemp[3] = "alpha";
                        rowDataTemp[4] = "beta";
                        rowDataTemp[5] = "gamma";
                        rowDataTemp[6] = "Event";
                        rowDataTemp[7] = "EventDate";
                        rowDataTemp[8] = "EventDuration";
                        rowData.Add(rowDataTemp);

                        double startTime = featureIndexList[featureIndexList.Count - 1].timestamp;

                        for(int i = featureIndexList.Count - 1; i >= 0 ; i--) {
                            rowDataTemp = new string[9];
                            rowDataTemp[0] = "" + (featureIndexList[i].timestamp - startTime);
                            rowDataTemp[1] = "" + featureIndexList[i].Delta(SelectChannel);
                            rowDataTemp[2] = "" + featureIndexList[i].Theta(SelectChannel);
                            rowDataTemp[3] = "" + featureIndexList[i].Alpha(SelectChannel);
                            rowDataTemp[4] = "" + featureIndexList[i].Beta(SelectChannel);
                            rowDataTemp[5] = "" + featureIndexList[i].Gamma(SelectChannel);
                            rowDataTemp[6] = "F";
                            rowDataTemp[7] = "";
                            rowDataTemp[8] = "";

                            if (triggerFeatureDic.ContainsKey(featureIndexList[i].timestamp)){
                                    rowDataTemp[8] = "" + triggerFeatureDic[featureIndexList[i].timestamp];
                                }

                            rowData.Add(rowDataTemp);
                        }

                        string[][] output = new string[rowData.Count][];

                        for (int i = 0; i < output.Length ; i++) {
                            output[i] = rowData[i];
                        }

                        int length = output.GetLength(0);
                        string delimiter = ",";

                        StringBuilder sb = new StringBuilder();

                        for (int index = 0; index < length; index++){
                            sb.AppendLine(string.Join(delimiter, output[index]));
                        }

                        string filePath = getPath();

                        StreamWriter outStream = System.IO.File.CreateText(filePath);
                        outStream.WriteLine(sb);
                        outStream.Close();
                    }

        }

            // AF3Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF3));
            // AF4Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF4));
            // Fp1Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.Fp1));
            // Fp2Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.Fp2));
            // AF7Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF7));
            // AF8Chart.SetValue(rawSignalData.FilteredRawSignal(EEGSensorID.AF8));

        

        void SaveRawData(double time)
        {

                    List<EEGRawSignal> rawIndexList = LooxidLinkData.Instance.GetEEGRawSignalData((float)time);
                    Debug.Log("Save Raw Data....!!");
                    
                    if (rawIndexList.Count > 0)
                    {
                        List<double> deltaScaleDataList = new List<double>();
                        List<double> thetaScaleDataList = new List<double>();
                        List<double> alphaScaleDataList = new List<double>();
                        List<double> betaScaleDataList = new List<double>();
                        List<double> gammaScaleDataList = new List<double>();

                        string[] rowDataTemp = new string[11];
                        rowDataTemp[0] = "Timestamp";
                        rowDataTemp[1] = "Seq";
                        rowDataTemp[2] = "AF3";
                        rowDataTemp[3] = "AF4";
                        rowDataTemp[4] = "Fp1";
                        rowDataTemp[5] = "Fp2";
                        rowDataTemp[6] = "AF7";
                        rowDataTemp[7] = "AF8";
                        rowDataTemp[8] = "Event";
                        rowDataTemp[9] = "EventDate";
                        rowDataTemp[10] = "EventDuration";
                        rowRawData.Add(rowDataTemp);

                        double startTime = rawIndexList[rawIndexList.Count - 1].rawSignal[0].timestamp;

                        for(int i = rawIndexList.Count - 1; i >= 0; i--) {
                            for(int j = 0; j < rawIndexList[i].rawSignal.Count; j++){
                                rowDataTemp = new string[11];
                                rowDataTemp[0] = "" + (rawIndexList[i].rawSignal[j].timestamp - startTime);
                                rowDataTemp[1] = "" + rawIndexList[i].rawSignal[j].seq_num;
                                rowDataTemp[2] = "" + rawIndexList[i].rawSignal[j].ch_data[0];
                                rowDataTemp[3] = "" + rawIndexList[i].rawSignal[j].ch_data[1];
                                rowDataTemp[4] = "" + rawIndexList[i].rawSignal[j].ch_data[2];
                                rowDataTemp[5] = "" + rawIndexList[i].rawSignal[j].ch_data[3];
                                rowDataTemp[6] = "" + rawIndexList[i].rawSignal[j].ch_data[4];
                                rowDataTemp[7] = "" + rawIndexList[i].rawSignal[j].ch_data[5];
                                rowDataTemp[8] = "";
                                rowDataTemp[9] = "";
                                rowDataTemp[10] = "";

                                if (triggerRawDic.ContainsKey(rawIndexList[i].rawSignal[j].timestamp)){
                                    rowDataTemp[8] = "" + triggerRawDic[rawIndexList[i].rawSignal[j].timestamp];
                                }

                                rowRawData.Add(rowDataTemp);
                            }
                        }

                        string[][] output = new string[rowRawData.Count][];

                        for (int i = 0; i < output.Length ; i++) {
                            output[i] = rowRawData[i];
                        }

                        int length = output.GetLength(0);
                        string delimiter = ",";

                        StringBuilder sb = new StringBuilder();

                        for (int index = 0; index < length; index++){
                            sb.AppendLine(string.Join(delimiter, output[index]));
                        }

                        string filePath = getRawPath();

                        StreamWriter outStream = System.IO.File.CreateText(filePath);
                        outStream.WriteLine(sb);
                        outStream.Close();
                    }
                    
        }
        
 
        double AvgAttenttionData(double time){
            List<MindIndex> IndexList = LooxidLinkData.Instance.GetMindIndexData((float)time);
            double AttentionSum=0;
            if (IndexList.Count > 0)
                    {
                         for(int i = 0; i < IndexList.Count ; i++) {
                            AttentionSum +=IndexList[i].attention;
                        }
                    }
                   AttentionSum=AttentionSum/(double)IndexList.Count;
                    return (AttentionSum/IndexList.Count);
        }
        double AvgAttenttionData(double stime,double etime){
            List<MindIndex> IndexList = LooxidLinkData.Instance.GetMindIndexData((float)etime);
            double AttentionSum=0;
            if (IndexList.Count > 0)
                    {
                         for(int i = 0; i < IndexList.Count ; i++) {
                            AttentionSum +=IndexList[i].attention;
                        }
                    }
            IndexList = LooxidLinkData.Instance.GetMindIndexData((float)stime);
            
             if (IndexList.Count > 0)
                    {
                         for(int i = 0; i < IndexList.Count ; i++) {
                            AttentionSum -=IndexList[i].attention;
                        }
                    }
                   AttentionSum=AttentionSum/(double)IndexList.Count;
                    return (AttentionSum/IndexList.Count);
        }

        double AvgRelaxationData(double time){
            List<MindIndex> IndexList = LooxidLinkData.Instance.GetMindIndexData((float)time);
            double RelaxationSum=0;
            if (IndexList.Count > 0)
                    {
                         for(int i = 0; i < IndexList.Count ; i++) {
                            RelaxationSum +=IndexList[i].relaxation;
                        }
                    }
                   RelaxationSum=RelaxationSum/(double)IndexList.Count;
                    return (RelaxationSum/IndexList.Count);
        }
        double AvgRelaxationData(double stime,double etime){
            List<MindIndex> IndexList = LooxidLinkData.Instance.GetMindIndexData((float)etime);
            double RelaxationSum=0;
            if (IndexList.Count > 0)
                    {
                         for(int i = 0; i < IndexList.Count ; i++) {
                            RelaxationSum +=IndexList[i].relaxation;
                        }
                    }
            IndexList = LooxidLinkData.Instance.GetMindIndexData((float)stime);
            
             if (IndexList.Count > 0)
                    {
                         for(int i = 0; i < IndexList.Count ; i++) {
                            RelaxationSum -=IndexList[i].relaxation;
                        }
                    }
                   RelaxationSum=RelaxationSum/(double)IndexList.Count;
                    return (RelaxationSum/IndexList.Count);
        }


        double AvgAlphaPerBetaData(double time){
            List<EEGFeatureIndex> FeatureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData((float)time);
            double AlphaSum=0;
            double BetaSum = 0;
            double AlphaPerBeta=0;
            if (FeatureIndexList.Count > 0)
                    {
                         for(int i = 0; i < FeatureIndexList.Count ; i++) {
                            AlphaSum += FeatureIndexList[i].Alpha(SelectChannel);   // SelectedChannel 설정될 수 있다.
                            BetaSum += FeatureIndexList[i].Beta(SelectChannel);
                        }
                    }
                   AlphaPerBeta=AlphaSum/BetaSum;
                    return AlphaPerBeta;
        }
        double AvgAlphaPerBetaData(double stime,double etime){
            List<EEGFeatureIndex> FeatureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData((float)etime);
            double AlphaSum=0;
            double BetaSum = 0;
            double AlphaPerBeta=0;
            if (FeatureIndexList.Count > 0)
                    {
                         for(int i = 0; i < FeatureIndexList.Count ; i++) {
                            AlphaSum += FeatureIndexList[i].Alpha(SelectChannel);
                            BetaSum += FeatureIndexList[i].Beta(SelectChannel);
                        }
                    }
            FeatureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData((float)stime);
            
             if (FeatureIndexList.Count > 0)
                    {
                         for(int i = 0; i < FeatureIndexList.Count ; i++) {
                            AlphaSum -= FeatureIndexList[i].Alpha(SelectChannel);
                            BetaSum -= FeatureIndexList[i].Beta(SelectChannel);
                        }
                    }
                   AlphaPerBeta=AlphaSum/BetaSum;
                    return AlphaPerBeta;
        }

        double AvgFearData(double time){
            List<EEGFeatureIndex> FeatureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData((float)time);
            double AlphaSum=0;
            double BetaSum = 0;
            double GammaSum = 0;
            double Fear=0;
            if (FeatureIndexList.Count > 0)
                    {
                         for(int i = 0; i < FeatureIndexList.Count ; i++) {
                            AlphaSum += FeatureIndexList[i].Alpha(SelectChannel);   // SelectedChannel 설정될 수 있다.
                            BetaSum += FeatureIndexList[i].Beta(SelectChannel);
                            GammaSum += FeatureIndexList[i].Gamma(SelectChannel);
                            }
                    }
                   Fear=AlphaSum/( AlphaSum + BetaSum + GammaSum);
                    return Fear;
        }
        double AvgFearData(double stime,double etime){
            List<EEGFeatureIndex> FeatureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData((float)etime);
            double AlphaSum=0;
            double BetaSum = 0;
            double GammaSum = 0;
            double Fear=0;
            if (FeatureIndexList.Count > 0)
                    {
                         for(int i = 0; i < FeatureIndexList.Count ; i++) {
                            AlphaSum += FeatureIndexList[i].Alpha(SelectChannel);
                            BetaSum += FeatureIndexList[i].Beta(SelectChannel);
                            GammaSum += FeatureIndexList[i].Gamma(SelectChannel);
                        }
                    }
            FeatureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData((float)stime);
            
             if (FeatureIndexList.Count > 0)
                    {
                         for(int i = 0; i < FeatureIndexList.Count ; i++) {
                            AlphaSum -= FeatureIndexList[i].Alpha(SelectChannel);
                            BetaSum -= FeatureIndexList[i].Beta(SelectChannel);
                            GammaSum -= FeatureIndexList[i].Gamma(SelectChannel);
                        }
                    }
                   Fear=AlphaSum/(AlphaSum + BetaSum + GammaSum);
                    return Fear;
        }
        void SaveIndexData(double time)
        {

                    List<MindIndex> IndexList = LooxidLinkData.Instance.GetMindIndexData((float)time);
                    Debug.Log("Save MindIndex Data....!!");
                    
                    if (IndexList.Count > 0)
                    {
                        List<double> deltaScaleDataList = new List<double>();
                        List<double> thetaScaleDataList = new List<double>();
                        List<double> alphaScaleDataList = new List<double>();
                        List<double> betaScaleDataList = new List<double>();
                        List<double> gammaScaleDataList = new List<double>();

                        /*
                        for (int i = 0; i < featureIndexList.Count; i++)
                        {
                            double deltaValue = featureIndexList[i].Delta(SelectChannel);
                            double thetaValue = featureIndexList[i].Theta(SelectChannel);
                            double alphaValue = featureIndexList[i].Alpha(SelectChannel);
                            double betaValue = featureIndexList[i].Beta(SelectChannel);
                            double gammaValue = featureIndexList[i].Gamma(SelectChannel);

                            
                            if ((double.IsInfinity(deltaValue)) && double.IsNaN(deltaValue)) || (double.IsInfinity(thetaValue) && double.IsNaN(thetaValue)) || (double.IsInfinity(alphaValue) && double.IsNaN(alphaValue)) || (double.IsInfinity(betaValue) && double.IsNaN(betaValue)) || (double.IsInfinity(gammaValue) && double.IsNaN(gammaValue)) 
                                continue();
                            
                            if (!double.IsInfinity(deltaValue) && !double.IsNaN(deltaValue)) deltaScaleDataList.Add(deltaValue);
                            if (!double.IsInfinity(thetaValue) && !double.IsNaN(thetaValue)) thetaScaleDataList.Add(thetaValue);
                            if (!double.IsInfinity(alphaValue) && !double.IsNaN(alphaValue)) alphaScaleDataList.Add(alphaValue);
                            if (!double.IsInfinity(betaValue) && !double.IsNaN(betaValue)) betaScaleDataList.Add(betaValue);
                            if (!double.IsInfinity(gammaValue) && !double.IsNaN(gammaValue)) gammaScaleDataList.Add(gammaValue);
                            
                        }
                        */

                        string[] rowDataTemp = new string[9];
                        rowDataTemp[0] = "Timestamp";
                        rowDataTemp[1] = "attention";
                        rowDataTemp[2] = "relaxation";
                        rowDataTemp[3] = "asymmetry";
                        rowDataTemp[4] = "leftActivity";
                        rowDataTemp[5] = "rightActivity";
                        rowDataTemp[6] = "Event";
                        rowDataTemp[7] = "EventDate";
                        rowDataTemp[8] = "EventDuration";
                        rowIndexData.Add(rowDataTemp);

                        // double standtime = IndexList[IndexList.Count-1].timestamp -> 이걸로 빼면 기준점이 0이 된다.
                        double startTime = IndexList[IndexList.Count - 1].timestamp;


                        for(int i = IndexList.Count - 1; i >= 0 ; i--) {
                            rowDataTemp = new string[9];
                            rowDataTemp[0] = "" + (IndexList[i].timestamp - startTime);
                            rowDataTemp[1] = "" + IndexList[i].attention;
                            rowDataTemp[2] = "" + IndexList[i].relaxation;
                            rowDataTemp[3] = "" + IndexList[i].asymmetry;
                            rowDataTemp[4] = "" + IndexList[i].leftActivity;
                            rowDataTemp[5] = "" + IndexList[i].rightActivity;
                            rowDataTemp[6] = "F";
                            rowDataTemp[7] = "";
                            rowDataTemp[8] = "";

                            if (triggerFeatureDic.ContainsKey(IndexList[i].timestamp)){
                                    rowDataTemp[6] = "" + triggerIndexDic[IndexList[i].timestamp];
                                }

                            rowIndexData.Add(rowDataTemp);
                        }

                        string[][] output = new string[rowIndexData.Count][];

                        for (int i = 0; i < output.Length ; i++) {
                            output[i] = rowIndexData[i];
                        }

                        int length = output.GetLength(0);
                        string delimiter = ",";

                        StringBuilder sb = new StringBuilder();

                        for (int index = 0; index < length; index++){
                            sb.AppendLine(string.Join(delimiter, output[index]));
                        }

                        string filePath = getIndexPath();

                        StreamWriter outStream = System.IO.File.CreateText(filePath);
                        outStream.WriteLine(sb);
                        outStream.Close();
                    }
                    
        }
        


        private string getPath() {
            #if UNITY_EDITOR
            return Application.dataPath + "/CSV/FEATURE/" +  Time.time +"_Saved_data.csv";
            #else
            return Application.dataPath +"/" + "Saved_data.csv";
            #endif
        }
        private string getRawPath() {
            #if UNITY_EDITOR
            return Application.dataPath + "/CSV/RAW/" + Time.time +"_Saved_raw_data.csv";
            #else
            return Application.dataPath +"/" + "Saved_data.csv";
            #endif
        }
        private string getIndexPath() {
            #if UNITY_EDITOR
            return Application.dataPath + "/CSV/MINDINDEX/" + Time.time +"_Saved_index_data.csv";
            #else
            return Application.dataPath +"/" + "Saved_data.csv";
            #endif
        }
        private string getDemoPath() {
            #if UNITY_EDITOR
            // 파일을 찾는다.
            String path = Application.dataPath + "/CSV/RAW/";
            String file = "_Saved_raw_data";
            String extension = ".csv";
            int count = 0;

            while(File.Exists(path + file + count + extension)) {
                count++;
            }

            String filename = path + file + count + extension;

            return filename;
            #else
            return Application.dataPath +"/" + "Saved_data.csv";
            #endif
        }

        IEnumerator DisplayData()
        {
            while (this.gameObject.activeSelf)
            {
                yield return new WaitForSeconds(0.1f);

                TenSecondAttnetion = AvgAttenttionData(11, 1);
                TenSecondRelaxation = AvgRelaxationData(11, 1);
                
                if(TenSecondAttnetion>TriggerSIze)
                TenSecondAttnetion=TriggerSIze;
                else if(TenSecondAttnetion<-1*TriggerSIze)
                TenSecondAttnetion=-1*TriggerSIze;
                if(TenSecondRelaxation>TriggerSIze)
                TenSecondRelaxation=TriggerSIze;
                else if(TenSecondRelaxation<-1*TriggerSIze)
                TenSecondRelaxation=-1*TriggerSIze;
                OneSecondAttnetion = AvgAttenttionData(1);
                
                OneSecondRelaxation = AvgRelaxationData(1);
                if(OneSecondAttnetion>TriggerSIze)
                OneSecondAttnetion=TriggerSIze;
                else if(OneSecondAttnetion<-1*TriggerSIze)
                OneSecondAttnetion=-1*TriggerSIze;
                if(OneSecondRelaxation>TriggerSIze)
                OneSecondRelaxation=TriggerSIze;
                else if(OneSecondRelaxation<-1*TriggerSIze)
                OneSecondRelaxation=-1*TriggerSIze;
                
                /*
                if (Mathf.Lerp((float)TenSecondAttnetion,(float)1.9,AttentionTriggerPercentage) < (float)OneSecondAttnetion) {
                    GM.GetComponent<GameManager>().AttentionTriggerSetter();
                    Debug.Log((float)TenSecondAttnetion +"\n"+(float)OneSecondAttnetion);
                    TriggerInsert(1);
                }*/

                if (!GM.GetComponent<GameManager>().isDefenseGetter()){
                    if ( !IsTriggerON &&  atten > AttentionTriggerPercentage)//Mathf.Lerp((float)TenSecondRelaxation,(float)1.9,RelaxationTriggerPercentage) < (float)OneSecondRelaxation ) 
                    {
                        GM.GetComponent<GameManager>().RelaxationTriggerSetter();
                        TriggerInsert(1);
                        IsTriggerON = true;
                    }
                }
                /*
                if ( !IsTriggerON &&  relax > RelaxationTriggerPercentage)//Mathf.Lerp((float)TenSecondRelaxation,(float)1.9,RelaxationTriggerPercentage) < (float)OneSecondRelaxation ) 
                {
                    GM.GetComponent<GameManager>().RelaxationTriggerSetter();
                    Debug.Log((float)TenSecondRelaxation +"\n"+(float)OneSecondRelaxation);
                    TriggerInsert(2);
                    IsTriggerON = true;
                }
                */

                
                FearStatus = AvgFearData(1);

                //if (CV.gameObject.activeSelf) {
                    if (this.SelectTab == Tab2DVisualizer.SENSOR_STATUS)
                    {
                        if (sensorStatusData != null)
                        {
                            Color32 offColor = new Color32(64, 64, 64, 255);

                            AF3SensorImage.color = sensorStatusData.IsSensorOn(EEGSensorID.AF3) ? (Color)LooxidLinkManager.linkColor : (Color)offColor;
                            AF4SensorImage.color = sensorStatusData.IsSensorOn(EEGSensorID.AF4) ? (Color)LooxidLinkManager.linkColor : (Color)offColor;
                            Fp1SensorImage.color = sensorStatusData.IsSensorOn(EEGSensorID.Fp1) ? (Color)LooxidLinkManager.linkColor : (Color)offColor;
                            Fp2SensorImage.color = sensorStatusData.IsSensorOn(EEGSensorID.Fp2) ? (Color)LooxidLinkManager.linkColor : (Color)offColor;
                            AF7SensorImage.color = sensorStatusData.IsSensorOn(EEGSensorID.AF7) ? (Color)LooxidLinkManager.linkColor : (Color)offColor;
                            AF8SensorImage.color = sensorStatusData.IsSensorOn(EEGSensorID.AF8) ? (Color)LooxidLinkManager.linkColor : (Color)offColor;
                        }
                    }
                    else if (this.SelectTab == Tab2DVisualizer.MIND_INDEX)
                    {
                        LeftActivityIndicator.SetValue((float)leftActivity.value);
                        RightActivityIndicator.SetValue((float)rightActivity.value);
                        AttentionIndicator.SetValue((float)attention.value);
                        RelaxationIndicator.SetValue((float)relaxation.value);
                    }
                    else if (this.SelectTab == Tab2DVisualizer.FEATURE_INDEX)
                    {
                        List<EEGFeatureIndex> featureIndexList = LooxidLinkData.Instance.GetEEGFeatureIndexData(10.0f);

                        if (featureIndexList.Count > 0)
                        {
                            List<double> deltaScaleDataList = new List<double>();
                            List<double> thetaScaleDataList = new List<double>();
                            List<double> alphaScaleDataList = new List<double>();
                            List<double> betaScaleDataList = new List<double>();
                            List<double> gammaScaleDataList = new List<double>();

                            for (int i = 0; i < featureIndexList.Count; i++)
                            {
                                double deltaValue = featureIndexList[i].Delta(SelectChannel);
                                double thetaValue = featureIndexList[i].Theta(SelectChannel);
                                double alphaValue = featureIndexList[i].Alpha(SelectChannel);
                                double betaValue = featureIndexList[i].Beta(SelectChannel);
                                double gammaValue = featureIndexList[i].Gamma(SelectChannel);

                                if (!double.IsInfinity(deltaValue) && !double.IsNaN(deltaValue)) deltaScaleDataList.Add(deltaValue);
                                if (!double.IsInfinity(thetaValue) && !double.IsNaN(thetaValue)) thetaScaleDataList.Add(thetaValue);
                                if (!double.IsInfinity(alphaValue) && !double.IsNaN(alphaValue)) alphaScaleDataList.Add(alphaValue);
                                if (!double.IsInfinity(betaValue) && !double.IsNaN(betaValue)) betaScaleDataList.Add(betaValue);
                                if (!double.IsInfinity(gammaValue) && !double.IsNaN(gammaValue)) gammaScaleDataList.Add(gammaValue);
                            }

                            delta.SetScale(deltaScaleDataList);
                            theta.SetScale(thetaScaleDataList);
                            alpha.SetScale(alphaScaleDataList);
                            beta.SetScale(betaScaleDataList);
                            gamma.SetScale(gammaScaleDataList);
                        }

                        DeltaIndicator.SetValue((float)delta.value);
                        ThetaIndicator.SetValue((float)theta.value);
                        AlphaIndicator.SetValue((float)alpha.value);
                        BetaIndicator.SetValue((float)beta.value);
                        GammaIndicator.SetValue((float)gamma.value);
                        // 그대로 list로 저장->엑셀파일에 쓰기
                    }

                   // }
            }
        }

        public void OnTabClick(int numTab)
        {
            SetTab((Tab2DVisualizer)numTab);
        }
        public void OnTabHoverEnter(int numTab)
        {
            Image TabImage = Tabs[numTab].GetComponent<Image>();
            if (TabImage != null) TabImage.color = (Color)LooxidLinkManager.linkColor;

            Text TabText = Tabs[numTab].GetComponent<Text>();
            if (TabText != null) TabText.color = TextColor;

            Tabs[numTab].SendMessage("SetNormalColor", (Color)LooxidLinkManager.linkColor);
            Tabs[numTab].SendMessage("SetTextNormalColor", (Color)BackColor);
        }
        public void OnTabHoverExit(int numTab)
        {
            Image TabImage = Tabs[numTab].GetComponent<Image>();
            if (TabImage != null) TabImage.color = ((Tab2DVisualizer)numTab == SelectTab) ? (Color)LooxidLinkManager.linkColor : (Color)BackColor;

            Text TabText = Tabs[numTab].GetComponent<Text>();
            if (TabText != null) TabText.color = ((Tab2DVisualizer)numTab == SelectTab) ? BackColor : TextColor;

            Tabs[numTab].SendMessage("SetNormalColor", ((Tab2DVisualizer)numTab == SelectTab) ? (Color)LooxidLinkManager.linkColor : (Color)BackColor);
            Tabs[numTab].SendMessage("SetTextNormalColor", ((Tab2DVisualizer)numTab == SelectTab) ? (Color)BackColor : (Color)TextColor);
        }
        public void OnClickLeftButton()
        {
            Tab2DVisualizer nowTab = SelectTab;
            if (nowTab > 0) nowTab--;
            else nowTab = (Tab2DVisualizer)System.Enum.GetValues(typeof(Tab2DVisualizer)).Length - 1;
            SetTab(nowTab);
        }
        public void OnClickRightButton()
        {
            Tab2DVisualizer nowTab = SelectTab;
            if (nowTab < (Tab2DVisualizer)System.Enum.GetValues(typeof(Tab2DVisualizer)).Length - 1) nowTab++;
            else nowTab = 0;
            SetTab(nowTab);
        }
        void SetTab(Tab2DVisualizer tab)
        {
            this.SelectTab = tab;
            if (Tabs != null)
            {
                for (int i = 0; i < Tabs.Length; i++)
                {
                    Image TabImage = Tabs[i].GetComponent<Image>();
                    if (TabImage != null) TabImage.color = ((Tab2DVisualizer)i == SelectTab) ? (Color)LooxidLinkManager.linkColor : (Color)BackColor;

                    Text TabText = Tabs[i].GetComponent<Text>();
                    if (TabText != null) TabText.color = ((Tab2DVisualizer)i == SelectTab) ? (Color)BackColor : (Color)TextColor;

                    Tabs[i].SendMessage("SetNormalColor", ((Tab2DVisualizer)i == SelectTab) ? (Color)LooxidLinkManager.linkColor : (Color)BackColor);
                    Tabs[i].SendMessage("SetTextNormalColor", ((Tab2DVisualizer)i == SelectTab) ? (Color)BackColor : (Color)TextColor);
                }
            }
        }



        void OnApplicationQuit()
        {
            TriggerInsert(0x02);    // trigger event 'end'

            if(DataAutoSaving){
                Debug.Log("Save Finish");
                end_time = Time.time;
                SaveData(end_time - start_time);
                SaveRawData(end_time - start_time);
                SaveIndexData(end_time - start_time);
                Debug.Log("Application ending after " + Time.time + " seconds");
            }
        }

        void Update()
        {
            if (Panels != null)
            {
                for (int i = 0; i < Panels.Length; i++)
                {
                    Panels[i].SetActive((Tab2DVisualizer)i == SelectTab);
                }
            }
            if (ChannelToggles != null)
            {
                for (int i = 0; i < ChannelToggles.Length; i++)
                {
                    ChannelToggles[i].isOn = ((EEGSensorID)i == SelectChannel);
                }
            }
            /*
            if (IsTriggerON) {
                GM.GetComponent<GameManager>().RelaxationTriggerSetter(); 
                IsTriggerON = false;
            }*/

            // 저장 기능
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                // start 
                count++;

                if (count % 2 == 1)  {
                    start_time = Time.time;
                    Debug.Log("Save Start");
                } else {
                    Debug.Log("Save Finish");
                    end_time = Time.time;
                    SaveData(end_time - start_time);
                    SaveRawData(end_time - start_time);
                    SaveIndexData(end_time - start_time);
                }
            }

            // trigger 기능
            if (Input.GetKeyDown(KeyCode.Alpha2)) {    // Fear.!!
                Debug.Log("trigger start");
                triggerRawDic[LooxidLinkData.Instance.GetRawTimestamp()] = 3005f;
                /*
                double[] tempTrigger = new double[2];   // Timestamp, EventId
                tempTrigger[0] = LooxidLinkData.Instance.GetRawTimestamp();
                tempTrigger[1] = 3005;
                triggerList.Add(tempTrigger);
                */
            }

            /*

            if (AvgAlphaPerBetaData(11, 1) > 0.7f) {    // Alpha High
                Debug.Log("alpha per beta trigger start");
                LooxidLinkData.Instance.GetRawTimestamp();                
            }
            */

            

            disconnectedWindowAlpha = Mathf.Lerp(disconnectedWindowAlpha, disconnectedWindowTargetAlpha, 0.2f);
            noiseSingalWindowAlpha = Mathf.Lerp(noiseSingalWindowAlpha, noiseSingalWindowTargetAlpha, 0.2f);
            sensorOffWindowAlpha = Mathf.Lerp(sensorOffWindowAlpha, sensorOffWindowTargetAlpha, 0.2f);

            if (DisconnecetdPanel != null) DisconnecetdPanel.alpha = disconnectedWindowAlpha;
            if (NoiseSignalPanel != null) NoiseSignalPanel.alpha = noiseSingalWindowAlpha;
            if (SensorOffPanel != null) SensorOffPanel.alpha = sensorOffWindowAlpha;

            leftActivity.value = Mathf.Lerp((float)leftActivity.value, (float)leftActivity.target, 0.2f);
            rightActivity.value = Mathf.Lerp((float)rightActivity.value, (float)rightActivity.target, 0.2f);
            attention.value = Mathf.Lerp((float)attention.value, (float)attention.target, 0.2f);
            relaxation.value = Mathf.Lerp((float)relaxation.value, (float)relaxation.target, 0.2f);

            delta.value = Mathf.Lerp((float)delta.value, (float)delta.target, 0.2f);
            theta.value = Mathf.Lerp((float)theta.value, (float)theta.target, 0.2f);
            alpha.value = Mathf.Lerp((float)alpha.value, (float)alpha.target, 0.2f);
            beta.value = Mathf.Lerp((float)beta.value, (float)beta.target, 0.2f);
            gamma.value = Mathf.Lerp((float)gamma.value, (float)gamma.target, 0.2f);

            // Debug.Log("Attention : " + attention.value);
            // Debug.Log("Relaxation : "+ relaxation.value);
            relax = (float)relaxation.value;
            atten = (float)LooxidLinkData.Instance.GetMindAttentionData();// (float)attention.value;

            if (!GM.GetComponent<GameManager>().isDefenseGetter()){
                if (IsTriggerON)    // After Trigger Activating, TriggerTimer get started.
                {
                        TriggerTimer += Time.deltaTime;
                
                    if (TriggerTimer > TriggerDelay) {
                        TriggerTimer = 0f;
                        IsTriggerON = false;
                    }
                }
             }
              
        }

        public void OnSelectChannel(int num)
        {
            this.SelectChannel = (EEGSensorID)num;
        }

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
                if (disconnectedWindowAlpha > 0.0f)
                {
                    disconnectedWindowTargetAlpha = -0.02f;
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

        public void TriggerInsert(byte eventID){
            Debug.Log("trigger start");
            triggerRawDic[LooxidLinkData.Instance.GetRawTimestamp()] = eventID;
            triggerFeatureDic[LooxidLinkData.Instance.GetFeatureTimestamp()] = eventID;
            triggerIndexDic[LooxidLinkData.Instance.GetIndexTimestamp()] = eventID;
            triggerManager.writeTriggerEvent(eventID);
        }

        public void TriggerInsert(){
            Debug.Log("trigger start");
            triggerRawDic[LooxidLinkData.Instance.GetRawTimestamp()] = 3005f;
            triggerFeatureDic[LooxidLinkData.Instance.GetFeatureTimestamp()] = 3005f;
            triggerIndexDic[LooxidLinkData.Instance.GetIndexTimestamp()] = 3005f;
            triggerManager.writeTriggerEvent();
        }

    }
}