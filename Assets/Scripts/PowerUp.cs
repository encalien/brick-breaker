using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    GameKeeper gameKeeper;
    LoseCollider loseCollider;
    Ball ball;
    PaddleController paddle;
    // Start is called before the first frame update
    void Start()
    {
        // find references
        gameKeeper = FindObjectOfType<GameKeeper>();
        loseCollider = FindObjectOfType<LoseCollider>();
        ball = FindObjectOfType<Ball>();
        paddle = FindObjectOfType<PaddleController>();

        // initialize power up
        object[] powerUps = Resources.LoadAll("Power-Ups", typeof(Sprite));
        int maxSpriteIndex = (gameKeeper.level < 5) ? 3 : (gameKeeper.level < 10) ? 8 : powerUps.Length;
        gameObject.GetComponent<SpriteRenderer>().sprite = (Sprite) powerUps[Random.Range(0, maxSpriteIndex)];
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -2f);
    }

    IEnumerator OnTriggerEnter2D(Collider2D collider)
    {
        object[] currentBalls = GameObject.FindObjectsOfType<Ball>();

        if(collider.gameObject.name == "Paddle")
        {
            switch (gameObject.GetComponent<SpriteRenderer>().sprite.name)
            {
                case "01-Life":
                    gameKeeper.GainLife();
                    break;
                case "02-50-points":
                    gameKeeper.ChangeScore(50);
                    break;
                case "03-100-points":
                    gameKeeper.ChangeScore(100);
                    break;
                case "04-250-points":
                    gameKeeper.ChangeScore(250);
                    break;
                case "05-500-points":
                    gameKeeper.ChangeScore(500);
                    break;
                case "06-Fast":
                    foreach (Ball ball in currentBalls)
                    {
                        StartCoroutine(ball.ChangeBallSpeed(1.5f));
                    }
                    break;
                case "07-Slow":
                    foreach (Ball ball in currentBalls)
                    {
                        StartCoroutine(ball.ChangeBallSpeed(0.5f));
                    }
                    break;
                case "08-Large":
                        StartCoroutine(paddle.ChangePaddleSize(1));
                    break;
                case "09-Small":
                        StartCoroutine(paddle.ChangePaddleSize(-1));
                    break;
                case "10-Split":
                    foreach (Ball ball in currentBalls)
                    {
                        StartCoroutine(ball.SplitBall());
                    }
                    break;
                case "11-Fire":
                    break;
                default:
                    Debug.Log("Default case");
                    break;
            }

            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(11);
            Destroy(gameObject);
        }
    }
}
