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
        // �ҷ��� �� �ѹ�
        this.sceneId = sceneId;

        // �ε�ȭ�� �� �ҷ�����
        SceneManager.LoadScene("Loading");
    }

    protected override void OnInitialize()
    {
        // �ҷ��� ���� �ѹ��� �����ϸ�
        if(sceneId != null)
        {
            LoadingWindow loadingWindow = FindAnyObjectByType<LoadingWindow>();
            // �ε�â UI �� �����Ѵٸ�
            if (loadingWindow != null) 
            {
                // �ε� ����
                loadingWindow.LoadScene(sceneId.Value);
                
                // �ҷ��� �� ���̵�� �ʱ�ȭ
                sceneId = null; 
            }
        }
    }
}