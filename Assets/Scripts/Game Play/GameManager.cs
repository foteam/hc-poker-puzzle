using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public class  ChipPacks
    {
        public GameObject prefab;
        public int number;
    }

    public static List<ChipPacks> chipPacks;
    public static int spawnedChipsID;
    public static int maxChipNumber = 10;
    
}
