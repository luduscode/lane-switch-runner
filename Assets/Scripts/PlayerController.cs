using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Lane Settings")]
    public float laneDistance = 2.5f;
    public float laneSwitchSpeed = 12f;

    [Header("Movement")]
    public float forwardSpeed = 8f;
    public float speedIncreaseRate = 0.05f;
    public float maxSpeed = 18f;

    private int currentLane = 0; // -1 for left, 0 for center, 1 for right
    private bool isAlive = true;

    private Animator characterAnimator;

    void Start()
    {
        characterAnimator = GetComponentInChildren<Animator>();
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
        // Keyboard for testing in editor
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            currentLane = Mathf.Max(-1, currentLane - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            currentLane = Mathf.Min(1, currentLane + 1);

        // Touch input for mobile
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Input.GetTouch(0).position.x < Screen.width / 2f)
                currentLane = Mathf.Max(-1, currentLane - 1);
            else
                currentLane = Mathf.Min(1, currentLane + 1);
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
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            isAlive = false;

            if (characterAnimator != null)
                characterAnimator.speed = 0f; // Stop running animation on death

            GameManager.Instance.GameOver();
        } 
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
