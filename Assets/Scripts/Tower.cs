using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The central tower manager
/// </summary>
public class Tower : MonoBehaviour, Damagable
{
    /// <summary>
    /// The Health points of the tower
    /// </summary>
    private float life = 1000;

    /// <summary>
    /// The max Health points of the tower
    /// </summary>
    private float maxLife = 1000;

    /// <summary>
    /// The health bar
    /// </summary>
    public Slider Slider;

    public static Tower instance;

    /// <summary>
    /// Time passed since you spawned a Mutant
    /// </summary>
    private float timepassed = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        updateScrollBar();
    }

    /// <summary>
    /// Spawn a new Mutant
    /// </summary>
    private void spawnMutant()
    {
        Vector3 m = transform.position;
        m.y -= 0.1f;

        var mu = Instantiate(GameManager.mutant, m, Quaternion.identity);
        GameManager.MutantList.Add(mu);
    }


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

    /// <inheritdoc />
    public void takeDamage(float damage)
    {
        life -= damage;
        updateScrollBar();
        if (life <= 0)
        {
            GameOverScreen.instance.won();
        }
    }

    /// <inheritdoc />
    public bool isAlive()
    {
        return life > 0;
    }
}