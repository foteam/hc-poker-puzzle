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
    public class Packs
    {
        public GameObject prefab;
        public int number;
    }
    [Serializable]
    public class Chips
    {
        public List<Packs> chipsPack;
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
            GameObject chipsPack = Instantiate(chipsPrefabs.chipsPack[rand].prefab, managers.spawnButtons[i].gameObject.transform.position,
                Quaternion.identity);
            chipsPack.name = chipsPack.name + " | " + chipsPack.GetComponent<ChipsManager>().chipPackID;
        }
    }

    public void Reset(Vector3 position)
    {
        int rand = RandomPack();
        GameObject chipsPack = Instantiate(chipsPrefabs.chipsPack[rand].prefab, position,
            Quaternion.identity);
        chipsPack.name = chipsPack.name + " | " + chipsPack.GetComponent<ChipsManager>().chipPackID;
        Debug.Log("Invoked");
    }

    public void SpawnNextPack(Vector3 position, int index)
    {
        GameObject chipsPack = Instantiate(chipsPrefabs.chipsPack[index].prefab, position,
            Quaternion.identity);
        chipsPack.name = chipsPack.name + " | " + chipsPack.GetComponent<ChipsManager>().chipPackID;

        chipsPack.GetComponent<LeanDragTranslateAlong>().enabled = false;
        chipsPack.GetComponent<LeanSelectableByFinger>().enabled = false;
        chipsPack.GetComponent<ChipsManager>().isCollectable = true;
    }

    private int RandomPack()
    {
        int random = Random.Range(0, chipsPrefabs.chipsPack.Count);
        if (chipsPrefabs.chipsPack[random].number != 0) Random.Range(0, chipsPrefabs.chipsPack.Count);
        return random;
    }
}
