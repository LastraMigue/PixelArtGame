using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;        // tiles per second (1 tile = 16px = 1 unit)
    public float jumpSpeed = 12.4f; // about 2.6 tiles of height at gravityScale 3
    public float levelWidth = 40f;  // 640px / 16
    public float camY = 6f;         // level is 12 tiles tall, centred at y = 6
    public float deathY = -3f;      // falling below this restarts the level

    Vector2 start;
    Rigidbody2D rb;
    SpriteRenderer sr;
    Camera cam;
    float move;
    bool jump, grounded, won;

#if UNITY_6000_0_OR_NEWER
    Vector2 Vel { get => rb.linearVelocity; set => rb.linearVelocity = value; }
#else
    Vector2 Vel { get => rb.velocity; set => rb.velocity = value; }
#endif

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        start = transform.position;
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.sharedMaterial = new PhysicsMaterial2D { friction = 0f }; // no sticking to walls
    }

    void Update()
    {
        if (won) { move = 0; return; }
        move = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) jump = true;
        if (move != 0) sr.flipX = move < 0;
        if (transform.position.y < deathY)
        {
            rb.position = start;
            Vel = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        Vel = new Vector2(move * speed, Vel.y);
        if (jump && grounded) Vel = new Vector2(Vel.x, jumpSpeed);
        jump = false;
        grounded = false; // OnCollisionStay2D sets it again before the next step
    }

    void LateUpdate()
    {
        // follow in X, stay inside the level, snap to the 16px grid so pixels don't shimmer
        float half = cam.orthographicSize * cam.aspect;
        float x = Mathf.Clamp(transform.position.x, half, levelWidth - half);
        cam.transform.position = new Vector3(Mathf.Round(x * 16f) / 16f, camY, -10f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Finish")) return;
        won = true;
        Invoke(nameof(End), 3f);
    }

    void OnGUI()
    {
        if (!won) return;
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = Screen.height / 8
        };
        style.normal.textColor = new Color(1f, 0.94f, 0.69f); // warm light from the palette
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "NIVEL SUPERADO!!!", style);
    }

    void End()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnCollisionStay2D(Collision2D c)
    {
        foreach (var p in c.contacts)
            if (p.normal.y > 0.5f) grounded = true;
    }
}
