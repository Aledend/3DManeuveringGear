using UnityEngine;
using UnityEngine.UI;

public enum GrappleTraceMode
{
    Line,
    Sphere
}

[RequireComponent(typeof(Rigidbody))]
public class GrappleHooks : MonoBehaviour
{
    [SerializeField]
    private Camera m_PlayerCamera = null;
    [SerializeField]
    private Rigidbody m_Rigidbody = null;

    [SerializeField]
    private Transform m_LeftGrappleHookNozzle = null, m_RightGrappleHookNozzle = null;
    private float m_SqrLeftGrappleMaxDistance = 0f, m_SqrRightGrappleMaxDistance = 0f;
    private Vector3 m_LeftGrappleLastPos = Vector3.zero, m_RightGrappleLastPos = Vector3.zero;

    [SerializeField]
    private GrappleTraceMode m_TraceMode = GrappleTraceMode.Line;

    [SerializeField]
    private Image m_LeftReticle = null, m_RightReticle = null;

    [SerializeField]
    private LayerMask m_TraceMask = 0;


    [SerializeField]
    private PlayerInput m_PlayerInput = null;

    private bool m_LeftTargetValid = false;
    private bool m_RightTargetValid = false;
    private bool m_LeftGrappling = false;
    private bool m_RightGrappling = false;
    private Vector3 m_LeftPreviewPosition = Vector3.zero;
    private Vector3 m_RightPreviewPosition = Vector3.zero;
    private Vector3 m_LeftTargetPosition = Vector3.zero;
    private Vector3 m_RightTargetPosition = Vector3.zero;


    [SerializeField]
    private GameObject m_GrapplingHookPrefab = null;

    private Transform m_LeftGrapple = null;
    private Transform m_RightGrapple = null;

    private void Awake()
    {
        m_Rigidbody = FunctionLibrary.GetIfNull(gameObject, m_Rigidbody);

        m_LeftGrapple = GameObject.Instantiate(m_GrapplingHookPrefab).transform;
        m_RightGrapple = GameObject.Instantiate(m_GrapplingHookPrefab).transform;
    }

    void Update()
    {
        FindTargets();
        HandleInput();
        ConstraintPlayer();

    }

    private void ConstraintPlayer()
    {
        float sqrLeftDistance = (m_LeftTargetPosition - m_LeftGrappleHookNozzle.position).sqrMagnitude;
        float sqrRightDistance = (m_LeftTargetPosition - m_RightGrappleHookNozzle.position).sqrMagnitude;

        if (m_LeftGrappling && sqrLeftDistance > m_SqrLeftGrappleMaxDistance)
        {
            Vector3 localDiff = transform.position - m_LeftGrappleHookNozzle.position;
            transform.position = m_LeftTargetPosition + (m_LeftGrappleHookNozzle.position - m_LeftTargetPosition).normalized * Mathf.Sqrt(m_SqrLeftGrappleMaxDistance) + localDiff;

            Vector3 newDir = Vector3.Cross(-Vector3.Cross(m_Rigidbody.velocity, m_LeftGrappleHookNozzle.position - m_LeftTargetPosition), m_LeftGrappleHookNozzle.position - m_LeftTargetPosition).normalized;
            m_Rigidbody.velocity = newDir * m_Rigidbody.velocity.magnitude;
        }

        if(m_RightGrappling && sqrRightDistance > m_SqrRightGrappleMaxDistance)
        {
            Vector3 newDir = Vector3.Cross(-Vector3.Cross(m_Rigidbody.velocity, m_RightGrappleHookNozzle.position - m_RightTargetPosition), m_RightGrappleHookNozzle.position - m_RightTargetPosition).normalized;
            m_Rigidbody.velocity = newDir * m_Rigidbody.velocity.magnitude;
        }

        m_SqrLeftGrappleMaxDistance = Mathf.Min(m_SqrLeftGrappleMaxDistance, sqrLeftDistance);
        m_SqrRightGrappleMaxDistance = Mathf.Min(m_SqrRightGrappleMaxDistance, sqrRightDistance);
    }

    private void HandleInput()
    {
        if(m_LeftTargetValid && Input.GetKeyDown(m_PlayerInput.GrapplingLeft))
        {
            m_LeftTargetPosition = m_LeftPreviewPosition;
            m_LeftGrappling = true;

            m_SqrLeftGrappleMaxDistance = (m_LeftTargetPosition - m_LeftGrappleHookNozzle.position).sqrMagnitude;
            //m_LeftGrappleLastPos = m_LeftGrappleHookNozzle.position;
        }
        else if (m_LeftGrappling && Input.GetKey(m_PlayerInput.GrapplingLeft))
        {
            m_LeftGrapple.position = m_LeftGrappleHookNozzle.position + (m_LeftTargetPosition - m_LeftGrappleHookNozzle.position) * 0.5f;
            m_LeftGrapple.up = (m_LeftTargetPosition - m_LeftGrappleHookNozzle.position).normalized;
            m_LeftGrapple.localScale = new Vector3(0.1f, Vector3.Distance(m_LeftTargetPosition, m_LeftGrappleHookNozzle.position), 0.1f);


            m_LeftGrapple.gameObject.SetActive(true);
        }
        else if(Input.GetKeyUp(m_PlayerInput.GrapplingLeft))
        {
            m_LeftGrappling = false;
            m_LeftGrapple.gameObject.SetActive(false);
        }

        if(m_RightTargetValid && Input.GetKeyDown(m_PlayerInput.GrapplingRight))
        {
            m_RightTargetPosition = m_RightPreviewPosition;
            m_RightGrappling = true;

            m_SqrRightGrappleMaxDistance = (m_RightTargetPosition - m_RightGrappleHookNozzle.position).sqrMagnitude;
            //m_RightGrappleLastPos = m_RightGrappleHookNozzle.position;
        }
        else if (m_RightGrappling && Input.GetKey(m_PlayerInput.GrapplingRight))
        {
            m_RightGrapple.position = m_RightGrappleHookNozzle.position + (m_RightTargetPosition - m_RightGrappleHookNozzle.position) * 0.5f;
            m_RightGrapple.up = (m_RightTargetPosition - m_RightGrappleHookNozzle.position).normalized;
            m_RightGrapple.localScale = new Vector3(0.1f, Vector3.Distance(m_RightTargetPosition, m_RightGrappleHookNozzle.position), 0.1f);


            m_RightGrapple.gameObject.SetActive(true);
        }
        else if(Input.GetKeyUp(m_PlayerInput.GrapplingRight))
        {
            m_RightGrappling = false;
            m_RightGrapple.gameObject.SetActive(false);
        }
    }

    private void FindTargets()
    {

        Ray leftCameraRay = m_PlayerCamera.ViewportPointToRay(new Vector3(0.4f, 0.65f, 0f));
        Ray rightCameraRay = m_PlayerCamera.ViewportPointToRay(new Vector3(0.6f, 0.65f, 0f));

        RaycastHit leftHit;
        RaycastHit rightHit;
        bool leftResult;
        bool rightResult;


        leftResult = Physics.Raycast(leftCameraRay, out leftHit, 100f, m_TraceMask);
        rightResult = Physics.Raycast(rightCameraRay, out rightHit, 100f, m_TraceMask);

        if (leftResult)
        {
            Ray leftGrappleRay = new Ray(m_LeftGrappleHookNozzle.position, leftHit.point - m_LeftGrappleHookNozzle.position);

            leftResult = Physics.SphereCast(leftGrappleRay, 1f, out leftHit, 100f, m_TraceMask);
        }
        if (rightResult)
        {
            Ray rightGrappleRay = new Ray(m_RightGrappleHookNozzle.position, rightHit.point - m_RightGrappleHookNozzle.position);
            rightResult = Physics.SphereCast(rightGrappleRay, 1f, out rightHit, 100f, m_TraceMask);
        }

        if (leftResult)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(leftHit.point);
            m_LeftReticle.rectTransform.anchoredPosition = screenPos;

            m_LeftReticle.rectTransform.localScale = Vector3.one / (Vector3.Distance(m_PlayerCamera.transform.position, leftHit.point) / 4f);

            m_LeftPreviewPosition = leftHit.point;

            Debug.DrawLine(m_LeftGrappleHookNozzle.position, leftHit.point);
        }

        if (rightResult)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(rightHit.point);
            m_RightReticle.rectTransform.anchoredPosition = screenPos;

            m_RightReticle.rectTransform.localScale = Vector3.one / (Vector3.Distance(m_PlayerCamera.transform.position, rightHit.point) / 4f);

            m_RightPreviewPosition = rightHit.point;

            Debug.DrawLine(m_RightGrappleHookNozzle.position, rightHit.point);
        }

        m_LeftReticle.enabled = m_LeftTargetValid = leftResult && !m_LeftGrappling;
        m_RightReticle.enabled = m_RightTargetValid = rightResult && !m_RightGrappling;

    }
}
