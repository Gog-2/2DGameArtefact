using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _baseColor, _rageColor, _findColor;
    [SerializeField] private float _durbationColor;
    [Header("Debug")]
    private Vector3 _huntPosition;
    private Transform _player;
    private NavMeshAgent _agent;
    [SerializeField] private bool _playerInRange;
    [SerializeField] private bool _playerInHunt = false;
    [SerializeField] private int _walkPos = 0;
    
    [Header("Settings")] 
    [SerializeField]private int _health;
    [SerializeField]private float _howLongHunt = 2f;
    [SerializeField]private Transform[] _durbationPoint;
    private CancellationTokenSource cts;
    [SerializeField]private bool _shoter;
    [SerializeField]private Bullet _bullet;
    [SerializeField]private Transform _aim;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateUpAxis = false;
        _agent.updateRotation = false;
        _huntPosition = _durbationPoint[0].position;
        if(_shoter) _bullet._playerBullet = false;
    }

    private void Start()
    {
        Walking(this.destroyCancellationToken).Forget();
    }
    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    public void TakeDamage(int damage)
    {
        if (_health - damage > 0) _health -= damage;
        else Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Movement Player = collision.GetComponent<Movement>();
        if (Player != null && !_playerInHunt) HuntStart(collision.transform);
        else if (Player != null)
        {
            _playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Movement Player = collision.GetComponent<Movement>();
        if (Player != null) _playerInRange = false;
        if (_agent.velocity.magnitude > 0.1f)
        {
            Vector3 direction = _agent.velocity.normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    
    private void HuntStart(Transform targer)
    {
        _playerInRange =  true;
        _player = targer;
        _playerInHunt = true;
        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();
        ChangeColor(_rageColor,cts.Token).Forget();
        PlayerTags(cts.Token).Forget();
    }

    private async UniTask ChangeColor(Color to, CancellationToken token)
    {
        Color startColor = _spriteRenderer.color;
        float timer = 0f;
    
        while (timer < _durbationColor)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
        
            float t = timer / _durbationColor;
            _spriteRenderer.color = Color.Lerp(startColor, to, t);
        
            await UniTask.WaitForEndOfFrame(token); 
            timer += Time.deltaTime;
        }
    
        _spriteRenderer.color = to; 
    }
    
    private async UniTask PlayerTags(CancellationToken token)
    {
        while (_playerInHunt)
        {
            if (_playerInRange)
            {
                _huntPosition = _player.position;
                await UniTask.WaitForSeconds(0.2f, cancellationToken: token);
            }
            else
            {
                float timer = 0f;
                while (timer < _howLongHunt)
                {
                    await UniTask.WaitForEndOfFrame(token);
                    if (_playerInRange)
                    {
                        timer = 0f;
                        await UniTask.WaitForSeconds(1f, cancellationToken: token);
                        if (_shoter) Instantiate(_bullet, _aim.position, _aim.rotation * Quaternion.Euler(0, 0, -90));
                        continue;
                    }
                    timer += Time.deltaTime;
                }
                _playerInHunt = false;
            }
        }

        _walkPos = ClosePoint();
        _huntPosition = _durbationPoint[_walkPos].position;
        ChangeColor(_findColor, cts.Token).Forget();
        transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
            .OnComplete(() => transform.rotation = Quaternion.identity);
    }
    private void FixedUpdate()
    {
        if (_agent != null && _agent.isActiveAndEnabled && _agent.isOnNavMesh)
        {
            _agent.SetDestination(_huntPosition);

            if (_agent.velocity.magnitude > 0.1f)
            {
                Vector3 direction = _agent.velocity.normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            }
        }
    }

    private int ClosePoint()
    {
        int closeIndex = 0;
        float minDistanceF = Vector2.Distance(transform.position, _durbationPoint[0].transform.position);
        for (int i = 1; i < _durbationPoint.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, _durbationPoint[i].transform.position);
            if (distance < minDistanceF)
            {
                minDistanceF = distance;
                closeIndex = i;
            }
        }
        ChangeColor(_baseColor,cts.Token).Forget();
        return closeIndex;
    }

    private async UniTask Walking(CancellationToken token)
    {
        while (true)
        {
            await UniTask.WaitForEndOfFrame(token);
        
            if (!_playerInHunt)
            {
                if (Vector2.Distance(_durbationPoint[_walkPos].position, transform.position) < 0.1f)
                {
                    _walkPos++;
                    if (_walkPos >= _durbationPoint.Length) _walkPos = 0;
                    _huntPosition = _durbationPoint[_walkPos].position;
                }
            }
        }
    }
    
}
