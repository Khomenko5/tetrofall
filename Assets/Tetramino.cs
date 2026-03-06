using UnityEngine;

public class Tetromino : MonoBehaviour
{
    float lastFall = 0;

    void Start()
    {
        if (!IsValidGridPos())
        {
            if (GridManager.instance != null)
            {
                GridManager.instance.GameOver();
            }
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);
            if (!IsValidGridPos()) transform.position += new Vector3(1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);
            if (!IsValidGridPos()) transform.position += new Vector3(-1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, -90);
            if (!IsValidGridPos()) transform.Rotate(0, 0, 90);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= GridManager.instance.currentFallSpeed)
        {
            transform.position += new Vector3(0, -1, 0);

            if (IsValidGridPos())
            {
                lastFall = Time.time;
            }
            else
            {
                transform.position += new Vector3(0, 1, 0);
                LockToGrid();
            }
        }
    }

    bool IsValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = GridManager.RoundVec2(child.position);

            if (!GridManager.InsideBorder(v)) return false;

            if (GridManager.grid[(int)v.x, (int)v.y] != null &&
                GridManager.grid[(int)v.x, (int)v.y].parent != transform)
            {
                return false;
            }
        }
        return true;
    }

    void LockToGrid()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = GridManager.RoundVec2(child.position);
            GridManager.grid[(int)v.x, (int)v.y] = child;
        }

        GridManager.DeleteFullRows();
        enabled = false;
        FindObjectOfType<Spawner>().SpawnNext();
    }
}