using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePresenter : MonoBehaviour
{
    [SerializeField]
    PlayerMovementContoller playerMovementContoller;

    TextMeshProUGUI scoreTextGUI;

    private void Start()
    {
        Health health = GetComponentInParent<Health>();
        health.onDie += UpdateScoreDisplay;

        scoreTextGUI = ScreenFader.Instance.ScoreTextGUI;
    }

    private void UpdateScoreDisplay()
    {
        if (scoreTextGUI != null)
        {
            scoreTextGUI.text = $"Score : {playerMovementContoller.Score}";
        }
    }
}
