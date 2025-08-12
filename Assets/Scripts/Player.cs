using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public static Player instance;

    [Header("Attributes")]
    [SerializeField] private float rotatePower = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float speed = 2f;

    [Header("Ice Mode Settings")]
    [SerializeField] private float iceFallMultiplier = 1.4f;
    [SerializeField] private float iceJumpMultiplier = 0.8f;

    [Header("Transform Settings")]
    [SerializeField] private Vector3 smallSize = new Vector3(0.7f, 0.7f, 1f);
    [SerializeField] private Vector3 bigSize = new Vector3(1.3f, 1.3f, 1f);
    [SerializeField] private Vector3 defaultSize = Vector3.one;

    public event Action OnJump;
    public event Action OnDied;
    public event Action OnScoreCollected;
    public event Action<bool> OnDangerZoneChanged;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 playerStartPos;
    private bool invertedGravity = false;
    private bool iceMode = false;
    public bool InvertedGravity => invertedGravity;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ObstacleBlock.speed = speed;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        defaultSize = transform.localScale;
        playerStartPos = transform.position;
    }

    private void OnEnable()
    {
        GameManager.instance.OnGameReset += ResetToStart;
    }

    private void OnDisable()
    {
        GameManager.instance.OnGameReset -= ResetToStart;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
            animator.SetTrigger("Flap");
        }

        float rotateDir = invertedGravity ? -1f : 1f;
        transform.eulerAngles = new Vector3(0, 0, rb.linearVelocityY * rotatePower * rotateDir);

    }

    private void Jump()
    {
        float finalJump = jumpForce;
        if (iceMode) finalJump *= iceJumpMultiplier;

        Vector2 jumpDir = invertedGravity ? Vector2.down : Vector2.up;
        rb.linearVelocity = jumpDir * finalJump;
        OnJump?.Invoke();
    }

    void FixedUpdate()
    {
        if (iceMode)
        {
            float gravityBoost = 9.81f * (iceFallMultiplier - 1f) * Time.fixedDeltaTime;
            rb.linearVelocity += Vector2.down * gravityBoost;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<Collider2D>().enabled = false;
        OnDied?.Invoke();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DangerZone"))
        {
            OnDangerZoneChanged?.Invoke(true);
        }

        if (collision.CompareTag("ScoreZone"))
        {
            OnScoreCollected?.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("DangerZone"))
        {
            OnDangerZoneChanged?.Invoke(false);
        }
    }
    
    private void ResetToStart()
    {
        ResetPosition(playerStartPos);
    }

    public void ResetPosition(Vector3 pos)
    {
        rb.simulated = false;

        transform.position = pos;
        transform.rotation = Quaternion.identity;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.rotation = 0f;

        rb.simulated = true;

        ResetSize();
        SetInvertedGravity(false);
        SetIceMode(false);

        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;

    }


    public void SetInvertedGravity(bool value)
    {
        invertedGravity = value;
        transform.localScale = new Vector3(defaultSize.x, value ? -defaultSize.y : defaultSize.y, defaultSize.z);
        rb.gravityScale = value ? -2f : 3f;
    }

    public void SetIceMode(bool value)
    {
        iceMode = value;
    }

    public void RandomSize()
    {
        int rnd = UnityEngine.Random.Range(0, 2);
        transform.localScale = (rnd == 0) ? smallSize : bigSize;
    }

    public void ResetSize()
    {
        transform.localScale = defaultSize;
    }
}
