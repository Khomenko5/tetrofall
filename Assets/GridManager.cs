using UnityEngine;
using TMPro;

public class GridManager : MonoBehaviour
{
    public static int w = 10;
    public static int h = 20;
    public static Transform[,] grid = new Transform[w, h];

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverPanel;
    public GameObject tryAgainButton;
    public GameObject borders;

    public TextMeshProUGUI finalScoreText;

    private int score = 0;
    private int lives = 3;

    public float currentFallSpeed = 1.0f;
    private float minFallSpeed = 0.1f;
    private float speedStep = 0.010f;

    public static GridManager instance;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
        UpdateLivesText();
    }

    public static Vector2 RoundVec2(Vector2 v) { return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y)); }
    public static bool InsideBorder(Vector2 pos) { return ((int)pos.x >= 0 && (int)pos.x < w && (int)pos.y >= 0); }

    public static void DeleteFullRows()
    {
        int linesCleared = 0;
        for (int y = 0; y < h; y++)
        {
            if (IsRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                y--;
                linesCleared++;
            }
        }
        if (linesCleared > 0 && instance != null) instance.AddScore(linesCleared);
    }

    static bool IsRowFull(int y)
    {
        for (int x = 0; x < w; x++) if (grid[x, y] == null) return false;
        return true;
    }

    static void DeleteRow(int y)
    {
        for (int x = 0; x < w; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    static void DecreaseRowsAbove(int y) { for (int i = y; i < h; i++) DecreaseRow(i); }

    static void DecreaseRow(int y)
    {
        for (int x = 0; x < w; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void AddScore(int lines)
    {
        if (lines == 1) score += 100;
        else if (lines == 2) score += 300;
        else if (lines == 3) score += 500;
        else if (lines >= 4) score += 800;

        if (scoreText != null) scoreText.text = "SCORE: " + score;

        currentFallSpeed -= speedStep * lines;
        if (currentFallSpeed < minFallSpeed)
        {
            currentFallSpeed = minFallSpeed;
        }
    }

    void UpdateLivesText()
    {
        if (livesText != null) livesText.text = "LIVES: " + lives;
    }

    void ClearBoard()
    {
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (grid[x, y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }

        Tetromino[] activePieces = FindObjectsOfType<Tetromino>();
        foreach (Tetromino piece in activePieces)
        {
            Destroy(piece.gameObject);
        }
    }

    public void GameOver()
    {
        lives--;
        UpdateLivesText();

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (lives > 0)
        {
            if (tryAgainButton != null) tryAgainButton.SetActive(true);
        }
        else
        {
            if (tryAgainButton != null) tryAgainButton.SetActive(false);
        }

        if (finalScoreText != null)
        {
            if (lives > 0)
            {
                finalScoreText.text = "ROUND SCORE: " + score;
            }
            else
            {
                finalScoreText.text = "FINAL SCORE: " + score;
            }
            finalScoreText.gameObject.SetActive(true);
        }

        if (scoreText != null) scoreText.gameObject.SetActive(false);
        if (livesText != null) livesText.gameObject.SetActive(false);
        if (borders != null) borders.SetActive(false);

        ClearBoard();
        Time.timeScale = 0f;
    }

    public void TryAgain()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (tryAgainButton != null) tryAgainButton.SetActive(false);
        if (finalScoreText != null) finalScoreText.gameObject.SetActive(false);

        if (scoreText != null) scoreText.gameObject.SetActive(true);
        if (livesText != null) livesText.gameObject.SetActive(true);
        if (borders != null) borders.SetActive(true);

        score = 0;
        if (scoreText != null) scoreText.text = "SCORE: " + score;
        currentFallSpeed = 1.0f;

        Time.timeScale = 1f;
        FindObjectOfType<Spawner>().SpawnNext();
    }
}