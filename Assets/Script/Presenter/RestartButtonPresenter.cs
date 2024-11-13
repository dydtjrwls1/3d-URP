using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartButtonPresenter : MonoBehaviour
{
    [SerializeField]
    SceneController sceneController;

    [SerializeField]
    Button restartButton;

    private void Start()
    {
        restartButton.image.color = Color.clear;
        restartButton.interactable = false;


        // sceneController에서 로딩 완료 시 restart 버튼 활성화
        sceneController.onLoadDone += () =>
        {
            restartButton.image.color = Color.white;
            restartButton.interactable = true;
        };

        restartButton.onClick.AddListener(sceneController.RestartBoolChange);
    }

    
}
