﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "CreatManagerVarsContainer")]
public class ManagerVars : ScriptableObject
{
    public static ManagerVars GetManagerVars()
    {
        return Resources.Load<ManagerVars>("ManagerVarsContainer");
    }
    public List<Sprite> bgThemeSpriteList = new List<Sprite>();
    public List<Sprite> platformThemeSpritList = new List<Sprite>();

    public GameObject characterPre;
    public GameObject normalPlatformPre;
    public GameObject diamondPre;

    public List<GameObject> commonPlatformGroup = new List<GameObject>();
    public List<GameObject> grassPlatformGroup = new List<GameObject>();
    public List<GameObject> winterPlatformGroup = new List<GameObject>();

    public GameObject spikePlatformLeft;
    public GameObject spikePlatformRight;
    public GameObject deathEffect;
    public float nextXPos = 0.554f;
    public float nextYPos = 0.645f;

}
