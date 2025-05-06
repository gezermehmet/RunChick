using UnityEngine;

public class TestScript : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float moveForce;
    [SerializeField] private float jumpForce;

    private bool _isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
        Jump();
    }

    public void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical);

        rb.AddForce(movement * moveForce , ForceMode.Force);
        
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce , ForceMode.Impulse);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
                               
    }
}