using UnityEngine;

public class MariaSpawner : MonoBehaviour
{
    private float timePassed;
    private float spawnRate = 5;

   
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