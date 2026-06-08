using UnityEngine;

public class FootstepEventReceiver : MonoBehaviour
{
    public PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    public void PlayFootstep()
    {
        if (playerController != null)
            playerController.PlayFootstep();
    }
}
