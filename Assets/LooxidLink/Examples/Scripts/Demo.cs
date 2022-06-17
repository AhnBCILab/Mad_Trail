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
    /*
    public enum Tab2DVisualizer
    {
        SENSOR_STATUS = 0,
        MIND_INDEX = 1,
        FEATURE_INDEX = 2,
        RAW_SIGNAL = 3
    }
    */

    public class Demo : MonoBehaviour
    {
        [Header("Tabs")]
        public Tab2DVisualizer SelectTab = Tab2DVisualizer.SENSOR_STATUS;
        public GameObject[] Tabs;
        public GameObject[] Panels;

        //TriggerData
        public double OneSecondAttnetion;
        public double OneSecondRelaxation;
        public double TenSecondAttnetion;
        public double TenSecondRelaxation;

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

        private LinkDataValue delta;
        private LinkDataValue theta;
        private LinkDataValue alpha;
        private LinkDataValue beta;
        private LinkDataValue gamma;

        private List<string[]> rowData = new List<string[]>();
        private List<string[]> rowRawData = new List<string[]>();
        private List<string[]> rowIndexData = new List<string[]>();

        void Start()
        {
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
            //LooxidLinkData.OnReceiveEEGRawSignals += OnReceiveEEGRawSignals;
            LooxidLinkData.OnReceiveMindIndexes += OnReceiveMindIndexes;
            //LooxidLinkData.OnReceiveEEGFeatureIndexes += OnReceiveEEGFeatureIndexes;
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
            //LooxidLinkData.OnReceiveEEGRawSignals -= OnReceiveEEGRawSignals;
            LooxidLinkData.OnReceiveMindIndexes -= OnReceiveMindIndexes;
            //LooxidLinkData.OnReceiveEEGFeatureIndexes -= OnReceiveEEGFeatureIndexes;
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

                        for(int i = featureIndexList.Count - 1; i >= 0 ; i--) {
                            rowDataTemp = new string[9];
                            rowDataTemp[0] = "" + featureIndexList[i].timestamp;
                            rowDataTemp[1] = "" + featureIndexList[i].Delta(SelectChannel);
                            rowDataTemp[2] = "" + featureIndexList[i].Theta(SelectChannel);
                            rowDataTemp[3] = "" + featureIndexList[i].Alpha(SelectChannel);
                            rowDataTemp[4] = "" + featureIndexList[i].Beta(SelectChannel);
                            rowDataTemp[5] = "" + featureIndexList[i].Gamma(SelectChannel);
                            rowDataTemp[6] = "F";
                            rowDataTemp[7] = "";
                            rowDataTemp[8] = "";

                            rowData.Add(rowDataTemp);
                        }

                        string[][] output = new string[rowData.Count][];

                        for (int i = 0; i  < output.Length; i++) {
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

                        for(int i = rawIndexList.Count - 1; i >= 0; i--) {
                            for(int j = rawIndexList[i].rawSignal.Count - 1; j >= 0; j--){
                                rowDataTemp = new string[11];
                                rowDataTemp[0] = "" + rawIndexList[i].rawSignal[j].timestamp;
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
                                rowRawData.Add(rowDataTemp);
                            }
                        }

                        string[][] output = new string[rowRawData.Count][];

                        for (int i = 0; i  < output.Length; i++) {
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
                            RelaxationSum +=IndexList[i].attention;
                        }
                    }
            IndexList = LooxidLinkData.Instance.GetMindIndexData((float)stime);
            
             if (IndexList.Count > 0)
                    {
                         for(int i = 0; i < IndexList.Count ; i++) {
                            RelaxationSum -=IndexList[i].attention;
                        }
                    }
                   RelaxationSum=RelaxationSum/(double)IndexList.Count;
                    return (RelaxationSum/IndexList.Count);
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

                        string[] rowDataTemp = new string[10];
                        rowDataTemp[0] = "Timestamp";
                        rowDataTemp[1] = "attention";
                        rowDataTemp[2] = "relaxation";
                        rowDataTemp[3] = "asymmetry";
                        rowDataTemp[4] = "leftActivity";
                        rowDataTemp[5] = "rightActivity";
                        rowDataTemp[6] = "Event";
                        rowDataTemp[7] = "Event";
                        rowDataTemp[8] = "EventDate";
                        rowDataTemp[9] = "EventDuration";
                        rowIndexData.Add(rowDataTemp);

                        // double standtime = IndexList[IndexList.Count-1].timestamp -> 이걸로 빼면 기준점이 0이 된다.

                        for(int i = IndexList.Count - 1; i >= 0 ; i--) {
                            rowDataTemp = new string[10];
                            rowDataTemp[0] = "" + IndexList[i].timestamp;
                            rowDataTemp[1] = "" + IndexList[i].attention;
                            rowDataTemp[2] = "" + IndexList[i].relaxation;
                            rowDataTemp[3] = "" + IndexList[i].asymmetry;
                            rowDataTemp[4] = "" + IndexList[i].leftActivity;
                            rowDataTemp[5] = "" + IndexList[i].rightActivity;
                            rowDataTemp[6] = "F";
                            rowDataTemp[7] = "";
                            rowDataTemp[8] = "";
                            rowDataTemp[9] = "";
                            rowIndexData.Add(rowDataTemp);
                        }

                        string[][] output = new string[rowIndexData.Count][];

                        for (int i = 0; i  < output.Length; i++) {
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

        private double start_time = 0f;
        private double end_time = 0f;
        private int count = 0;

        void Update()
        {
            OneSecondAttnetion = AvgAttenttionData(1);
            TenSecondAttnetion = AvgAttenttionData(11,1);
            OneSecondRelaxation = AvgRelaxationData(1);
            TenSecondRelaxation = AvgRelaxationData(11,1);
            if (Input.GetKeyDown(KeyCode.Space)) {
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
    }
}