using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource laneSwitchAudioSource;
    public AudioClip laneSwitchClip;
    public AudioClip damageGruntClip;
    public float laneSwitchPitch = 1.35f;

    [Header("Lane Settings")]
    public float laneDistance = 2.5f;
    public float laneSwitchSpeed = 12f;

    [Header("Jump")]
    public float jumpForce = 4f;
    public bool isGrounded = true;
    private Rigidbody rb;

    [Header("Movement")]
    public float forwardSpeed = 8f;
    public float speedIncreaseRate = 0.05f;
    public float maxSpeed = 18f;

    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;
    public float damageCooldown = 1f;
    private bool canTakeDamage = true;

    private int currentLane = 0; // -1 for left, 0 for center, 1 for right
    private bool isAlive = true;

    // For swiping on mobile
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    public float swipeThreshold = 75f;

    private Animator characterAnimator;

    void Start()
    {
        characterAnimator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        GameManager.Instance.UpdateHealth(currentHealth, maxHealth);

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) return;

        HandleInput();
        MoveForward();
        MoveToLane();
    }

    void HandleInput()
    {
        if (laneSwitchAudioSource != null)
            laneSwitchAudioSource.pitch = laneSwitchPitch;

        // Keyboard for testing in editor
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && isGrounded)
        {
            MoveLeft();
        }

        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && isGrounded)
        {
            MoveRight();
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            Jump();
        }

        // Touch input for mobile: swipe left/right/up
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                touchEndPos = touch.position;

                Vector2 swipe = touchEndPos - touchStartPos;

                if (swipe.magnitude < swipeThreshold)
                    return;

                if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                {
                    // Horizontal swipe
                    if (swipe.x > 0 && isGrounded)
                        MoveRight();
                    else if (swipe.x < 0 && isGrounded)
                        MoveLeft();
                }
                else
                {
                    // Vertical swipe
                    if (swipe.y > 0 && isGrounded)
                        Jump();
                }
            }
        }
    }

    void Jump()
    {
        isGrounded = false;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (characterAnimator != null)
            characterAnimator.SetTrigger("Jump");
    }

    void MoveLeft()
    {
        int targetLane = Mathf.Max(-1, currentLane - 1);

        if (targetLane != currentLane)
        {
            currentLane = targetLane;
            PlayLaneSwitchSound();
        }
    }

    void MoveRight()
    {
        int targetLane = Mathf.Min(1, currentLane + 1);

        if (targetLane != currentLane)
        {
            currentLane = targetLane;
            PlayLaneSwitchSound();
        }
    }

    void PlayLaneSwitchSound()
    {
        if (laneSwitchAudioSource != null && laneSwitchClip != null)
        {
            laneSwitchAudioSource.pitch = laneSwitchPitch;
            laneSwitchAudioSource.PlayOneShot(laneSwitchClip);
            laneSwitchAudioSource.pitch = 1f;
        }
    }

    void MoveForward()
    {
        forwardSpeed = Mathf.Min(maxSpeed, forwardSpeed + speedIncreaseRate * Time.deltaTime);
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime, Space.World);
    }

    void MoveToLane()
    {
        Vector3 targetPosition = new Vector3(currentLane * laneDistance, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, laneSwitchSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            TakeDamage(1);

            if (damageGruntClip != null && isAlive)
            {
                laneSwitchAudioSource.PlayOneShot(damageGruntClip);
            }

            if (characterAnimator != null && isAlive)
                characterAnimator.SetTrigger("Stumble");

            ExplodeOnHit explosion = other.GetComponentInParent<ExplodeOnHit>();
            if (explosion != null)
            {
                explosion.Explode();
            }
        }
    }

    void TakeDamage(int damage)
    {
        if (!isAlive || !canTakeDamage) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        GameManager.Instance.UpdateHealth(currentHealth, maxHealth);

        if (characterAnimator != null)
            characterAnimator.SetTrigger("Stumble");

        StartCoroutine(DamageCooldown());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    void Die()
    {
        if (!isAlive) return;

        isAlive = false;

        if (characterAnimator != null)
        {
            characterAnimator.speed = 0f;
        }

        GameManager.Instance.GameOver();
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
