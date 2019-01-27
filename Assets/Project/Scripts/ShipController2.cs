using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#pragma warning disable CS0649 //serialize field bullshit

public class ShipController2 : MonoBehaviour
{
    [Header("Control scheme")]
    [SerializeField]
    private bool m_UseMouse = false;
    [Range(0f, 5f)]
    [SerializeField]
    private float m_MouseSensitivity = 1f;
    [SerializeField]
    private bool m_InvertMousePitch = false;
    [SerializeField]
    private bool m_SimpleMode = true;
    [Header("Ship params")]
    [SerializeField]
    float m_TurnSpeed;   
    [SerializeField]
    float m_Speed;
    [SerializeField]
    float m_StrafeSpeed;
    [SerializeField]
    private float m_HyperdriveMultiplier;
    [SerializeField]
    private float m_MaxSpeed;
    [SerializeField]
    private float m_MaxReverseSpeed;

    [Header("RenderTex")]
    [SerializeField]
    private RenderTexController m_RenderTexController;
    [Header("UI")]
    [SerializeField]
    private GameObject m_SpeedUI;
    [SerializeField]
    private GameObject m_ResourcesUI;
    [SerializeField]
    private GameObject m_InteractUI;
    [SerializeField]
    private GameObject m_FeedbackUI;

    [Header("Do not edit")]
    [SerializeField]
    private float m_TrueSpeed = 0.0f;
    public float trueSpeed{ get{ return m_TrueSpeed;}}
    public bool hiperDriving{ get{ return m_TrueSpeed > m_MaxSpeed;}}


    private ShipConsole m_ShipConsole;
    private Rigidbody m_Rigidbody;
    private float m_Roll;
    private float m_Pitch;
    private float m_Yaw;
    private float m_Power;
    private Vector3 m_Strafe;

    private bool m_Hyperdrive;
    private float m_PrevTrueSpeed;

    private readonly Vector2 m_ScreenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

    private WarpSpeed m_WarpSpeed;

    void Awake(){
        m_ShipConsole = GetComponent<ShipConsole>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_WarpSpeed = GetComponentInChildren<WarpSpeed>();

        m_SpeedBar = m_SpeedUI.GetComponentInChildren<Image>();
        m_SpeedLabel = m_SpeedUI.GetComponentInChildren<TextMeshProUGUI>();
        m_FuelLabel = m_ResourcesUI.transform.Find("Fuel").GetComponent<TextMeshProUGUI>();
        m_ResearchLabel = m_ResourcesUI.transform.Find("Research").GetComponent<TextMeshProUGUI>();
        m_InteractLabel = m_InteractUI.transform.Find("Interact").GetComponent<TextMeshProUGUI>();
        m_FeedbackLabel = m_FeedbackUI.transform.Find("Feedback").GetComponent<TextMeshProUGUI>();
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

        if(m_Hyperdrive && m_TrueSpeed > 0f){
            m_TrueSpeed *= m_HyperdriveMultiplier;
            Mathf.Min(m_MaxSpeed * m_HyperdriveMultiplier, m_TrueSpeed);
        }

        m_Rigidbody.AddRelativeTorque(m_Pitch*m_TurnSpeed*Time.deltaTime, m_Yaw*m_TurnSpeed*Time.deltaTime, m_Roll*m_TurnSpeed*Time.deltaTime);
        m_Rigidbody.AddRelativeForce(0,0, m_TrueSpeed*m_Speed*Time.deltaTime);
        m_Rigidbody.AddRelativeForce(m_Strafe);
    }

    private bool _MousePosValid(Vector3 mPos){
        return mPos.x >= 0f && mPos.x <= Screen.width && mPos.y >= 0f && mPos.y <= Screen.height;
    }

    private void _HandleSteering(){
        m_Roll = 0f;
        m_Pitch = 0f;
        m_Yaw = 0f;
        m_Power = 0f;
        m_Strafe = Vector3.zero;

        if(!m_UseMouse){
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
        }
        else {

            Vector3 mPos = Input.mousePosition;
            if(_MousePosValid(mPos)){
                Vector2 mDelta = ((Vector2)mPos - m_ScreenCenter);
                mDelta /= m_ScreenCenter;
                // Debug.Log(mDelta);
                //pitch,yaw
                if(mDelta.magnitude > 0.2f){
                    m_Yaw = m_MouseSensitivity * mDelta.x * Mathf.Abs(mDelta.x);
                    m_Pitch = m_MouseSensitivity * (-mDelta.y * (m_InvertMousePitch ? -1f : 1f)) * Mathf.Abs(mDelta.y);
                }
                //roll
                bool m0 = Input.GetMouseButton(0);
                bool m1 = Input.GetMouseButton(1);

                if(m_SimpleMode){
                    if(m0)
                        m_Power = 1f;
                    if(m1)
                        m_Power = -1f;
                }
                else{
                    if(m0)
                        m_Roll = -1f;
                    if(m1)
                        m_Roll = 1f;
                }
            }
        }
            
        m_Strafe = new Vector3(Input.GetAxis("Horizontal")*m_StrafeSpeed*Time.deltaTime, Input.GetAxis("Vertical")*m_StrafeSpeed*Time.deltaTime, 0);

        if(m_SimpleMode)
        {
            if(Input.GetKey(KeyCode.Z))
                m_Roll = 1f;
            if(Input.GetKey(KeyCode.X))
                m_Roll = -1f;
        }
        else
        {
            if(Input.GetKey(KeyCode.Z))
                m_Power = 1f;
            if(Input.GetKey(KeyCode.X))
                m_Power = -1f;
        }

        if (Input.GetKey("backspace")){
            m_TrueSpeed = 0;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift)){
            m_PrevTrueSpeed = m_TrueSpeed;
            m_Hyperdrive = true;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift)){
            m_TrueSpeed = m_PrevTrueSpeed;
            m_Hyperdrive = false;
        }

        m_ShipConsole.SetWarp(m_Hyperdrive);
    }

    private void _HandlePanelInputs(){
        if(Input.GetKeyDown(KeyCode.V)){
            m_ShipConsole.FocusPanel(ShipConsole.EPanels.Center);
        }

        if(Input.GetKeyDown(KeyCode.B)){
            m_ShipConsole.FocusPanel(ShipConsole.EPanels.Right);
        }

        if(Input.GetKeyDown(KeyCode.C)){
            m_ShipConsole.FocusPanel(ShipConsole.EPanels.Left);
        }
        
        if(Input.GetKeyDown(KeyCode.Space)){
            m_ShipConsole.FocusPanel(ShipConsole.EPanels.None);
        }
    }

    private void _ShowResourceManagementConsole()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
               
        }
    }


    private Planet[] m_CurrentTrackedPlanets;

    private void _HandleConsoleInput(){
        if(Input.GetKeyDown(KeyCode.T)){
            if(m_RenderTexController != null){
                m_CurrentTrackedPlanets = m_ShipConsole.ScanClosestPlanet(3);
                m_RenderTexController.SetupNavigationPreview(m_CurrentTrackedPlanets);
                hasScanned = true;
            }
        }
        if(Input.GetKeyDown(KeyCode.Return)){
            if(!m_ShipConsole.IsSteering && m_CurrentTrackedPlanets != null){
                int index = m_ShipConsole.GetCurrentPanelIndex();
                m_ShipConsole.FlyToPlanet(m_CurrentTrackedPlanets[index]);
                m_ShipConsole.FocusPanel(ShipConsole.EPanels.None);
                hasScanned = false;
            }
        }
    }

    private Image m_SpeedBar;
    private TextMeshProUGUI m_SpeedLabel;
    private TextMeshProUGUI m_FuelLabel;
    private TextMeshProUGUI m_ResearchLabel;
    private TextMeshProUGUI m_InteractLabel;
    private TextMeshProUGUI m_FeedbackLabel;

    private bool hasScanned = false;

    private void _UpdateUIElements(){
        if(m_SpeedLabel != null)
            m_SpeedLabel.text = m_TrueSpeed.ToString();
        if(m_SpeedBar != null){
            Vector3 targetScale = m_SpeedBar.transform.localScale;
            targetScale.x = m_TrueSpeed / m_MaxSpeed;
            targetScale.x /= m_Hyperdrive ? (m_HyperdriveMultiplier / 3f) : 1f;
            m_SpeedBar.transform.localScale = Vector3.Lerp(m_SpeedBar.transform.localScale, targetScale, Time.deltaTime * 5f);
            m_SpeedBar.color = m_TrueSpeed > 0f ? Color.white : Color.red;
        }
        m_FuelLabel.text = string.Format("Fuel levels: {0}", m_ShipConsole.fuel.ToString());
        m_ResearchLabel.text = string.Format("Research progress: {0}", m_ShipConsole.investigation.ToString());

        string interact = "";
        if(!hasScanned)
            interact = "Scan closest planets [T]";
        else{
            if(m_ShipConsole.IsSteering)
                interact = "Select scanned planet [C][V][B]";
            else
                interact = "[Enter] to punch coordinates [Space] to cancel";
        }
        m_InteractLabel.text = interact;
    }

    Coroutine feedbackRoutine = null;

    private IEnumerator FeedbackRoutine(string s, float delay){
        //show label
        m_FeedbackLabel.text = string.Format("Got {0}", s);
        yield return new WaitForSeconds(delay);
        //hide label
        m_FeedbackLabel.text = "";
    }

    public void ShowFeedback(string f){
        if(feedbackRoutine != null){
            StopCoroutine(feedbackRoutine);
            feedbackRoutine = null;
        }
        feedbackRoutine = StartCoroutine(FeedbackRoutine(f, 3f));
    }

    private void _HandleInput(){
        
        if(m_ShipConsole.IsSteering)
            _HandleSteering();
        _HandlePanelInputs();
        _UpdateUIElements();
        _HandleConsoleInput();
    }

}
