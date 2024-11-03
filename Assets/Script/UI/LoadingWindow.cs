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

    // �ּ� �ε� �ð�
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

        // �ε��� �Ϸ�� �� ����
        while (!isLoadingComplete)
        {
            elapsedTime += Time.deltaTime;

            float progress = 0.0f;

            // �����ð��� �ּ� �ε��ð� ���� �۴ٸ�
            if (elapsedTime < minWaitTime)
            {
                // ���������� �ּ� ���ð��� ������.
                progress = elapsedTime / minWaitTime;
            }
            else
            {
                // �׷��� ������� ���������� async �� ���� ������.
                progress = Mathf.Clamp01(async.progress / 0.9f);
            }
            
            loadingSlider.value = progress;

            // �ε��� ���� �Ϸ�Ǿ��� �ּ� ���ð��� ���������
            if(async.progress >= 0.9f && elapsedTime > minWaitTime)
            {
                loadingText.text = "Complete!";

                // �ƹ�Ű�� ������
                if (Input.anyKeyDown)
                {
                    // �ε� �Ϸ�
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

        // �ε��� ���� �Ϸ�� �� ����
        while (async.progress <= 0.9f)
        {
            elapsedTime += Time.deltaTime;

            // ���� �ð��� ���ݺ��� Ŀ����
            if (elapsedTime > interval) 
            {
                // ���� �ð� �ʱ�ȭ
                elapsedTime = 0.0f;

                // index�� �ش��ϴ� �ε� �ؽ�Ʈ ���
                loadingText.text = loadingTexts[index];

                // index ������Ʈ
                index = (index + 1) % loadingTexts.Length;
            }
            
            yield return null;
        }
    }
}
