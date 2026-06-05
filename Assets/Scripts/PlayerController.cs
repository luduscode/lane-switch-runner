using UnityEngine;
using UnityEngine.UI;

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

    [Header("Movement")]
    public float forwardSpeed = 8f;
    public float speedIncreaseRate = 0.05f;
    public float maxSpeed = 18f;

    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;
    public float damageCooldown = 1f;

    private bool canTakeDamage = true;

    public Slider healthBar;

    private int currentLane = 0; // -1 for left, 0 for center, 1 for right
    private bool isAlive = true;

    private Animator characterAnimator;

    void Start()
    {
        characterAnimator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        if(healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
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
        laneSwitchAudioSource.pitch = laneSwitchPitch;

        // Keyboard for testing in editor
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            int targetLane = Mathf.Max(-1, currentLane - 1);
            if(targetLane != currentLane)
            {
                currentLane = targetLane;

                if (laneSwitchAudioSource != null && laneSwitchClip != null)
                    laneSwitchAudioSource.PlayOneShot(laneSwitchClip);
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            int targetLane = Mathf.Min(1, currentLane + 1);
            if (targetLane != currentLane)
            {
                currentLane = targetLane;

                if (laneSwitchAudioSource != null && laneSwitchClip != null)
                    laneSwitchAudioSource.PlayOneShot(laneSwitchClip);
            }
        }


        // Touch input for mobile
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Input.GetTouch(0).position.x < Screen.width / 2f)
            {
                int targetLane = Mathf.Max(-1, currentLane - 1);
                if (targetLane != currentLane)
                {
                    currentLane = targetLane;

                    if (laneSwitchAudioSource != null && laneSwitchClip != null)
                        laneSwitchAudioSource.PlayOneShot(laneSwitchClip);
                }
            }
            else
            {
                int targetLane = Mathf.Min(1, currentLane + 1);
                if (targetLane != currentLane)
                {
                    currentLane = targetLane;

                    if (laneSwitchAudioSource != null && laneSwitchClip != null)
                        laneSwitchAudioSource.PlayOneShot(laneSwitchClip);
                }
            }
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacle"))
        {
            TakeDamage(1);

            //Animator obstacleAnimator = other.GetComponentInChildren<Animator>();
            //if (obstacleAnimator != null)
            //    obstacleAnimator.speed = 0f; // Stop obstacle animation on collision
            if(damageGruntClip != null)
            {
                //laneSwitchAudioSource.pitch = Random.Range(1.75f, 2f);
                laneSwitchAudioSource.PlayOneShot(damageGruntClip);
                //laneSwitchAudioSource.pitch = 1f;
            }

            if (characterAnimator != null)
                characterAnimator.SetTrigger("Stumble");

            ExplodeOnHit explosion = other.GetComponentInParent<ExplodeOnHit>();
            if (explosion != null)
            {
                explosion.Explode();
            }

            //GameManager.Instance.GameOver();
        } 
    }

    void TakeDamage(int damage)
    {
        if (!canTakeDamage) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Health: " + currentHealth);

        if(healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (characterAnimator != null)
            characterAnimator.SetTrigger("Stumble");

        StartCoroutine(DamageCooldown());

        if(currentHealth <= 0)
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
        isAlive = false;

        if(characterAnimator != null)
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
