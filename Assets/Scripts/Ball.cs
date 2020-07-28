using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] GameObject paddle;
    [SerializeField] AudioClip audioClip1;
    [SerializeField] AudioClip audioClip2;
    public bool isResting;
    private int speed;

    Rigidbody2D rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        int ballCount = FindObjectsOfType<Ball>().Length;
        if (ballCount == 1)
        {
            isResting = true;
        }
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isResting)
        {
            // move with the paddle
            transform.position = paddle.transform.position + new Vector3(0, 0.66f, 0);
            if(Input.GetMouseButtonDown(0))
            {
                Launch();
                // FindObjectOfType<LevelLoader>().NextLevel();  -- to test the level generation
            }
        }
    }

    void Launch()
    {
        // launch the ball
        rigidbody2D.velocity = new Vector2(0, 11 + speed);
        Debug.Log("Ball launched");
        isResting = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ball")
        {
            Physics2D.IgnoreCollision(
                gameObject.GetComponent<Collider2D>(),
                col.gameObject.GetComponent<Collider2D>(),
                true
            );
        }
        else
        {
            if(!isResting)
            {
                float xVelocity = Random.Range(0.0f, 0.5f) * Power(-1, Random.Range(1, 3));
                float yVelocity = Random.Range(0.0f, 0.5f) * Power(-1, Random.Range(1, 3));
                rigidbody2D.velocity += new Vector2(xVelocity, yVelocity);
                PlayAudio(col);
            }
        }
    }

    private void PlayAudio(Collision2D col)
    {
        if(col.gameObject.name == "Paddle")
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(audioClip1);
        }
        else
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(audioClip2);
        }
    }

    private int Power(int value, int power)
    {
        int result = 1;
        for (int i = 0; i < power; i++)
        {
            result *= value;
        }
        return result;
    }

    public IEnumerator ChangeBallSpeed(float speedPercentage)
    {
        rigidbody2D.velocity = new Vector2(
            rigidbody2D.velocity.x * speedPercentage,
            rigidbody2D.velocity.y * speedPercentage
            );
        yield return new WaitForSecondsRealtime(5);

        rigidbody2D.velocity = new Vector2(
            rigidbody2D.velocity.x / speedPercentage,
            rigidbody2D.velocity.y / speedPercentage
            );
        yield return null;
    }

    public IEnumerator SplitBall()
    {
        // split ball
        GameObject bonusBall1 = Instantiate(gameObject, transform.position, Quaternion.identity);
        GameObject bonusBall2 = Instantiate(gameObject, transform.position, Quaternion.identity);
        bonusBall1.GetComponent<Rigidbody2D>().velocity = new Vector2(
            rigidbody2D.velocity.x - 1,
            rigidbody2D.velocity.y + 1
        );
        bonusBall2.GetComponent<Rigidbody2D>().velocity = new Vector2(
            rigidbody2D.velocity.x + 1,
            rigidbody2D.velocity.y - 1
        );
        yield return new WaitForSecondsRealtime(5);

        // delete other ball instances
        Ball[] currentBalls = FindObjectsOfType<Ball>();
        if (currentBalls.Length > 1)
        {
            for (int i = 1; i < currentBalls.Length; i++)
            {
                GameObject ballToDestroy = currentBalls[i].gameObject;
                Debug.Log("Destroy ball: " + ballToDestroy.name);
                Destroy(ballToDestroy);
            }
        }
        yield return null;
    }
}
