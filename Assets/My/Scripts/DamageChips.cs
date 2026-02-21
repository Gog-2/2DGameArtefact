using System;
using UnityEngine;

public class DamageChips : MonoBehaviour
{
    [SerializeField] private int damageChips;

    private void OnCollisionEnter2D(Collision2D other)
    {
        EnemyBase enemy = other.transform.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(damageChips);
            return;
        }
        Movement player = other.transform.GetComponent<Movement>();
        if (player != null)
        {
            player.TakeDamage(damageChips);
        }
    }
}
