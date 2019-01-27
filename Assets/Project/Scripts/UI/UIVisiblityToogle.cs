using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVisiblityToogle : MonoBehaviour
{

    private CanvasGroup canvasGroup;

    private bool isVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToogleVisibility()
    {
        if (isVisible)
        {
            Hide();
            isVisible = false;
            return;
        }
        Show();
        isVisible = true;
    }


    void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

}
