using UnityEngine;

public class GroundLooper : MonoBehaviour
{
    public Transform player;
    public float tileLength = 30f;
    public int tilesAhead = 3;

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        if(player.position.z > transform.position.z + tileLength)
        {
            transform.position += Vector3.forward * tileLength * tilesAhead;
        }
    }
}
