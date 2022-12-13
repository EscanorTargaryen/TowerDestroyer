using UnityEngine;

public class MariaSpawner : MonoBehaviour
{
    private float timePassed;
    private float spawnRate = 5;

    // Update is called once per frame
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

                Instantiate(GameManager.maria, pos, Quaternion.identity);
            }
        }
    }
}