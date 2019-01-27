using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class GameCoordinator : MonoBehaviour {
    

    float TimeOfNextCycle = 0.0f;
    [SerializeField] float TimeBetweenCycles = 10 ;

    [SerializeField] float cyclesToBeginAffectingPlanetFactor = 4;


    shipSpeed currentShipSpeed  = shipSpeed.NormalSpeed;

    ResourceManager resourceManagerRef;
    SectorManager sectorManager;

    int numberCycles = 0;
    int cyclesAwayFromPlanet = 0;


    private static GameCoordinator _instance;

    public static GameCoordinator getInstance()
    {
        return _instance;
    }



    void Awake()
    {
        //Check if instance already exists
        if (_instance == null)

            //if not, set instance to this
            _instance = this;

        //If instance already exists and it's not this:
        else if (_instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

    }

    // Use this for initialization
    void Start () {
        SetUpNextCycle();
        resourceManagerRef = FindObjectOfType<ResourceManager>();
        sectorManager = FindObjectOfType<SectorManager>();
    }
	
	// Update is called once per frame
	void Update () {

		if(Time.time >= TimeOfNextCycle)
        {  
            CallCycles();
            SetUpNextCycle();
            numberCycles++;
            cyclesAwayFromPlanet++;
        }
	}

    private void SetUpNextCycle()
    {
        TimeOfNextCycle = Time.time + TimeBetweenCycles;
    }

    private void CallCycles()
    {
        SectorManager.getInstance().DecayCycle();
        ResourceManager.getInstance().DecrementFuel(ResourceManager.getInstance().CalculateFuelConsumption());
        SectorManager.getInstance().DeathCycle();
        SectorManager.getInstance().HappinessCycle();

        ResourceManager.getInstance().CalculateTotalHappy();

        CheckIfGameShouldEnd();

    }

    public void CheckIfGameShouldEnd()
    {
        if(!ResourceManager.getInstance().CheckIfThereIsFuel() || !SectorManager.getInstance().CheckIfPeopleAreAlive())
        {
            if(Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
            {
                Application.Quit();
            }
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }
    }


    public int getNumberCycles()
    {
        return numberCycles;
    }

    public float getTimeAwayFromPlanet()
    {

        if (cyclesAwayFromPlanet >= cyclesToBeginAffectingPlanetFactor) {
            return Mathf.Exp(-(cyclesAwayFromPlanet - cyclesToBeginAffectingPlanetFactor));
        }
        return 1;
            
        
    }

    public shipSpeed getCurrentSpeedPolicy()
    {
        return currentShipSpeed;
    }

    public void setSpeedPolicy(shipSpeed newShipSpeed)
    {
        currentShipSpeed = newShipSpeed;
        ResourceManager.getInstance().CalculateFuelConsumption();
    }

}
