﻿using UnityEngine;
using System.Collections;

//Stitches the controller and animation together and also handles the inputs
public class PlayerManager : MonoBehaviour
{
    private PlayerController PC;
    private PlayerAnimation PA;

    //Inputs
    [HideInInspector]
    public bool UP;
    [HideInInspector]
    public bool DOWN;
    [HideInInspector]
    public bool LEFT;
    [HideInInspector]
    public bool RIGHT;
    [HideInInspector]
    public bool LOCKON;
    [HideInInspector]
    public bool DODGE;
    [HideInInspector]
    public bool BLOCK;
    [HideInInspector]
    public bool ATTACK;
    [HideInInspector]
    public bool WEAPON1;
    [HideInInspector]
    public bool WEAPON2;
    [HideInInspector]
    public bool WEAPON3;
    [HideInInspector]
    public bool SKILLSELECT;
    [HideInInspector]
    public bool PAUSE;
    [HideInInspector]
    public bool RELOAD;
    [HideInInspector]
    public bool DEBUG;

    //Access to global vars
    private GlobalVars Global;

    void Start()
    {
        Global = FindObjectOfType<GlobalVars>();
        PC = GetComponent<PlayerController>();
        PA = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        if(Global.Pause == false) Inputs();
        PA.Attack();
    }

    void FixedUpdate()
    {
        PA.Fall();
        PA.Animate();
        PC.Control();
    }

    void LateUpdate()
    {
    }

    void Inputs()
    {
        UP = Input.GetButton("Up");
        DOWN = Input.GetButton("Down");
        LEFT = Input.GetButton("Left");
        RIGHT = Input.GetButton("Right");
        LOCKON = Input.GetButtonDown("LockOn");
        DODGE = Input.GetButtonDown("Dodge");
        BLOCK = Input.GetButton("Block");
        ATTACK = Input.GetButton("Attack");
        WEAPON1 = Input.GetButtonDown("Weapon1");
        WEAPON2 = Input.GetButtonDown("Weapon2");
        WEAPON3 = Input.GetButtonDown("Weapon3");
        SKILLSELECT = Input.GetButtonDown("SkillSelect");
        PAUSE = Input.GetButtonDown("Pause");
        RELOAD = Input.GetButtonDown("Reload");
        DEBUG = Input.GetButtonDown("Debug");
    }
}
