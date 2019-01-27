using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private SMenu[] m_Menus = null;


    [SerializeField]
    private UIVisiblityToogle SimulationControls = null;

    
    

    public void Init()
    {
        SMenu sm = _GetMenu(GameController.EState.Tutorial);
        Button tutButton = sm.menuObject.GetComponentInChildren<Button>();
        tutButton.onClick.AddListener(_OnClickTutorialButton);
        EnableMenu(GameController.EState.Tutorial);
    }

    private void _OnClickTutorialButton()
    {
        GameController.instance.CloseTutorial();
        GameController.instance.audioManager.PlaySFX(AudioManager.ESFX.Button1);
    }

    private void _DisableAllMenus()
    {
        foreach(SMenu s in m_Menus)
        {
            s.menuObject.SetActive(false);
        }
    }

    private SMenu _GetMenu(GameController.EState st)
    {
        foreach(SMenu s in m_Menus)
        {
            if(s.state == st)
            {
                return s;
            }
            
        }
        return new SMenu();
    } 

    public void EnableMenu(GameController.EState st)
    {
        _DisableAllMenus();
        SMenu s = _GetMenu(st);
        s.menuObject.SetActive(true);
    }



    [System.Serializable]
    public struct SMenu
    {
        public GameController.EState state;
        public GameObject menuObject;
    }

    public void ToogleSimulationControlsInterface()
    {
        SimulationControls.ToogleVisibility();
    }

    public void presentEndGame(LostGameReason reasonToLose)
    {
        switch (reasonToLose)
        {
            case LostGameReason.lackOfFuel:
                //endGameFuelRef.show();
                return;
            default:
            case LostGameReason.lackOfPeople:
                //endGamePeopleRef.show();
                return;
            
        }
    }


}

public enum LostGameReason
{
    lackOfFuel,
    lackOfPeople
}

