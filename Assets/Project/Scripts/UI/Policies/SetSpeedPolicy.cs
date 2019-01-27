using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSpeedPolicy : MonoBehaviour
{

    Button buttonRef;

    [SerializeField] shipSpeed speedPolicyToApply;
    

    Color OGNormalColor;
    Color OGHighlighted;

    // Use this for initialization
    void Start()
    {
        buttonRef = GetComponent<Button>();
        buttonRef.onClick.AddListener(ApplySpeedPolicy);
        
        OGNormalColor = buttonRef.colors.normalColor;
        OGHighlighted = buttonRef.colors.highlightedColor;
        if (GameCoordinator.getInstance().getCurrentSpeedPolicy() == speedPolicyToApply)
        {
            changeColorToActive();
        }


    }

    public void changeColorToActive()
    {
        var colors = buttonRef.colors;
        colors.normalColor = buttonRef.colors.pressedColor;
        colors.highlightedColor = buttonRef.colors.pressedColor;
        buttonRef.colors = colors;
    }

    public void changeColorToNormal()
    {
        var colors = buttonRef.colors;
        colors.normalColor = OGNormalColor;
        colors.highlightedColor = buttonRef.colors.pressedColor;
        buttonRef.colors = colors;
    }


    public void ApplySpeedPolicy()
    {
        GameCoordinator.getInstance().setSpeedPolicy(speedPolicyToApply);

        changeColorToActive();
    }


}

