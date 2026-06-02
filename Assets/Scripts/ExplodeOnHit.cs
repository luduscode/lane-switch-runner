using UnityEngine;

public class ExplodeOnHit : MonoBehaviour
{
    public GameObject explosionPrefab;
    public AudioClip explosionSound;

    public void Explode()
    {
        if(explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if(explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(
                explosionSound,
                transform.position,
                1.0f
            );
        }

        gameObject.SetActive(false);
    }
}
