using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour, Damagable
{
    private float life = 1000;
    private float maxLife = 1000;
    public Slider Slider;
    public static Tower instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        updateScrollBar();
    }

    private void spawnMutant()
    {
        Vector3 m = transform.position;
        m.y -= 0.3f;

        var mu = Instantiate(GameManager.mutant, m, Quaternion.identity);
        GameManager.MutantList.Add(mu);
    }

    private float timepassed = 0;

    void Update()
    {
        timepassed += Time.deltaTime;

        if (timepassed > 10)
        {
            timepassed = 0;
            spawnMutant();
        }
    }

    private void updateScrollBar()
    {
        Slider.value = life / maxLife;
    }

    

    public void takeDamage(float damage)
    {
        life -= damage;
        updateScrollBar();
        if (life <= 0)
        {
            GameOverScreen.instance.won();
        }
    }

    public bool isAlive()
    {
        return life > 0;
    }
}