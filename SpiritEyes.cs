using UnityEngine;

public class SpiritEyes : MonoBehaviour
{
    public Spirit parent;  // drag the parent Spirit here in Inspector

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true; // eyes collider must be a trigger
    }

    // called every frame while the reticle is on the eyes (for slow)
    public void Expose()
    {
        if (parent) parent.ExposeToLight();
    }

    // === NEW: called on flashlight *press* while aiming at the eyes (for hits) ===
    public void AddHit()
    {
        if (parent) parent.AddEyeHit();
    }
}
