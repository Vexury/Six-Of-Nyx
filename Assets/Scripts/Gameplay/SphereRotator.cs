using UnityEngine;

public class SphereRotator : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float rotationSpeed = 90f;

    private Rigidbody _rb;
    private Vector2 _moveInput;
    private float _rollInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputReader.MoveEvent += OnMove;
        inputReader.RollEvent += OnRoll;
    }

    private void OnDisable()
    {
        inputReader.MoveEvent -= OnMove;
        inputReader.RollEvent -= OnRoll;
        _moveInput = Vector2.zero;
        _rollInput = 0f;
    }

    private void OnMove(Vector2 input) => _moveInput = input;
    private void OnRoll(float input) => _rollInput = input;

    private void FixedUpdate()
    {
        if (_moveInput == Vector2.zero && _rollInput == 0f) return;

        Quaternion yRot = Quaternion.AngleAxis(-_moveInput.x * rotationSpeed * Time.fixedDeltaTime, Vector3.up);
        Quaternion xRot = Quaternion.AngleAxis(_moveInput.y * rotationSpeed * Time.fixedDeltaTime, Vector3.right);
        Quaternion zRot = Quaternion.AngleAxis(-_rollInput * rotationSpeed * Time.fixedDeltaTime, Vector3.forward);
        _rb.MoveRotation(yRot * xRot * zRot * _rb.rotation);
    }
}
