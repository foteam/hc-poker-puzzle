using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public class GUIChipsManager : MonoBehaviour
{
    [Serializable]
    public class Chips
    {
        public List<GameObject> chipsPack;
    }

    [Serializable]
    public class Managers
    {
        public List<Transform> spawnButtons;
    }
    public Chips chipsPrefabs;
    public Managers managers;
    
    public static GUIChipsManager Instance { get; private set; }

    private void Start()
    {
        SetStartChipsPack();   
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void SetStartChipsPack()
    {
        for (int i = 0; i < managers.spawnButtons.Count; i++)
        {
            int rand = RandomPack();
            GameObject chipsPack = Instantiate(chipsPrefabs.chipsPack[rand], managers.spawnButtons[i].gameObject.transform.position,
                Quaternion.identity);
            chipsPack.name = chipsPack.name + " | " + chipsPack.GetComponent<ChipsManager>().chipPackID;
        }
    }

    public void Reset(Vector3 position)
    {
        int rand = RandomPack();
        GameObject chipsPack = Instantiate(chipsPrefabs.chipsPack[rand], position,
            Quaternion.identity);
        chipsPack.name = chipsPack.name + " | " + chipsPack.GetComponent<ChipsManager>().chipPackID;
        Debug.Log("Invoked");
    }

    private int RandomPack()
    {
        int random = Random.Range(0, chipsPrefabs.chipsPack.Count);
        return random;
    }
}
