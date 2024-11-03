using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : SingleTon<LoadingManager>
{
    int? sceneId = null;

    public void LoadScene(int sceneId)
    {
        // 불러올 신 넘버
        this.sceneId = sceneId;

        // 로딩화면 신 불러오기
        SceneManager.LoadScene("Loading");
    }

    protected override void OnInitialize()
    {
        // 불러올 신의 넘버가 존재하면
        if(sceneId != null)
        {
            LoadingWindow loadingWindow = FindAnyObjectByType<LoadingWindow>();
            // 로딩창 UI 가 존재한다면
            if (loadingWindow != null) 
            {
                // 로딩 시작
                loadingWindow.LoadScene(sceneId.Value);
                
                // 불러올 신 아이디는 초기화
                sceneId = null; 
            }
        }
    }
}