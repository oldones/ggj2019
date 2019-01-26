using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private WorldSpace m_Space;

    // Start is called before the first frame update
    void Start()
    {
        m_Space.Init();    
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        m_Space.UpdateWorldSpace(dt);
    }
}
