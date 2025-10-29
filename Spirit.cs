using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : MonoBehaviour
{
    [Header("spirit stats")]
    public float speed = 1.4f;
    public float slowed = 0.35f; //scrappeddddd

   
    [Header("death by hits")]
    public int deathHits = 3;      // 3 eye hits seems appropriate since you can "SPAM" a bit but on higher speed you have to track eyes which is hard
    int hits = 0;

    Transform player;
    Rigidbody2D rb;
    bool isLit = false;               // still used to slow while the beam is on (ABANDONED MECHANIC WHERE HITTING THEM ANYWHERE SLOWED THEM DOWN) game was too easy

    [SerializeField] bool debugSpirit = false;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform; //should be part of the rigidbod2d editor but it was giving me trouble
        rb = GetComponent<Rigidbody2D>();
        if (rb) rb.gravityScale = 0f;
    }

    void Update()
    {
        if (!player) return;

       
        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized; //seek out player vectorially (same as zombie just alternating speeds)
        float mult = isLit ? slowed : 1f;
        if (rb) rb.velocity = dir * speed * mult;
    }

    void LateUpdate()
    {
        
        isLit = false;// reset per-frame “lit” so you must keep the light on to slow it
    }

    // called every frame the beam is on the eyes (for slow) (again scrapped feature)
    public void ExposeToLight()
    {
        isLit = true;
    }

    
    public void AddEyeHit()
    {
        hits++;
        if (debugSpirit) Debug.Log($"[Spirit] Eye HIT {hits}/{deathHits}"); //was used to debug bc eyes were tricky due to layering but we got it now. I kpet bc it gave hitbox info on console
        if (hits >= deathHits)
        {
            if (debugSpirit) Debug.Log("[Spirit] Banished!");
            Destroy(gameObject);
        }
    }

    // immune to bullets
    public void TryHit(WeaponType weapon, float damage) { }
}