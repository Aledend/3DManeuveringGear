using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
public class GasThrusters : MonoBehaviour
{
    [SerializeField]
    private PlayerInput m_PlayerInput = null;
    [SerializeField]
    private Rigidbody m_Rigidbody = null;
    [SerializeField]
    private PlayerMovement m_PlayerMovement = null;
    [SerializeField]
    private GameObject m_CameraBoomVertical = null;

    [SerializeField]
    private ParticleSystem m_GasParticleLeft = null, m_GasParticleRight = null, m_GasParticleForward = null, m_GasParticleBackward = null, m_GasParticleUp = null, m_GasParticleDown = null;

    [SerializeField]
    private Camera m_PlayerCamera = null;

    [SerializeField]
    private float m_GasForce = 50f;

    private void Awake()
    {
        m_Rigidbody = FunctionLibrary.GetIfNull(gameObject, m_Rigidbody);
        m_PlayerMovement = FunctionLibrary.GetIfNull(gameObject, m_PlayerMovement);
    }

    void Update()
    {
        HandleCanisters();
        HandleParticles();
    }

    private void HandleCanisters()
    {
        Vector3 direction = m_PlayerCamera.transform.right * m_PlayerInput.GetGasRight 
            + m_PlayerCamera.transform.forward * m_PlayerInput.GetGasForw 
            + m_PlayerCamera.transform.up * m_PlayerInput.GetGasUp;

        if(!m_PlayerMovement.IsOnGround)
        {
            Vector3 force = direction.normalized * m_GasForce * Time.deltaTime;
            float snapMultiplier = 1.05f - (Vector3.Dot(m_Rigidbody.velocity.normalized, force.normalized) + 1f) * 0.5f;
            m_Rigidbody.velocity += force * snapMultiplier;
        }

        


    }

    private void HandleParticles()
    {
        if(!m_PlayerMovement.IsOnGround)
        {
            if(Input.GetKeyDown(m_PlayerInput.GasDirForw))
            {
                m_GasParticleForward.Play();
            }
            else if (Input.GetKeyUp(m_PlayerInput.GasDirForw))
            {
                m_GasParticleForward.Stop();
            }

            if (Input.GetKeyDown(m_PlayerInput.GasDirBack))
            {
                m_GasParticleBackward.Play();
            }
            else if (Input.GetKeyUp(m_PlayerInput.GasDirBack))
            {
                m_GasParticleBackward.Stop();
            }

            if (Input.GetKeyDown(m_PlayerInput.GasDirRight))
            {
                m_GasParticleRight.Play();
            }
            else if (Input.GetKeyUp(m_PlayerInput.GasDirLeft))
            {
                m_GasParticleLeft.Stop();
            }

            if (Input.GetKeyDown(m_PlayerInput.GasDirLeft))
            {
                m_GasParticleLeft.Play();
            }
            else if (Input.GetKeyUp(m_PlayerInput.GasDirRight))
            {
                m_GasParticleRight.Stop();
            }

            if (Input.GetKeyDown(m_PlayerInput.GasDirUp))
            {
                m_GasParticleUp.Play();
            }
            else if (Input.GetKeyUp(m_PlayerInput.GasDirUp))
            {
                m_GasParticleUp.Stop();
            }

            if (Input.GetKeyDown(m_PlayerInput.GasDirDown))
            {
                m_GasParticleDown.Play();
            }
            else if (Input.GetKeyUp(m_PlayerInput.GasDirDown))
            {
                m_GasParticleDown.Stop();
            }
        }
        else
        {
            m_GasParticleForward.Stop();
            m_GasParticleBackward.Stop();
            m_GasParticleLeft.Stop();
            m_GasParticleRight.Stop();
            m_GasParticleUp.Stop();
            m_GasParticleDown.Stop();
        }
    }
}
