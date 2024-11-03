using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingWindow : MonoBehaviour
{
    Slider loadingSlider;

    TextMeshProUGUI loadingText;

    AsyncOperation async;

    // 최소 로딩 시간
    [SerializeField]
    float minWaitTime = 5.0f;

    private void Awake()
    {
        Transform child = transform.GetChild(1);
        loadingSlider = child.GetComponent<Slider>();

        child = transform.GetChild(2);
        loadingText = child.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        loadingSlider.value = 0.0f;
    }

    public void LoadScene(int sceneNumber)
    {
        StartCoroutine(LoadSceneAsync(sceneNumber));
        StartCoroutine(UpdateLoadingText());
    }
    
    IEnumerator LoadSceneAsync(int sceneNumber)
    {
        float elapsedTime = 0.0f;
        bool isLoadingComplete = false;

        async = SceneManager.LoadSceneAsync(sceneNumber);
        async.allowSceneActivation = false;

        // 로딩이 완료될 때 까지
        while (!isLoadingComplete)
        {
            elapsedTime += Time.deltaTime;

            float progress = 0.0f;

            // 누적시간이 최소 로딩시간 보다 작다면
            if (elapsedTime < minWaitTime)
            {
                // 진행정도는 최소 대기시간에 따른다.
                progress = elapsedTime / minWaitTime;
            }
            else
            {
                // 그렇지 않을경우 진행정도는 async 의 값에 따른다.
                progress = Mathf.Clamp01(async.progress / 0.9f);
            }
            
            loadingSlider.value = progress;

            // 로딩이 거의 완료되었고 최소 대기시간이 지났을경우
            if(async.progress >= 0.9f && elapsedTime > minWaitTime)
            {
                loadingText.text = "Complete!";

                // 아무키나 누르면
                if (Input.anyKeyDown)
                {
                    // 로딩 완료
                    async.allowSceneActivation = true;
                    isLoadingComplete = true;
                }
            }

            yield return null;
        }
    }

    IEnumerator UpdateLoadingText()
    {
        string[] loadingTexts =
        {
            "Loading .",
            "Loading ..",
            "Loading ...",
            "Loading ...."
        };

        int index = 0;

        float elapsedTime = 0.0f;
        float interval = 0.3f;

        // 로딩이 거의 완료될 때 까지
        while (async.progress <= 0.9f)
        {
            elapsedTime += Time.deltaTime;

            // 누적 시간이 간격보다 커지면
            if (elapsedTime > interval) 
            {
                // 누적 시간 초기화
                elapsedTime = 0.0f;

                // index에 해당하는 로딩 텍스트 출력
                loadingText.text = loadingTexts[index];

                // index 업데이트
                index = (index + 1) % loadingTexts.Length;
            }
            
            yield return null;
        }
    }
}
