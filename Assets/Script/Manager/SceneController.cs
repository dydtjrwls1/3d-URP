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

    // 게임오버일 경우 현재 신 로드
    IEnumerator LoadCurrentSceneAsync()
    {
        // 게임 오버 화면으로 전환
        yield return StartCoroutine(ScreenFader.Instance.FadeOut(ScreenFader.ScreenType.GameOver));

        // 현재 신 로드 
        AsyncOperation async = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        async.allowSceneActivation = false;

        // 로딩 완료 될때 까지 대기
        while(async.progress < 0.9f)
        {
            yield return null;
        }

        // 로딩 완료 이벤트 발생
        onLoadDone?.Invoke();

        IsRestartButtonClicked = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // restart 버튼 클릭 시 까지 대기
        while (!IsRestartButtonClicked)
        {
            yield return null;
        }

        async.allowSceneActivation = true;

        // 현재 화면으로 전환
        yield return StartCoroutine(ScreenFader.Instance.FadeIn());
    }

    // 특정 신 로드
    IEnumerator LoadSceneAsync(int sceneId)
    {
        // 로딩화면으로 전환
        yield return StartCoroutine(ScreenFader.Instance.FadeOut(ScreenFader.ScreenType.Loading));

        yield return SceneManager.LoadSceneAsync(sceneId);

        // 현재 화면으로 전환
        yield return StartCoroutine(ScreenFader.Instance.FadeIn());
    }

    public void RestartBoolChange()
    {
        IsRestartButtonClicked = true;
    }
}