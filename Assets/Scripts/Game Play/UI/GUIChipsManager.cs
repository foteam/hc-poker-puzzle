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
        public List<LeanSpawnWithFinger> spawnButtons;
    }
    public Chips chipsPrefabs;
    public Managers managers;

    private void Start()
    {
        SetStartChipsPack();   
    }

    private void SetStartChipsPack()
    {
        for (int i = 0; i < managers.spawnButtons.Count; i++)
        {
            int rand = RandomPack();
            GameObject chipsPack = Instantiate(chipsPrefabs.chipsPack[rand], managers.spawnButtons[i].gameObject.transform.position,
                Quaternion.identity);

            managers.spawnButtons[i].Prefab = chipsPrefabs.chipsPack[rand].transform;
            
            chipsPack.GetComponent<LeanDragTranslateAlong>().enabled = false;
            chipsPack.transform.position = managers.spawnButtons[i].transform.position;
            chipsPack.transform.parent = managers.spawnButtons[i].transform;
            chipsPack.transform.localScale = new Vector3(75, 75, 75);

            chipsPack.transform.localPosition -= new Vector3(0, 0, 15);

            var packRotation = Quaternion.Euler(25f, 180, 0f);
            chipsPack.transform.rotation = packRotation;

            chipsPack.layer = LayerMask.NameToLayer("UI");
            foreach (Transform child in chipsPack.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("UI");
                foreach (Transform childer in child)
                {
                    childer.gameObject.layer = LayerMask.NameToLayer("UI");
                }
            }
        }
    }

    public void RandomizeNextPrefab(int index)
    {
        Destroy(managers.spawnButtons[index].Prefab.gameObject);
    }

    private int RandomPack()
    {
        int random = Random.Range(0, chipsPrefabs.chipsPack.Count);
        return random;
    }
}
