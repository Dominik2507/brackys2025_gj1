using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetingGridScript : MonoBehaviour
{
    [SerializeField] int rows;
    [SerializeField] int cols;
    [SerializeField] float cellSize;

    private Dictionary<string, GameObject> targetedCellsDictionary = new Dictionary<string, GameObject>();

    //Prefabs
    public GameObject targetPrefab;
    public GameObject birdPrefab;

    //Debuging
    public Color gridColor = Color.red;
    [SerializeField] bool testSpawnTarget = false;

    private void Update()
    {
        if (testSpawnTarget)
        {
            TestTargetRandom();
            testSpawnTarget = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        // Calculate the grid center offset
        Vector3 centerOffset = new Vector3(cols * cellSize * 0.5f, rows * cellSize * 0.5f, 0);
        Vector3 origin = transform.position - centerOffset;

        // Draw vertical lines
        for (int x = 0; x <= cols; x++)
        {
            Vector3 start = origin + new Vector3(x * cellSize, 0, 0);
            Vector3 end = origin + new Vector3(x * cellSize, rows * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }

        // Draw horizontal lines
        for (int y = 0; y <= rows; y++)
        {
            Vector3 start = origin + new Vector3(0, y * cellSize, 0);
            Vector3 end = origin + new Vector3(cols * cellSize, y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }

    void TestTargetRandom()
    {
        int row = Random.Range(0, rows - 1);
        int col = Random.Range(0, cols - 1);

        TargetSingleCell(row, col);
    }

    bool TargetSingleCell(int row, int col)
    {
        if (targetedCellsDictionary.ContainsKey(row.ToString() + "-" + col.ToString())) return false;

        Vector3 centerOffset = new Vector3(cols * cellSize * 0.5f, rows * cellSize * 0.5f, 0);
        Vector3 origin = transform.position - centerOffset;
        Vector3 worldPosition = origin + new Vector3((col + 0.5f) * cellSize, (row + 0.5f) * cellSize, 0);

        GameObject spawned_bird = Instantiate(birdPrefab, 10.0f * Vector3.left, Quaternion.identity);

        targetedCellsDictionary.Add(row.ToString() + "-" + col.ToString(), spawned_bird);

        BirdTargetingScript b_script = spawned_bird.GetComponent<BirdTargetingScript>();

        b_script.grid = this;
        b_script.SetTargetPosition(worldPosition);
        b_script.allow_move = true;
        return true;
    }

    public void RemoveBirdFromDictionary(GameObject target)
    {
        string keyToRemove = null;

        // Find the key corresponding to the given target
        foreach (var pair in targetedCellsDictionary)
        {
            if (pair.Value == target)
            {
                keyToRemove = pair.Key;
                break; // Stop after finding the first match
            }
        }

        // Remove from dictionary if found
        if (keyToRemove != null)
        {
            targetedCellsDictionary.Remove(keyToRemove);
            Destroy(target);
        }
    }

    
}
