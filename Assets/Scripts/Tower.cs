using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    private float life = 1000;
    private float maxLife = 1000;
    public Scrollbar Scrollbar;
    public static Tower instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        updateScrollBar();
    }


    void Update()
    {
    }

    private void updateScrollBar()
    {
        Scrollbar.size = life / maxLife;
    }


    public static void towerTakeDamage(int damage)
    {
        instance.takeDamage(damage);
    }

    public void takeDamage(int damage)
    {
        life -= damage;
        updateScrollBar();
    }
}