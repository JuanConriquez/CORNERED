using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// weapons available to player
public enum WeaponType { Pistol, Flashlight }

// basic weapon info
[System.Serializable]
public class Weapon
{
    public WeaponType type = WeaponType.Pistol;   // default weapon
    public float fireRate = 6f;                   // how many shots per second
    public float damage = 1f;                     // damage per shot
    public float maxRange = 20f;                  // max distance hitscan can reach
    public LayerMask hitMask;                     // layer that can be hit (enemy layer)
}

// player main controller
public class PlayerMechanics : MonoBehaviour
{
    // internal reticle state (for smooth, weighty movement)
    Vector3 flashLightCirclePos;

    [Header("References")]
    public Camera cam;                // main camera (for mouse position)
    public Transform crosshair;       // small sprite that follows the mouse (orange)

    [Header("Weapons")]
    public Weapon pistol;             // main gun (scrapped shotgun idea its meant to be like resident evil survival horror)
    public Weapon flashlight;         // switch between them with mouse buttons

    [Header("Flashlight Reticle")]
    public Transform flashLightCircle; // a sprite, will be a yellow circle for flaslight beam
    public LayerMask spiritmask;        // it affects the spirit
    public float reticleSmooth = 10f;   // lower smoothing makes it feel weighty and hard to aim (WHAT WE WANT)
    public float wobbleFreq = 8f;  // wobble strentgh (character is scared)
    public float lightRadius = 1.0f;    // radius of the flashlight effect (was hardcoded 0.8f before)

    [Header("Flashlight shaking")]
    public float flashlightLag = 0.12f; //makes it feel heavier isnt 1:1 with mouse so its not too fast/not scary (I want to add this lag to the gun too just very miniscule so its not as fast)
    public float flashlightWobbleSize = 0.35f;
    public float flashlightWobbleSpeed = 9f;
    Vector3 flashVel;

    [Header("Flashlight Eyes Hit")]
    public LayerMask spiritEyeMask; //should only look for the layer we made for the eyes
    public float eyeHitBox = 0.35f; //can make smaller etc (nvm this was perfect size)


    [Header("crosshair aim")] //lets make it meaty so it feels cool
    public float clickRadius = 0.2f; //gotta be on the dot (hitbox is large rectangle so its fine - it only exludes the arms)
    public bool preferClosest = true; //if touvhing 2 enemies choose the closest layerwise

    [Header("Cursor Visuals")]
    public SpriteRenderer crosshairSR;          // drag Crosshair's SpriteRenderer
    public SpriteRenderer flashlightReticleSR;  // drag FlashlightReticle's SpriteRenderer
    public float shootPulseScale = 1.25f;       // how big the crosshair pulses when shot it makes it feel interactive 
    public float shootPulseTime = 0.08f;        // how long the pulse lasts

   
    //honestly not the greates recoil it goes up vectorially (in a line) like say a cod game but it goes back to center anyways (not doing this felt too weird and plus the gun is already slow)
    [Header("Crosshair Recoil")]
    public float crosshairJump = 0.5f;   // how far the crosshair jumps when you shoot
    public float crosshairRecovery = 8f;  // how fast it returns to the mouse
    Vector3 crosshairRecoilOffset = Vector3.zero; // internal offset that decays
    Vector3 crosshairBaseScale; //had to look into vector 3 but it is essentially a way to work with 2 points!! ( a line a vector) in our X,Y,Z planes
    float shootPulseT = 0f;

    
    float waitToShoot = 0f;           // prevents from spamming shoot
    bool flashlightOn = false;         // right mouse must be held for flashlight to come up 
    WeaponType current = WeaponType.Pistol; // naturally in pistol mode

    [Header("Audio")]
    public AudioSource gunshot;
    public AudioClip pistolShot;
    public float shotVolume = 1f;

    // Start is called before the first frame update
    void Start()
    {

        if (!cam) cam = Camera.main;   // auto-grab camera if not assigned
        Cursor.visible = false;        // hide OS cursor, we’ll use sprites instead

        // auto-grab missing SpriteRenderers to avoid wiring bugs in Inspector
        if (!crosshairSR && crosshair) crosshairSR = crosshair.GetComponent<SpriteRenderer>();             // safety
        if (!flashlightReticleSR && flashLightCircle) flashlightReticleSR = flashLightCircle.GetComponent<SpriteRenderer>(); // safety

        if (crosshair) crosshairBaseScale = crosshair.localScale;           // for pulse reset

        // initialize flashlight reticle to current mouse world so smoothing starts from the right place
        if (flashLightCircle)
        {
            flashLightCirclePos = cam.ScreenToWorldPoint(Input.mousePosition);  // start reticle at mouse
            flashLightCirclePos.z = 0f;                                         // keep on 2D plane
            flashLightCircle.position = flashLightCirclePos;                   // place it there on frame 0
        }

        // make sure these render on top of everything so you can see them
        if (crosshairSR) crosshairSR.sortingOrder = 100;                    // always visible
        if (flashlightReticleSR) flashlightReticleSR.sortingOrder = 100;    // always visible

        current = WeaponType.Pistol;   //we only want to shoot in
    }

    // Update is called once per frame
    void Update()
    {
       
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition); //again Vectore 3 is from point to point so it tracks the mouse movement 
        mouseWorld.z = 0; //we dont use the z plane in 2D

       
        if (crosshair) crosshair.position = mouseWorld + crosshairRecoilOffset; //makes the crosshair follow the vectorial movement of the recoil that we coded earlier 

        
        crosshairRecoilOffset = Vector3.Lerp(crosshairRecoilOffset, Vector3.zero, crosshairRecovery * Time.deltaTime); // lerp: "perform linear interpolation between two 3D points (vectors). It calculates a point that lies a certain percentage of the way between two given points."
        //The deltaTime will be given by us and essentailly we tell it how fast or slow we want the vecotre to recover to its initial point (pre-recoil)


        
       

       
        bool rmb = Input.GetMouseButton(1) || Input.GetKey(KeyCode.Mouse1); // old input
        bool fkey = Input.GetKey(KeyCode.F); // fallback key (was mostly for debugging but it helped when cursors werent)
        flashlightOn = rmb || fkey;


        current = flashlightOn ? WeaponType.Flashlight : WeaponType.Pistol; //if not using flaslight then ur using pistol

       
        if (crosshairSR) crosshairSR.enabled = !flashlightOn;       // pistol crosshair (orange) pops back up when not flashlight
        if (flashlightReticleSR) flashlightReticleSR.enabled = flashlightOn; // right click light beam 

       
        if (flashlightOn && flashLightCircle)
        {
            //wobble (to look shaky and nervous)
            Vector3 wobble = new Vector3(
                Mathf.Sin(Time.time * flashlightWobbleSpeed),
                Mathf.Cos(Time.time * flashlightWobbleSpeed * 1.27f), 0f) * flashlightWobbleSize;

           
            Vector3 target = mouseWorld + wobble; //the wobbling affects mouse input (the point is that its hard to control(

            //Flashlag is how behind it is to mouse which will make it feel heavy and meaty like a AAA game B)
            flashLightCirclePos = Vector3.SmoothDamp(
                //our current reticle postion which is saved and stored in vector3
                                                  //target: where we want the reticle to go this frame (mouse position + wobble)
                                                  //flashVel: internal velocity stored smoothdamp updates it and flashvel gets it
                                                  //flashlag: how long it should take it to stop wobbling and moving which makes it feel heavy)
                                                  //Mathfinity: this one is weird its used to cap velocity but since its not physically possible to go faster than mouse we wont cap
                                                  //Time.deltatime: time since last frame
                flashLightCirclePos, target, ref flashVel, flashlightLag, Mathf.Infinity, Time.deltaTime   );
            flashLightCirclePos.z = 0f; //stay on 2D plane

         
            flashLightCircle.position = flashLightCirclePos;   //move the round flaslight cursor

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.F))
    {
                var eyesOnPress = Physics2D.OverlapCircleAll(
                    flashLightCircle.position,
                    eyeHitBox,
                    spiritEyeMask   
                );

                foreach (var h in eyesOnPress)
                {
                    var eye = h.GetComponent<SpiritEyes>();
                    if (eye != null) eye.AddHit();   // sends signa; back if eyes are hit which we can use on SpiritEyes.cs to make it do things
                }
            }


            //this is to detect only eyehits i lowkey could have just made the eyes a hitbox but i wanted the ghost to slow down anywhere and die only in the eyes.
            var EyeHits = Physics2D.OverlapCircleAll(flashLightCircle.position, eyeHitBox, spiritEyeMask);




            foreach (var h in EyeHits)
            {
                var eye = h.GetComponentInParent<SpiritEyes>();
                if (eye != null)
                    eye.Expose();
            }
        }

      
        if (Input.GetMouseButton(0) && current == WeaponType.Pistol) //if the guns out the bullets are out B)
        {
            TryShoot(mouseWorld);
            shootPulseT = shootPulseTime; // trigger crosshair pulse

            //we gotta make the gunshot sound activate
            if (gunshot && !gunshot.isPlaying)
                gunshot.Play();
        }

       
        if (crosshair && crosshairSR && crosshairSR.enabled)
        {
            if (shootPulseT > 0f)
            {
                shootPulseT -= Time.deltaTime;
                float t = Mathf.Clamp01(shootPulseT / shootPulseTime);
                float s = Mathf.Lerp(1f, shootPulseScale, t); // grow then shrink
                crosshair.localScale = crosshairBaseScale * s;
            }
            else
            {
                crosshair.localScale = Vector3.Lerp(crosshair.localScale, crosshairBaseScale, 20f * Time.deltaTime);
            }
        }
    }

    // function for hitscan-style shooting  (MOVED OUT of Update to fix local function issue)
    void TryShoot(Vector3 mouseWorld)
    {
        // fire rate limiter
        if (Time.time < waitToShoot) return;
        waitToShoot = Time.time + (1f / pistol.fireRate);

        if (gunshot)
        {
            var clip = pistolShot != null ? pistolShot : gunshot.clip;
            if (clip) gunshot.PlayOneShot(clip, shotVolume);
        }

        //looks if enemy is under cursor to kill
        Collider2D[] hits = Physics2D.OverlapCircleAll(mouseWorld, clickRadius, pistol.hitMask);
        if (hits.Length == 0)
        {
            //if theres no targer it wont hit but we still want it to recoil
            KickCrosshairRandom();
            return;

        }

        //we said closest target was chosen earlier so now we make that one get damage applied
        Collider2D chosen = hits[0];
        if (preferClosest && hits.Length > 1)
        {
            float best = Vector2.Distance(hits[0].bounds.ClosestPoint(mouseWorld), mouseWorld);
            for (int i = 1; i < hits.Length; i++)
            {
                float d = Vector2.Distance(hits[i].bounds.ClosestPoint(mouseWorld), mouseWorld);
                if (d < best) { best = d; chosen = hits[i]; }
            }
        }

        //now we hurt the zombie (well most likely recycle this hit scan logic for the spirits eyes since they arent working rn)
        var z = chosen.GetComponent<Zombie>();
        if (z) z.TryHit(WeaponType.Pistol, pistol.damage);

        //RECOILLLL *might make recoil bigger or smaller if you actually hit them
        KickCrosshairAwayFrom(mouseWorld, chosen.bounds.center);

    }
  

   
    void KickCrosshairAwayFrom(Vector3 from, Vector3 to)
    {
       
        Vector3 dir = (to - from).normalized; //again vector3 is 2 points, direction from click to the thing u hit

        crosshairRecoilOffset -= dir * crosshairJump;
        // push the crosshair opposite that direction by our recoil amount (we mess with this on inspector)
    }

   
    public bool IsFlashlightOn() => flashlightOn;

    
}