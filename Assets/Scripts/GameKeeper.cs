using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameKeeper : MonoBehaviour
{
    [SerializeField] GameObject scoreDisplay;
    [SerializeField] GameObject lifeDisplay;

    LevelLoader levelLoader;

    public int level;
    private int score;
    private int maxLives = 3;
    private int currentLives = 3;
    object[] livesSprites = new object[4];

    // Start is called before the first frame update
    void Awake()
    {
        int gameKeeperCount = FindObjectsOfType<GameKeeper>().Length;
        if (gameKeeperCount > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        livesSprites = Resources.LoadAll("Lives", typeof(Sprite));
        SetLifeDisplay();
    }

    void Update()
    {
        DisplayScore();
    }

    public void ChangeScore(int amount)
    {
        score += amount;
    }

    public void Reset()
    {
        score = 0;
        level = 0;
        currentLives = maxLives;
        SetLifeDisplay();
    }

    private void DisplayScore()
    {
        scoreDisplay.GetComponent<UnityEngine.UI.Text>().text = score.ToString();
    }

    public void LoseLife()
    {
        currentLives--;
        SetLifeDisplay();
        if (currentLives <= 0)
        {
            levelLoader.GameOver();
        }
    }
    public void GainLife()
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            SetLifeDisplay();
        }
    }

    private void SetLifeDisplay()
    {
        lifeDisplay.GetComponent<SpriteRenderer>().sprite = (Sprite) livesSprites[currentLives];
    }

}
