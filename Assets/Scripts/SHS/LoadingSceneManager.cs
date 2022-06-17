using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public Slider slider;
    public string SceneName;

    private float time;
    
    
    void Start() {
        StartCoroutine(LoadAsyncSceneCoroutine());
        slider.value = 0.0f;
    }
    

    /*
    void Awake() {
        StartCoroutine(LoadAsyncSceneCoroutine());
        slider.value = 0.0f;
    }
    */

    IEnumerator LoadAsyncSceneCoroutine() {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName);
        operation.allowSceneActivation = false;

        while(!operation.isDone) {
            time =+ Time.timeSinceLevelLoad;
            slider.value = time / 5.0f;

            if(time > 5) {
                operation.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
}
