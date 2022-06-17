using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

namespace Looxid.Link
{
    public class LooxidLinkProcess
    {
        // This is after the Awake method has been invoked.
        [RuntimeInitializeOnLoadMethod] 
        static void RuntimeInitWrapper()
        {
            #if !UNITY_EDITOR
                OnInit();
            #endif
        }

        #if UNITY_EDITOR
        // method attribute, compile, playmode 진입시 Awake 호출 이전에 실행
        [UnityEditor.InitializeOnLoadMethod]
        #endif
        static void OnInit()
        {
            string roamingFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);  // 사용자 애플리케이션 관련 데이터 대한 공용 리포지토리로 사용되는 디렉터리
            string looxidlabsFolderPath = "looxidlabs";
            string looxidlinkFolderPath = "Looxid Link Core";
            string appLocationFileName = "app_location.txt";

            string roamingFile = roamingFolderPath + "\\" + looxidlabsFolderPath + "\\" + looxidlinkFolderPath + "\\" + appLocationFileName;
            if (!File.Exists(roamingFile))
            {   // LogggerExtensions.LogError?
                LXDebug.LogError("File does not exist: " + roamingFile);    
            }
            StreamReader reader = new System.IO.StreamReader(roamingFile); 

            //  현재 위치에서 끝까지의 스트림은 문자열 반환, 현재 위치가 끝에 있으면 빈 문자열 반환
            string appPath = reader.ReadToEnd();
            if (!File.Exists(appPath))
            {
                LXDebug.LogError("File does not exist: " + appPath);
            }

            //LXDebug.Log("LooxidLinkCore: " + appPath);
            // 문서 또는 애플리케이션 파일이름 지정, 프로세스 리소스 시작, 해당 리소스를 새 Process 구성 요소에 연결
            Process.Start(appPath);
        }
    }
}