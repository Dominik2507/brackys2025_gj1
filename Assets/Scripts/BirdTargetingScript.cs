using UnityEngine;
using System.Collections;

public class BirdTargetingScript : MonoBehaviour
{
    public Vector3 target;
    public bool allow_move = false;
    public float move_speed = 3.0f;
    public Vector3 move_direction;

    public GameObject targetPrefab;
    public GameObject poopPrefab;
    public TargetingGridScript grid;

    private GameObject spawnedTarget = null;
    private GameObject spawnedPoop = null;

    private void Update()
    {
        if (allow_move)
        {
            transform.position += move_speed * Time.deltaTime * move_direction;

            if(spawnedTarget == null && Vector3.Distance(target, transform.position) < 1e-1)
            {
                SpawnTargetPrefab();
            }
        }   
    }

    public void SpawnTargetPrefab()
    {
        //spawn
        spawnedTarget = Instantiate(targetPrefab, target, Quaternion.identity);

        //despawn
        StartCoroutine(DropBomb());
    }

    IEnumerator DropBomb()
    {
        spawnedPoop = Instantiate(poopPrefab, target, Quaternion.identity);

        for(float time = 0; time < 3.0f; time += Time.fixedDeltaTime)
        {
            spawnedPoop.transform.localScale = Mathf.Lerp(0.5f, 1.0f, time / 3.0f) * Vector3.one;
            yield return new WaitForFixedUpdate();
        }
        
        Destroy(spawnedTarget);

        StartCoroutine(DespawnTarget());
    }

    IEnumerator DespawnTarget()
    {
        yield return new WaitForSeconds(3f);

        Destroy(spawnedPoop);

        grid.RemoveBirdFromDictionary(gameObject);
    }

    public void SetTargetPosition(Vector3 target)
    {
        this.target = target;
        move_direction = (target - transform.position).normalized;
    }
}
