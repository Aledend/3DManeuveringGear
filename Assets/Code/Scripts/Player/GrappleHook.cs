using UnityEngine;
using UnityEngine.UI;

public enum GrappleTraceMode
{
    Line,
    Sphere
}

[RequireComponent(typeof(Rigidbody))]
public class GrappleHook : MonoBehaviour
{
    [SerializeField]
    private Camera m_PlayerCamera = null;
    [SerializeField]
    private Rigidbody m_Rigidbody = null;

    [SerializeField]
    private Transform m_GrappleHookNozzle = null;
    private float m_SqrGrappleMaxDistance = 0f;

    [SerializeField]
    private GrappleTraceMode m_TraceMode = GrappleTraceMode.Line;

    [SerializeField]
    private Image m_Reticle = null;

    [SerializeField]
    private LayerMask m_TraceMask = 0;

    [SerializeField]
    private float m_GrapplingForce = 5f;

    [SerializeField]
    private KeyCode m_GrapplingKey;

    private bool m_TargetValid = false;
    private bool m_Grappling = false;
    private Vector3 m_PreviewPosition = Vector3.zero;
    private Vector3 m_TargetPosition = Vector3.zero;

    [SerializeField]
    private Vector3 m_ViewPortTracePoint = Vector3.zero;


    [SerializeField]
    private GameObject m_GrapplingHookPrefab = null;

    private Transform m_GrappleObject = null;

    private void Awake()
    {
        m_Rigidbody = FunctionLibrary.GetIfNull(gameObject, m_Rigidbody);

        m_GrappleObject = GameObject.Instantiate(m_GrapplingHookPrefab).transform;
    }

    void Update()
    {
        FindTargets();
        HandleInput();
        ConstraintPlayer();

    }

    private void ConstraintPlayer()
    {
        float sqrDistance = (m_TargetPosition - m_GrappleHookNozzle.position).sqrMagnitude;

        if (m_Grappling && sqrDistance > m_SqrGrappleMaxDistance)
        {
            Vector3 localDiff = transform.position - m_GrappleHookNozzle.position;
            transform.position = m_TargetPosition + (m_GrappleHookNozzle.position - m_TargetPosition).normalized * Mathf.Sqrt(m_SqrGrappleMaxDistance) + localDiff;

            Vector3 newDir = Vector3.Cross(-Vector3.Cross(m_Rigidbody.velocity, m_GrappleHookNozzle.position - m_TargetPosition), m_GrappleHookNozzle.position - m_TargetPosition).normalized;
            m_Rigidbody.velocity = newDir * m_Rigidbody.velocity.magnitude;
        }

        m_SqrGrappleMaxDistance = Mathf.Min(m_SqrGrappleMaxDistance, sqrDistance);
    }

    private void HandleInput()
    {
        if(m_TargetValid && Input.GetKeyDown(m_GrapplingKey))
        {
            m_TargetPosition = m_PreviewPosition;
            m_Grappling = true;

            m_SqrGrappleMaxDistance = (m_TargetPosition - m_GrappleHookNozzle.position).sqrMagnitude;
        }
        else if (m_Grappling && Input.GetKey(m_GrapplingKey))
        {
            m_GrappleObject.position = m_GrappleHookNozzle.position + (m_TargetPosition - m_GrappleHookNozzle.position) * 0.5f;
            m_GrappleObject.up = (m_TargetPosition - m_GrappleHookNozzle.position).normalized;
            m_GrappleObject.localScale = new Vector3(0.1f, Vector3.Distance(m_TargetPosition, m_GrappleHookNozzle.position), 0.1f);

            transform.position += m_GrappleObject.up * m_GrapplingForce * Time.deltaTime;

            m_GrappleObject.gameObject.SetActive(true);
        }
        else if(Input.GetKeyUp(m_GrapplingKey))
        {
            m_Rigidbody.velocity += m_GrappleObject.up * m_GrapplingForce;
            m_Grappling = false;
            m_GrappleObject.gameObject.SetActive(false);
        }
    }

    private void FindTargets()
    {

        Ray CameraRay = m_PlayerCamera.ViewportPointToRay(m_ViewPortTracePoint);

        bool result = Physics.Raycast(CameraRay, out RaycastHit hit, 100f, m_TraceMask);

        if (result)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(hit.point);
            m_Reticle.rectTransform.anchoredPosition = screenPos;

            m_Reticle.rectTransform.localScale = Vector3.one / (Vector3.Distance(m_PlayerCamera.transform.position, hit.point) / 4f);

            m_PreviewPosition = hit.point;

            Debug.DrawLine(CameraRay.origin, hit.point);
        }


        m_Reticle.enabled = m_TargetValid = result && !m_Grappling;

    }
}
