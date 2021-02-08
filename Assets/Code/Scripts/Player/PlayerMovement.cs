using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody m_RigidBody = null;
    private CapsuleCollider m_CapsuleComponent = null;
    private Quaternion m_PlayerRotation = Quaternion.identity;


    [SerializeField]
    private float m_Speed = 5f;

    [SerializeField]
    private float m_JumpForce = 5f;

    [SerializeField]
    private KeyCode m_JumpKey = KeyCode.Space;

    [SerializeField]
    private float m_GroundMaxSlope = 30f;
    private float m_CosGroundMaxSlope;

    private bool m_IsSliding = false;
    private bool m_IsOnGround = false;
    private float m_GroundOffset = 0.1f;

    private Vector3 m_InputVelocity = Vector3.zero;
    private Vector3 m_LastInputVelocity = Vector3.zero;
    private Vector3 m_RisingGravity = Vector3.down * 9.8f;
    private Vector3 m_FallingGravity = Vector3.down * 12f;

    private Vector3 m_Velocity = Vector3.zero;

    public bool IsOnGround => m_IsOnGround;
    public Vector3 InputVelocity => m_InputVelocity;

    private void Awake()
    {
        m_RigidBody = FunctionLibrary.GetIfNull(gameObject, m_RigidBody);
        m_CapsuleComponent = FunctionLibrary.GetIfNull(gameObject, m_CapsuleComponent);

        m_CosGroundMaxSlope = Mathf.Cos(Mathf.Deg2Rad * m_GroundMaxSlope);
    }


    void Update()
    {
        ApplyGravity();
        UpdateInputVelocity();
        UpdateIsOnGround();
        UpdateFriction();
        CheckJump();

        MovePlayer();
        RotatePlayer();

        m_LastInputVelocity = m_InputVelocity;
    }

    void ApplyGravity()
    {
        m_RigidBody.velocity += (m_RigidBody.velocity.y > 0f ? m_RisingGravity : m_FallingGravity) * Time.deltaTime;
    }

    private void UpdateInputVelocity()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float frameSpeed = Mathf.Sqrt(Mathf.Pow(horizontal, 2f) + Mathf.Pow(vertical, 2f)) * m_Speed;

        m_InputVelocity = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * (new Vector3(horizontal, 0f, vertical).normalized * frameSpeed);
    }

    private void MovePlayer()
    {
        //Debug.Log(m_InputVelocity);
        if (IsOnGround && !m_IsSliding)
        {
            transform.position += m_InputVelocity * Time.deltaTime;// - m_LastInputVelocity;
        }
    }

    private RaycastHit[] m_GroundCheckBuffer = new RaycastHit[10];
    private void UpdateIsOnGround()
    {
        Vector3 capsuleCenter = transform.position + m_CapsuleComponent.center;
        Vector3 capsuleDirection = GetCapsuleColliderDirection();
        float sphereOffset = m_CapsuleComponent.height * 0.5f - m_CapsuleComponent.radius + m_GroundOffset;

        Vector3 point1 = capsuleCenter + capsuleDirection * sphereOffset;
        Vector3 point2 = capsuleCenter + capsuleDirection * -sphereOffset;
        float radius = m_CapsuleComponent.radius + m_GroundOffset;

        int hitCount = Physics.CapsuleCastNonAlloc(point1, point2, radius, Vector3.down, m_GroundCheckBuffer, 0f);

        m_IsOnGround = false;
        for(int i = 0; i < hitCount; i++)
        {
            if(m_GroundCheckBuffer[i].transform.TryGetComponent<GroundComponent>(out GroundComponent ground))
            {
                if(Vector3.Dot(m_GroundCheckBuffer[i].normal, Vector3.up) > m_CosGroundMaxSlope)
                {
                    m_IsOnGround = true;
                    break;
                }
            }
        }
    }

    private void UpdateFriction()
    {
        m_RigidBody.drag = IsOnGround ? 2f : 0f;
    }

    private Vector3 GetCapsuleColliderDirection()
    {
        switch(m_CapsuleComponent.direction)
        {
            case 0:
                return transform.right;
            case 1:
                return transform.up;
            case 2:
                return transform.forward;
        }
        return Vector3.zero;
    }

    private void CheckJump()
    {
        if(IsOnGround && Input.GetKeyDown(m_JumpKey))
        {
            m_RigidBody.AddForce(transform.up * m_JumpForce);
            m_RigidBody.velocity += m_InputVelocity;
        }
    }

    private void RotatePlayer()
    {
        m_PlayerRotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X"), Vector3.up);
        transform.rotation = m_PlayerRotation;
    }
}
