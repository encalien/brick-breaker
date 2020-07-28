using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BrickLayout : MonoBehaviour
{
    // parameters
    [SerializeField] GameObject brick;
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] int full;
    [SerializeField] int empty;
    int breakableBricks;
    int numColumns = 9;
    int numRows = 10;
    private PointF[] possiblePositions;
    float[] columnNumber;
    float[] rowNumber;
    bool canFinishLevel;
    float randomColorAlignment;
    int colorOffset;


    // cached reference
    GameKeeper gameKeeper;

    // Start is called before the first frame update
    void Start()
    {
        gameKeeper = GameObject.FindObjectOfType<GameKeeper>();
        canFinishLevel = true;

        randomColorAlignment = Random.Range(0f, 1f);
        colorOffset = Random.Range(0, 5);

        breakableBricks = Random.Range(5, (numColumns * numRows)/10);
        GenerateLayout(breakableBricks);
    }

    void Update()
    {
        // if there are no more bricks reload level
        if (GameObject.FindGameObjectsWithTag("Breakable").Length == 0 && canFinishLevel)
        {
            canFinishLevel = false;
            GameObject.FindObjectOfType<Ball>().isResting = true;
            StartCoroutine (LevelCleared());
        }
    }

    IEnumerator LevelCleared()
    {
        gameKeeper.ChangeScore(500);
        yield return new WaitForSeconds(1);
        levelLoader.NextLevel();

        yield return null;
    }

    // Update is called once per frame

    private void GeneratePossiblePositions()
    {
        possiblePositions = new PointF[numColumns * numRows];
        columnNumber = new float[numColumns];
        rowNumber = new float[numRows];

        // snap to 1; 0.7
        for (int i = 0; i < numColumns; i++)
        {
            columnNumber[i] = 0.89f + 1.78f * i;
        }
        for (int i = 0; i < numRows; i++)
        {
            rowNumber[i] = 10.7f - 0.62f * i;
        }

        for (int i = 0; i < columnNumber.Length; i++)
        {
            for (int j = 0; j < rowNumber.Length; j++)
            {
                possiblePositions[i * numRows + j] = new PointF(columnNumber[i], rowNumber[j]);
            }
        }
    }
    private void GenerateLayout(int breakableBricks)
    {
        GeneratePossiblePositions();
        // OuterDiamondLayout(false);
        // RectangleLayout();
        // FullRowsLayout();
        // FullColumnsLayout();

        switch (gameKeeper.level / 5)
        {
            case 0:
                FullRowsLayout();
                break;
            case 1:
                FullColumnsLayout();
                break;
            case 2:
                RectangleLayout();
                break;
            case 3:
                InnerDiamondLayout(false);
                break;
            case 4:
                OuterDiamondLayout(false);
                break;
            case 5:
                InnerDiamondLayout(true);
                OuterDiamondLayout(false);
                break;
            case 6:
                bool isSameColor = Random.Range(0f, 1f) < 0.5f ? true : false;
                CrossLayout(isSameColor);
                break;
            case 7:
                InnerDiamondLayout(false);
                OuterDiamondLayout(true);
                break;
            default:
                float rand = Random.Range(0f, 1f);
                PickRandomLayout(rand);
                break;
        }
    }

    private void RandomLayout()
    {
        for (int i = 0; i < breakableBricks; i++)
        {
            PointF randomPoint;
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, possiblePositions.Length);
                randomPoint = (PointF) possiblePositions[randomIndex];
            }
            while (randomPoint.X == -1f && randomPoint.Y == -1f);

            Vector2 position = new Vector2(randomPoint.X, randomPoint.Y);
            var newBrick = Instantiate(brick, position, Quaternion.identity);
            newBrick.transform.parent = gameObject.transform;
            possiblePositions[randomIndex] = new PointF(-1f, -1f);
        }
    }

    private void PickRandomLayout(float rand)
    {
        if (rand < 0.15f)
        {
            FullColumnsLayout();
        }
        else if (rand < 0.3f)
        {
            FullRowsLayout();
        }
        else if (rand < 0.45f)
        {
            RectangleLayout();
        }
        else if (rand < 0.6f)
        {
            InnerDiamondLayout(false);
        }
        else if (rand < 0.75f)
        {
            OuterDiamondLayout(false);
        }
        else if (rand < 0.9f)
        {
            InnerDiamondLayout(true);
            OuterDiamondLayout(false);
        }
        else if (rand < 0.95f)
        {
            InnerDiamondLayout(false);
            OuterDiamondLayout(true);
        }
        else
        {
            bool isSameColor = Random.Range(0f, 1f) < 0.5f ? true : false;
            CrossLayout(isSameColor);
        }
    }

    private void FullRowsLayout()
    {
        int wantedFullRows = Random.Range(1, 6);
        int wantedEmptyRows = Random.Range(1, 3);
        bool startingFull = Random.Range(0f, 1f) < 0.5f ? true : false;
        bool isFull = startingFull;
        int fullRows = 0;
        int emptyRows = 0;
        int extraRows;

        // calculate extra rows
        if (startingFull)
        {
            int repeatingK = (rowNumber.Length + wantedEmptyRows) / (wantedFullRows + wantedEmptyRows);
            extraRows = rowNumber.Length - (wantedFullRows * repeatingK) - (wantedEmptyRows * (repeatingK - 1));
        }
        else
        {
            int repeatingK = rowNumber.Length / (wantedFullRows + wantedEmptyRows);
            extraRows = rowNumber.Length - (wantedFullRows * repeatingK) - (wantedEmptyRows * repeatingK);
        }

        for (int rowIndex = 0; rowIndex < rowNumber.Length - extraRows; rowIndex++)
        {
            if (isFull)
            {
                for (int columnIndex = 0; columnIndex < columnNumber.Length; columnIndex++)
                {
                    int colorIndex = SetBrickColor(rowIndex, columnIndex);
                    PlaceBrick(columnIndex, rowIndex, colorIndex);
                }

                // count full rows
                fullRows++;
                if (fullRows == wantedFullRows)
                {
                    isFull = !isFull;
                    fullRows = 0;
                }
            }
            else
            {
                // count empty rows
                emptyRows++;
                if (emptyRows == wantedEmptyRows)
                {
                    isFull = !isFull;
                    emptyRows = 0;
                }
            }
        }
    }

    private void FullColumnsLayout()
    {
        bool placeUnbreakables = Random.Range(0f, 1f) < 0.5f ? true : false;
        Dictionary<int, bool> columnsStatus = new Dictionary<int, bool>(){};
        int fullColumns = 0;
        do
        {
            columnsStatus = new Dictionary<int, bool>(){};
            for (int i = 0; i < 5; i++)
            {
                bool randBool = Random.Range(0f, 1f) < 0.5f ? true : false;
                if (randBool) fullColumns++;
                columnsStatus[i] = randBool;
                columnsStatus[columnNumber.Length - 1 - i] = randBool;
            }
        } while (fullColumns < 2 || fullColumns == 5);

        for (int columnIndex = 0; columnIndex < columnNumber.Length; columnIndex++)
        {
            if (columnsStatus[columnIndex])
            {
                for (int rowIndex = 0; rowIndex < rowNumber.Length; rowIndex++)
                {
                    if (rowIndex == rowNumber.Length - 1 && placeUnbreakables)
                    {
                        PlaceUnbreakableBrick(columnIndex, rowIndex);
                    }
                    else
                    {
                        int colorIndex = SetBrickColor(rowIndex, columnIndex);
                        PlaceBrick(columnIndex, rowIndex, colorIndex);
                    }
                }
            }
        }
    }

    private void InnerDiamondLayout(bool isSameColor)
    {
        //   0  1  2  3  4  5  6  7  8
        // 0             *
        // 1          *     *
        // 2       *     *     *
        // 3    *     *     *     *
        // 4 *     *     *     *     *
        // 5    *     *     *     *
        // 6       *     *     *
        // 7          *     *
        // 8             *
        int middleColumnIndex = columnNumber.Length / 2;
        int randSpacing = isSameColor ? Random.Range(1, 3) : Random.Range(1, 5);
        int randStartingRing = Random.Range(0, 3);
        int color = Random.Range(0, 5);


        int sameColorDiff = isSameColor ? 1 : 0;
        for (int i = 0; i + sameColorDiff <= middleColumnIndex; i++)
        {
            int remainder = randStartingRing;
            while (remainder >= randSpacing)
            {
                remainder -= randSpacing;
            }

            if (i % randSpacing == remainder && i >= randStartingRing)
            {
                int colorIndex = isSameColor ? color : Random.Range(0, 5);
                int offset = 0;
                for (int rowIndex = i; rowIndex <= middleColumnIndex; rowIndex++)
                {
                    PlaceBrick(middleColumnIndex - offset, rowIndex, colorIndex);
                    PlaceBrick(middleColumnIndex - offset, columnNumber.Length - 1 - rowIndex, colorIndex);
                    if (rowIndex != i && rowIndex != columnNumber.Length - 1 - i)
                    {
                        PlaceBrick(middleColumnIndex + offset, rowIndex, colorIndex);
                        PlaceBrick(middleColumnIndex + offset, columnNumber.Length - 1 - rowIndex, colorIndex);
                    }
                    offset++;
                }
            }
        }
    }

    private void OuterDiamondLayout(bool isSameColor)
    {
        //   0  1  2  3  4  5  6  7  8
        // 0 *  *  *  *  *  *  *  *  *
        // 1 *  *  *  *     *  *  *  *
        // 2 *  *  *           *  *  *
        // 3 *  *                 *  *
        // 4 *                       *
        // 5 *  *                 *  *
        // 6 *  *  *           *  *  *
        // 7 *  *  *  *     *  *  *  *
        // 8 *  *  *  *  *  *  *  *  *

        int middleColumnIndex = columnNumber.Length / 2;
        int randSpacing = Random.Range(1, 3);
        int randStartingRing = Random.Range(0, 3);

        int sameColorDiff = isSameColor ? 1 : 0;
        int color = Random.Range(0, 5);
        int last = columnNumber.Length - 1;

        for (int i = 0; i + sameColorDiff <= middleColumnIndex; i++)
        {
            int remainder = randStartingRing;
            while (remainder >= randSpacing)
            {
                remainder -= randSpacing;
            }

            if (i % randSpacing == remainder && i >= randStartingRing)
            {
                color = isSameColor ? color : Random.Range(0, 5);
                for (int j = 0; j <= i; j++)
                {
                    PlaceBrick(i - j, j, color);
                    PlaceBrick(last - (i - j), j, color);

                    PlaceBrick(i - j, last - j, color);
                    PlaceBrick(last - (i - j), last - j, color);
                }
            }
        }
    }

    private void CrossLayout(bool isSameColor)
    {
        int middleColumnIndex = columnNumber.Length / 2;
        int randSpacing = Random.Range(1, 5);
        int randStartingRing = Random.Range(0, 3);

        // int offset = isSameColor ? 0 : -1;
        int color = Random.Range(0, 5);

        for (int i = 0; i <= middleColumnIndex; i++)
        {
            // int remainder = randStartingRing;
            // while (remainder >= randSpacing)
            // {
            //     remainder -= randSpacing;
            // }

            // if (i % randSpacing == remainder && i >= randStartingRing)
            // {
                int colorIndex = isSameColor ? color : Random.Range(0, 5);
                for (int rowIndex = 0; rowIndex < columnNumber.Length; rowIndex++)
                {
                    if (rowIndex == i || rowIndex == columnNumber.Length - 1 - i)
                    {
                        PlaceBrick(i, rowIndex, colorIndex);
                        PlaceBrick(columnNumber.Length - 1 - i, rowIndex, colorIndex);
                    }
                }
            // }
        }

        // different cross

        // int last = columnNumber.Length - 1;
        // for (int i = 0; i < middleColumnIndex; i++)
        // {
        //     for (int j = 0; j <= i; j++)
        //     {
        //         PlaceBrick(i - j, j, i);
        //         PlaceBrick(last - (i - j), j, i);

        //         PlaceBrick(i - j, last - (j), i);
        //         PlaceBrick(last - (i - j), last - (j), i);
        //     }
        // }
    }

    private void RectangleLayout()
    {
        bool startingFull = Random.Range(0f, 1f) < 0.5f ? true : false;

        for (int i = 0; i < columnNumber.Length; i++)
        {
            int colorIndex = Random.Range(0, 5);
            if ((startingFull && i % 2 == 0) || (!startingFull && i % 2 == 1))
            {
                for (int rowIndex = 0 + i; rowIndex < columnNumber.Length - i; rowIndex++)
                {
                    if (rowIndex == 0 + i || rowIndex == columnNumber.Length - 1 - i)
                    {
                        for (int columnIndex = 0 + i; columnIndex < columnNumber.Length - i; columnIndex++)
                        {
                            PlaceBrick(columnIndex, rowIndex, colorIndex);
                        }
                    }
                    PlaceBrick(i, rowIndex, colorIndex);
                    PlaceBrick(columnNumber.Length - 1 - i, rowIndex, colorIndex);
                }
            }
        }
    }

    private void PlaceUnbreakableBrick(int columnIndex, int rowIndex)
    {
        Vector2 position = new Vector2(columnNumber[columnIndex], rowNumber[rowIndex]);
        var newBrick = Instantiate(brick, position, Quaternion.identity);
        newBrick.gameObject.tag = "Unbreakable";

    }
    private void PlaceBrick(int columnIndex, int rowIndex, int colorIndex)
    {
        Vector2 position = new Vector2(columnNumber[columnIndex], rowNumber[rowIndex]);
        var newBrick = Instantiate(brick, position, Quaternion.identity);
        newBrick.transform.parent = gameObject.transform;
        newBrick.GetComponent<Brick>().colorIndex = colorIndex;
    }

    private int SetBrickColor(int rowIndex, int columnIndex)
    {
        int colorIndex = randomColorAlignment < 0.5f ? (rowIndex + colorOffset) : (columnIndex + colorOffset);
        while (colorIndex > 4)
        {
            colorIndex -= 5;
        }
        return colorIndex;
    }
}
