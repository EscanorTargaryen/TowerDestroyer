using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private float life;
    private float maxLife=1000;
    
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }

    public void takeDamage(int damage)
    {
        life -= damage;
    }
}
