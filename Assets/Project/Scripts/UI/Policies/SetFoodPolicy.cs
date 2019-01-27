using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetFoodPolicy : MonoBehaviour {

    Button buttonRef;

    [SerializeField] FoodPolicy foodPolicyToApply;
    Sector sectorItAppliesTo;

    Color OGNormalColor;
    Color OGHighlighted;

	// Use this for initialization
	void Start () {
        buttonRef = GetComponent<Button>();
        buttonRef.onClick.AddListener(ApplyFoodPolicy);
        sectorItAppliesTo = GetComponentInParent<Sector>();
        OGNormalColor = buttonRef.colors.normalColor;
        OGHighlighted = buttonRef.colors.highlightedColor;
        if (sectorItAppliesTo.getActiveFoodPolicy() == foodPolicyToApply)
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

	
	public void ApplyFoodPolicy()
    {
        sectorItAppliesTo.changeFoodPolicy(foodPolicyToApply);
        changeColorToActive();
    }


}

