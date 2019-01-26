using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum EState
    {
        Tutorial,
        Game,
        EndGame
    }

    [SerializeField]
    private WorldSpace m_Space = null;
    [SerializeField]
    private UIManager m_UIManager = null;
    private delegate void UpdateState(float dt);
    private UpdateState m_UpdateState = null;

    public static GameController instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("show tutorial");
        m_UpdateState = _UpdateTutorial;
        m_Space.Init();    
        m_UIManager.Init();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        m_UpdateState(dt);
    }

    void _UpdateTutorial(float dt)
    {
    }


    void _UpdateGame(float dt)
    {   
        bool reachEnd = m_Space.UpdateWorldSpace(dt);
        if(reachEnd)
        {
            Debug.Log("Change to end game state");
            m_UIManager.EnableMenu(EState.EndGame);
            m_UpdateState = _UpdateEnd;
        }
    }

    void _UpdateEnd(float dt)
    {
        
    }

    public void CloseTutorial()
    {
        Debug.Log("Change to game state");
        m_UIManager.EnableMenu(GameController.EState.Game);
        m_UpdateState = _UpdateGame;
    }
}
