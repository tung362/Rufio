using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/*
--------------------------------
Author: Tung Nguyen

Purpose: Like the LevelTracker script in which it stores data for the gameobjects to use except
storing it here will save the data to be used on the next loaded scene also. Many scripts will
rely on this script.

Script communicate with: LevelTracker(For spawning the object this script is attached to, not
directly)

Used by: Almost everything

Last edited: Tung Nguyen
--------------------------------
*/

public class GlobalVars : MonoBehaviour
{
    //Stats
    public float MaxHealth = 100;
    public float CurrentHealth = 100;
    public float PreviousHealth = 100;
    public float MaxEnergy = 100;
    public float CurrentEnergy = 100;
    public float MaxSpecial = 100;
    public float CurrentSpecial = 100;

    //Timers
    public bool StartHealthRegen = false; //When taking damage, set to false
    public float HealthRegenStartTimer = 0; //When taking damage, set to 0
    public float HealthRegenStartDelay = 4.0f;
    public float HealthRegenTimer = 0;
    public float HealthRegenDelay = 0.1f;

    public float EnergyRegenTimer = 0;
    public float EnergyRegenDelay = 0.2f;

    //Misc
    public bool Pause = false;
    public bool Save = false;
    public bool Load = false;

    //Tests
    public bool DebugMode = false;

    public string MainMenu;

    //void Awake()
    //{
    //    Cursor.visible = false;
    //    Cursor.lockState = CursorLockMode.Confined;
    //    Cursor.lockState = CursorLockMode.Locked;
    //}

    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    void Update()
    {
        PauseGame();
        SaveGame();
        LoadGame();


        //Health Regen
        if (CurrentHealth < MaxHealth)
        {
            if(CurrentHealth < PreviousHealth)
            {
                StartHealthRegen = false;
                HealthRegenStartTimer = 0;
            }

            if (StartHealthRegen == false)
            {
                HealthRegenStartTimer += Time.deltaTime;
                if(HealthRegenStartTimer >= HealthRegenStartDelay)
                {
                    HealthRegenStartTimer = 0;
                    StartHealthRegen = true;
                }
            }

            if (StartHealthRegen == true)
            {
                HealthRegenTimer += Time.deltaTime;
                if (HealthRegenTimer >= HealthRegenDelay)
                {
                    CurrentHealth += 5;
                    EnergyRegenTimer = 0;
                }
            }
            PreviousHealth = CurrentHealth;
        }
        else
        {
            if (StartHealthRegen == true)
            {
                PreviousHealth = MaxHealth;
                HealthRegenStartTimer = 0;
                StartHealthRegen = false;
            }
        }
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth; //Limit
        if (CurrentHealth < 0) CurrentHealth = 0; //Limit

        //Energy Regen
        if(CurrentEnergy < MaxEnergy)
        {
            EnergyRegenTimer += Time.deltaTime;
            if(EnergyRegenTimer >= EnergyRegenDelay)
            {
                CurrentEnergy += 5;
                EnergyRegenTimer = 0;
            }
        }
        if (CurrentEnergy > MaxEnergy) CurrentEnergy = MaxEnergy; //Limit
        if (CurrentEnergy < 0) CurrentEnergy = 0; //Limit

        //Special Regen
        if (CurrentSpecial > MaxSpecial) CurrentSpecial = MaxSpecial; //Limit
        if (CurrentSpecial < 0) CurrentSpecial = 0; //Limit

        //Debug Toggle
        if (Input.GetButtonDown("Debug"))
        {
            if (DebugMode == true) DebugMode = false;
            else DebugMode = true;
        }

        //Test
        if (Input.GetKeyDown(KeyCode.H)) CurrentHealth += 5;
        if (Input.GetKeyDown(KeyCode.G)) CurrentHealth -= 5;
        if (Input.GetKeyDown(KeyCode.B)) CurrentEnergy += 5;
        if (Input.GetKeyDown(KeyCode.V)) CurrentEnergy -= 5;
        if (Input.GetKeyDown(KeyCode.J)) CurrentSpecial += 5;
        if (Input.GetKeyDown(KeyCode.K)) CurrentSpecial -= 5;

        //Reload the scene
        if (Input.GetButtonDown("Reload")) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //Back to main menu
        if (Input.GetButtonDown("Pause"))
        {
            SceneManager.LoadScene(MainMenu);
        }
    }

    //Pause the game
    void PauseGame()
    {
        if (Pause == true) Time.timeScale = 0;
        else if (Time.timeScale == 0 && Pause == false) Time.timeScale = 1;
    }

    //Saves the game
    void SaveGame()
    {
        if(Save == true)
        {
            Save = false;
        }
    }

    //Loads the game
    void LoadGame()
    {
        if (Load == true)
        {
            Load = false;
        }
    }
}
