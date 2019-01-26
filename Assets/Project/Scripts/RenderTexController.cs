using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTexController : MonoBehaviour
{
    private Camera[] m_Cameras;

    void Awake(){
        m_Cameras = GetComponentsInChildren<Camera>();
    }

    public void SetupNavigationPreview(Planet[] planets){
        for (int i = 0; i < planets.Length; i++)
        {
            Debug.LogFormat("Got planet {0}", planets[i].name);
            m_Cameras[i].transform.position = planets[i].transform.position;
            float dist = planets[i].transform.localScale.x * 1.5f;
            m_Cameras[i].transform.Translate(-Vector3.forward * dist, Space.World);
            m_Cameras[i].transform.LookAt(planets[i].transform.position);
        }
    }
}
