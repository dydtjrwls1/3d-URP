using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : SingleTon<SceneController>
{
    public bool IsRestartButtonClicked { get; private set; }

    public event Action onLoadDone = null;

    public void ReloadCurrentScene()
    {
        StartCoroutine(LoadCurrentSceneAsync());
    }

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    // ���ӿ����� ��� ���� �� �ε�
    IEnumerator LoadCurrentSceneAsync()
    {
        // ���� ���� ȭ������ ��ȯ
        yield return StartCoroutine(ScreenFader.Instance.FadeOut(ScreenFader.ScreenType.GameOver));

        // ���� �� �ε� 
        AsyncOperation async = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        async.allowSceneActivation = false;

        // �ε� �Ϸ� �ɶ� ���� ���
        while(async.progress < 0.9f)
        {
            yield return null;
        }

        // �ε� �Ϸ� �̺�Ʈ �߻�
        onLoadDone?.Invoke();

        IsRestartButtonClicked = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // restart ��ư Ŭ�� �� ���� ���
        while (!IsRestartButtonClicked)
        {
            yield return null;
        }

        async.allowSceneActivation = true;

        // ���� ȭ������ ��ȯ
        yield return StartCoroutine(ScreenFader.Instance.FadeIn());
    }

    // Ư�� �� �ε�
    IEnumerator LoadSceneAsync(int sceneId)
    {
        // �ε�ȭ������ ��ȯ
        yield return StartCoroutine(ScreenFader.Instance.FadeOut(ScreenFader.ScreenType.Loading));

        yield return SceneManager.LoadSceneAsync(sceneId);

        // ���� ȭ������ ��ȯ
        yield return StartCoroutine(ScreenFader.Instance.FadeIn());
    }

    public void RestartBoolChange()
    {
        IsRestartButtonClicked = true;
    }
}