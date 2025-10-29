using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TouchingPoint : MonoBehaviour
{
    public PlayerHealth player;      
    public float damagePoints = 1f;  

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Awake()
    {
       
        if (!player)
        {
           
            var tagged = GameObject.FindGameObjectWithTag("Player");
            if (tagged) player = tagged.GetComponent<PlayerHealth>();
            if (!player) player = FindObjectOfType<PlayerHealth>(); //connecting little area to player so we know who to remove points from
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool isEnemy = other.GetComponent<Zombie>() != null || other.GetComponent<Spirit>() != null;
        if (!isEnemy) return;

        Debug.Log($"[TouchingPoint] Enemy reached zone: {other.name}");

        if (player)
        {
            player.TakeDamage(damagePoints); //hurt player (remove one health point) when they reach the middle (sorry it would be easier to see if they scaled uo as they got closer)
        }
        else
        {
            Debug.LogWarning("[TouchingPoint] No PlayerHealth found! Drag your Player to the 'player' field or add the 'Player' tag to the Player object.");
        }

       .
        Destroy(other.gameObject); //makes sure they can only hit u once then they die
    }
}