using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    [SerializeField] bool autoPlay = false;
    object[] paddleSprites = new object[3];
    SpriteRenderer spriteRenderer;
    Ball ball;
    int currentSize = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        paddleSprites = Resources.LoadAll("Paddle", typeof(Sprite));
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        ball = FindObjectOfType<Ball>();

        spriteRenderer.sprite = (Sprite) paddleSprites[currentSize];
        transform.position = new Vector3(8, 0.3f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float newPositionX;
        if (Input.GetKeyUp(KeyCode.Space))
        {
            autoPlay = !autoPlay;
        }
        if (autoPlay)
        {
            newPositionX = Mathf.Clamp(ball.transform.position.x, 1.2f, 14.8f);
        }
        else
        {
            newPositionX = Mathf.Clamp((Input.mousePosition.x / Screen.width * 16), 1.2f, 14.8f);
        }
        transform.position = new Vector3(newPositionX, transform.position.y, 0);
    }

    public IEnumerator ChangePaddleSize(int size)
    {
        currentSize = Mathf.Clamp((currentSize + size), 0, 2);

        spriteRenderer.sprite = (Sprite) paddleSprites[currentSize];
        yield return new WaitForSecondsRealtime(10);

        currentSize = 1;
        spriteRenderer.sprite = (Sprite) paddleSprites[currentSize];
        yield return null;
    }

}
