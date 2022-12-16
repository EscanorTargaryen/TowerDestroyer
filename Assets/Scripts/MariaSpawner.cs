using UnityEngine;

/// <summary>
/// Camp Manager
/// </summary>
public class MariaSpawner : MonoBehaviour
{
    /// <summary>
    /// Time passed since you spawned a Maria
    /// </summary>
    private float timePassed=6;
    
    /// <summary>
    /// Time between a new spawn of Maria
    /// </summary>
    private float spawnRate = 6;
    
    void Update()
    {
        if (!GameOverScreen.GameOver)
        {
            timePassed += Time.deltaTime;
            if (timePassed > spawnRate)
            {
                timePassed = 0;


                Vector3 pos = transform.position;
                pos.y -= 0.1f;

                var m = Instantiate(GameManager.maria, pos, Quaternion.identity);
                GameManager.mariaList.Add(m);
            }
        }
    }
}