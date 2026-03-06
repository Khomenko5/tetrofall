using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] tetrominoes;

    void Start()
    {
        SpawnNext();
    }

    public void SpawnNext()
    {
        int randomIndex = Random.Range(0, tetrominoes.Length);
        Instantiate(tetrominoes[randomIndex], transform.position, Quaternion.identity);
    }
}