using UnityEngine;

public class TestScript : MonoBehaviour
{
    private float _moveSpeed = 20f;
    private Rigidbody _rigidbody;
    private Vector3 _moveDirection;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        _moveDirection = new Vector3(horizontal, 0, vertical);
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(_moveDirection * _moveSpeed);
    }
}
