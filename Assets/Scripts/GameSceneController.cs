using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    [SerializeField] Button startButton;
    Player player;
    ObstacleController obstacleController;
    [SerializeField] Image endGameMsg;
    [SerializeField] Text scoreText;

    int score = 0;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        obstacleController = FindObjectOfType<ObstacleController>();
    }

    public void StartGame()
    {
        startButton.gameObject.SetActive(false);
        score = 0;
        scoreText.gameObject.SetActive(true);
        UpdateScore();
        player.Begin();
        obstacleController.Begin();
    }

    public void StopGame()
    {
        obstacleController.End();
        endGameMsg.gameObject.SetActive(true);
    }

    public void IncrementScore()
    {
        score += 1;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();
    }
}
