using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePanel : MonoBehaviour
{
    public float changeSpeed = 2f;

    TextMeshProUGUI m_ScoreGUI;

    float m_TargetScore;
    float m_CurrentScore;

    float CurrentScore
    {
        get => m_CurrentScore;
        set
        {
            m_CurrentScore = value;
            m_ScoreGUI.text = $"{m_CurrentScore:f0}";
        }
    }

    private void Awake()
    {
        m_ScoreGUI = GetComponentInChildren<TextMeshProUGUI>(); 
    }

    void Start()
    {
        CurrentScore = 0f;

        PlayerMovementContoller player = GameManager.Instance.Player;

        player.onScoreChange += (score) =>
        {
            m_TargetScore = score;
        };
    }

    private void Update()
    {
        CurrentScore = Mathf.Lerp(CurrentScore, m_TargetScore, changeSpeed * Time.deltaTime);
    }
}
