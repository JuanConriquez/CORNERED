using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWalk : MonoBehaviour
{
    public Transform target; //where theyll spawn
    public float speed = 1.6f;

    Rigidbody2D rigidbod;

    void Awake()// instead of start cuz we want this to only happen when they are instantiated (spwaned) not when program runs
    {
        rigidbod = GetComponent<Rigidbody2D>();
        if (rigidbod) rigidbod.gravityScale = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!target) return; //if the target dont exist we dgaf
        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized; //find the directional vector its at towards the touching point
        //then it tells the zombie which way to move
        GetComponent<Rigidbody2D>().velocity = dir * speed; //might make it it slower and harder to aim or faster and easier to aim
    }
}
