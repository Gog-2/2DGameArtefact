using UnityEngine;

public class Movement : MonoBehaviour
{
    private InputSystem_Actions _inputSystemActions;
    private Vector2 _moveVector;
    private Rigidbody2D _rb;
    [SerializeField] private float _speed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _inputSystemActions = new InputSystem_Actions();
        _inputSystemActions.Player.Move.performed += ctx => _moveVector = ctx.ReadValue<Vector2>();
        _inputSystemActions.Player.Move.canceled += ctx => _moveVector = Vector2.zero;
    }

    private void OnEnable()
    {
        _inputSystemActions.Enable();
    }

    private void OnDisable()
    {
        _inputSystemActions.Disable();
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _moveVector * Time.fixedDeltaTime * _speed);
    }
    
}
