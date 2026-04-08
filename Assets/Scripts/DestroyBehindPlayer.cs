using UnityEngine;

public class DestroyBehindPlayer : MonoBehaviour
{
    public Transform player;
    public float destroyDistance = 15f;

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        if(transform.position.z < player.position.z - destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
