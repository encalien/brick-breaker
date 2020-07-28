using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    // parameters
    [SerializeField] AudioClip audioClip1;
    [SerializeField] GameObject powerUp;
    Sprite[] fullSpriteArray = new Sprite[10];
    Sprite[] damagedSpriteArray = new Sprite[10];
    static Color[] colors = new Color[] {Color.red, Color.green, Color.blue, Color.cyan, Color.yellow, Color.magenta};
    int maxHits;
    int timesHit;
    object[] sprites;
    SpriteRenderer spriteRenderer;
    public int colorIndex;

    GameKeeper gameKeeper;

    // cached references

    // Start is called before the first frame update
    void Start()
    {
        gameKeeper = FindObjectOfType<GameKeeper>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        sprites = Resources.LoadAll("Brick", typeof(Sprite));

        if (tag == "Breakable")
        {
            SetMaxHits();
            spriteRenderer.sprite = (Sprite) sprites[timesHit];
            spriteRenderer.color = colors[colorIndex];
            if (maxHits > 1)
            {
                GameObject metalPlates = new GameObject("metal plates");
                metalPlates.AddComponent<SpriteRenderer>().sprite = (Sprite) sprites[2];
                metalPlates.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
                metalPlates.transform.localScale = new Vector3(0.45f, 0.45f, 1);
                metalPlates.transform.parent = gameObject.transform;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Brick hit");
        if (tag == "Breakable")
        {
            HandleHit();
        }
    }

    private void SetMaxHits()
    {
        int rand = Random.Range(0, 10);
        maxHits = (rand + gameKeeper.level < 20) ? 1 : 2;
    }

    private void HandleHit()
    {
        timesHit++;
        if (timesHit >= maxHits)
        {
            gameKeeper.ChangeScore(10);
            GeneratePowerUp();
            AudioSource.PlayClipAtPoint(audioClip1, Camera.main.transform.position);
            Destroy(gameObject);
        }
        else
        {
            gameKeeper.ChangeScore(5);
            spriteRenderer.sprite = (Sprite) sprites[timesHit];
        }
    }

    private void GeneratePowerUp()
    {
        float index = Random.Range(0f, 1f);
        if ((maxHits == 2 && index < 0.3f) || (maxHits == 1 && index < 0.05f))
        {
            Instantiate(powerUp, gameObject.transform.position, Quaternion.identity);
        }
    }
}
