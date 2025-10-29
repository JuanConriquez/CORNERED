using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float health = 3f; //takes 3 bullets
    
    public void TryHit(WeaponType weapon, float damage)
    {
        health -= damage;  //loses health per hit (3 bullets)
        Debug.Log($"{name} took {damage} damage from {weapon}, HP now {health}"); //terminal information since i dont have time to animate hits and death

        if (health <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"{name} died!");
        Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
