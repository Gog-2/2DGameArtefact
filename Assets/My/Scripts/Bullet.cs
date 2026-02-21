using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool _playerBullet = false;
    [SerializeField]private int _damage;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _rb.AddForce(10 * transform.up, ForceMode2D.Impulse);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_playerBullet) PlayerBullet(collision);
        else EnemyBullet(collision);
    }

    private void PlayerBullet(Collider2D collision)
    {
        EnemyBase enemy = collision.transform.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damage);
            Destroy(this.gameObject);
            return;
        }
        Box box = collision.transform.GetComponent<Box>();
        if (box != null)
        {
            box.TakeDamage(_damage);
        }
        MakeDamage makeDamage = collision.transform.GetComponent<MakeDamage>();
        if (makeDamage != null)return;
        Destroy(this.gameObject);
    }

    private void EnemyBullet(Collider2D collision)
    {
        Movement player = collision.transform.GetComponent<Movement>();
        if (player != null)
        {
            player.TakeDamage(_damage);
        }
        MakeDamage makeDamage = collision.transform.GetComponent<MakeDamage>();
        if (makeDamage != null)return;
        EnemyBase enemy = collision.transform.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            return;
        }
        Destroy(this.gameObject);
    }
}
