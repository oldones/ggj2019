using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController2 : MonoBehaviour
{
    [SerializeField]
    float m_TurnSpeed;   
    [SerializeField]
    float m_Speed;
    [SerializeField]
    float m_StrafeSpeed;
    [SerializeField]
    private float m_MaxSpeed;
    [SerializeField]
    private float m_MaxReverseSpeed;

    [Header("Do not edit")]
    [SerializeField]
    private float m_TrueSpeed = 0.0f;

    private Rigidbody m_Rigidbody;
    private float m_Roll;
    private float m_Pitch;
    private float m_Yaw;
    private float m_Power;
    private Vector3 m_Strafe;

    void Awake(){
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update () {

        _HandleInput();
        _ApplyForces();

    }

    private void _ApplyForces(){
        //Truespeed controls
        m_TrueSpeed += m_Power;

        //cap values
        m_TrueSpeed = Mathf.Max(-m_MaxReverseSpeed, Mathf.Min(m_MaxSpeed, m_TrueSpeed));

        m_Rigidbody.AddRelativeTorque(m_Pitch*m_TurnSpeed*Time.deltaTime, m_Yaw*m_TurnSpeed*Time.deltaTime, m_Roll*m_TurnSpeed*Time.deltaTime);
        m_Rigidbody.AddRelativeForce(0,0,m_TrueSpeed*m_Speed*Time.deltaTime);
        m_Rigidbody.AddRelativeForce(m_Strafe);
    }

    private void _HandleInput(){
        m_Roll = 0f;
        m_Pitch = 0f;
        m_Yaw = 0f;
        m_Power = 0f;
        m_Strafe = Vector3.zero;

        if(Input.GetKey(KeyCode.LeftArrow))
            m_Roll = 1f;
        if(Input.GetKey(KeyCode.RightArrow))
            m_Roll = -1f;
        if(Input.GetKey(KeyCode.UpArrow))
            m_Pitch = -1f;
        if(Input.GetKey(KeyCode.DownArrow))
            m_Pitch = 1f;
        if(Input.GetKey(KeyCode.Q))
            m_Yaw = -1f;
        if(Input.GetKey(KeyCode.E))
            m_Yaw = 1f;
            
        m_Strafe = new Vector3(Input.GetAxis("Horizontal")*m_StrafeSpeed*Time.deltaTime, Input.GetAxis("Vertical")*m_StrafeSpeed*Time.deltaTime, 0);

        if(Input.GetKey(KeyCode.Z))
            m_Power = 1f;
        if(Input.GetKey(KeyCode.X))
            m_Power = -1f;

        if (Input.GetKey("backspace")){
            m_TrueSpeed = 0;
        }
    }

}
