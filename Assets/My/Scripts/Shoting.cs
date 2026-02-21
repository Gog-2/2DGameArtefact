using UnityEngine;
using UnityEngine.InputSystem;

public class Shoting : MonoBehaviour
{
    [SerializeField]private Movement _movement;
    [SerializeField]private Bullet _bullet;
    [SerializeField]private Transform _aim;
    [SerializeField] private bool flipSpriteOnLeft = true;
    private Camera _camera;
    private void Start()
    {
        _bullet._playerBullet =  true;
        _camera = Camera.main;
        _movement._inputSystemActions.Player.Shoot.performed += Shoot;
    }   
    private void Update()
    {
        RotateTowardsMouse();        
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed) Instantiate(_bullet, _aim.position, _aim.rotation);
    }
    private void RotateTowardsMouse()
    {
        Vector2 mouseScreenPos;
        
            mouseScreenPos = Mouse.current != null
                ? Mouse.current.position.ReadValue()
                : (Vector2)Input.mousePosition;
        
        Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, -_camera.transform.position.z)
        );
        mouseWorldPos.z = 0f;
        
        Vector3 direction = mouseWorldPos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        
        if (flipSpriteOnLeft && _aim != null)
        {
            bool lookingLeft = Mathf.Abs(angle) > 90f;
            _aim.localScale = new Vector3(
                1f,
                lookingLeft ? -1f : 1f,
                1f
            );
        }
    }
}
