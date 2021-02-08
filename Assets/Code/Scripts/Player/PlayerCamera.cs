using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 5f;

    [SerializeField]
    private GameObject m_Target = null;

    [SerializeField]
    private GameObject m_CameraBoomHorizontal = null;

    [SerializeField]
    private GameObject m_CameraBoomVertical = null;

    [SerializeField]
    private Camera m_Camera = null;

    [SerializeField]
    private Vector3 m_BoomOffset = Vector3.zero;

    [SerializeField]
    private Vector3 m_CameraOffset = Vector3.zero;

    [SerializeField]
    private PlayerMovement m_PlayerMovement = null;

    private Vector3 m_CameraVelocity = Vector3.zero;

    private Vector3 m_LastTargetPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_LastTargetPosition = m_Target.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        RotateCamera();
        MoveCamera();
    }

    private void FixedUpdate()
    {
        
    }
     
    private void MoveCamera()
    {
        Vector3 m_TargetPos = Quaternion.AngleAxis(-m_Target.transform.rotation.eulerAngles.y, Vector3.up) * -(m_Target.transform.position - m_LastTargetPosition) / (Time.deltaTime * 10F);
        transform.localPosition = Vector3.Lerp(transform.localPosition, m_TargetPos, Time.deltaTime);
        //transform.localPosition = Vector3.SmoothDamp(transform.localPosition, m_TargetLocal, ref m_CameraVelocity, 1 / m_Speed, 1000f, Time.deltaTime);
        //transform.position = Vector3.SmoothDamp(transform.position, m_LastTargetPosition, ref m_CameraVelocity, 1f / m_Speed, 1000f, Time.deltaTime);

        m_LastTargetPosition = m_Target.transform.position;
           /* transform.position = new Vector3(
                    Mathf.Lerp(transform.position.x, m_Target.transform.position.x, Time.deltaTime * 5f),
                    Mathf.Lerp(transform.position.y, m_Target.transform.position.y, Time.deltaTime * 5f),
                    Mathf.Lerp(transform.position.z, m_Target.transform.position.z, Time.deltaTime * 5f)
                    );*/

            //transform.position = Vector3.Lerp(transform.position, m_Target.transform.position, Time.deltaTime * 5f);
            //transform.position = Vector3.Lerp(transform.position, m_Target.transform.position, );
        
    }

    private void RotateCamera()
    {
        //if(m_CameraBoomHorizontal)
            //m_CameraBoomHorizontal.transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X"), Vector3.up); 
        
        if(m_CameraBoomVertical)
        {
            m_CameraBoomVertical.transform.rotation *= Quaternion.AngleAxis(-Input.GetAxis("Mouse Y"), Vector3.right);
        }
    }

    private void OnValidate()
    {
        if(m_CameraBoomHorizontal)
            m_CameraBoomHorizontal.transform.localPosition = m_BoomOffset;
        if(m_Camera)
            m_Camera.transform.localPosition = m_CameraOffset;
    }
}
