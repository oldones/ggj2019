using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    
    [SerializeField]
    private float m_AngularRotation; //angles p/ sec
    [SerializeField]
    private float m_MaxSpeed; //units p/ sec

    private float m_SpeedIncrement;
    private Vector3 m_DirVec;

    private float m_CurrPitch;
    private float m_CurrYaw;
    private float m_CurrRoll;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _HandleInput();
        _UpdatePosition();
    }

    //Listen to user input and update pitch/yaw/roll
    private void _HandleInput(){
        //yaw and roll working together cause simpler
        Vector3 dir = Vector2.zero;

        if(Input.GetKey(KeyCode.RightArrow)){
            dir.x = 1f;
        }
        if(Input.GetKey(KeyCode.LeftArrow)){
            dir.x = -1f;
        }
        if(Input.GetKey(KeyCode.UpArrow)){
            dir.y = 1f;
        }
        if(Input.GetKey(KeyCode.DownArrow)){
            dir.y = -1f;
        }
        if(Input.GetKey(KeyCode.A)){
            //accel
            dir.z = 1f;
        }
        if(Input.GetKey(KeyCode.Z)){
            //brake
            dir.z = -1f;
        }
        m_DirVec = dir;
    }

    float dampenx = 0f;
    float dampeny = 0f;
    
    Vector3 newDir = Vector3.zero;
    

    private void _UpdatePosition(){
        
        // if(m_DirVec.x != 0 && dampenx < m_AngularRotation){
        //     dampenx += Time.deltaTime;
        // }
        // else if(m_DirVec.x == 0 && dampenx > 0f){
        //     dampenx -= Time.deltaTime;
        // }

        newDir = Quaternion.AngleAxis(transform.localRotation.x + (m_AngularRotation * m_DirVec.x), Vector3.up) * transform.forward;

        // Quaternion rotation = transform.localRotation * Quaternion.AngleAxis(m_AngularRotation * m_DirVec.x, Vector3.up);

        Quaternion q = Quaternion.FromToRotation(transform.forward, newDir);

        // transform.localRotation = q;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, q, Time.deltaTime);

        // transform.forward = Vector3.Lerp(transform.forward, newDir, Time.deltaTime);

        transform.Rotate(m_DirVec.x * Vector3.up, Space.World);
        // transform.Rotate(m_DirVec.y * Vector3.right * m_AngularRotation, Space.Self);

    }

}
