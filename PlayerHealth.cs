using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float healthPoints = 3f; //total health i only want 3 lives
    float currentHealth;

    void Awake() => currentHealth = healthPoints; //when start w/ 3 lives

    public void TakeDamage(float amount)
    {
        currentHealth -= amount; //you lose per hit
        Debug.Log($"Player took {amount} damage. HP = {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("YOU DIED");
        }

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
