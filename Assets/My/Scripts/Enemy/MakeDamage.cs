using UnityEngine;

public class MakeDamage : MonoBehaviour
{
    [SerializeField] private int _damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Movement player =  collision.GetComponent<Movement>();
        if (player != null)player.TakeDamage(_damage);
    }
}
