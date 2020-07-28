using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseCollider : MonoBehaviour
{
    private LevelLoader levelLoader;
    private GameKeeper gameKeeper;

    void Start()
    {
        levelLoader = (LevelLoader) GameObject.FindObjectOfType(typeof(LevelLoader));
        gameKeeper = (GameKeeper) GameObject.FindObjectOfType(typeof(GameKeeper));
    }
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject.tag == "Ball")
        {
            if (FindObjectsOfType<Ball>().Length == 1)
            {
                gameKeeper.LoseLife();
                collider.gameObject.GetComponent<Ball>().isResting = true;
            }
            else
            {
                Destroy(collider.gameObject);
            }
        }
    }
}
