using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public class GUI
    {
        public TextMeshProUGUI targetText;
    }
    /// <summary>
    /// All spawned chip packs in scene IDies 
    /// </summary>
    public static int spawnedChipsID;
    
    /// <summary>
    /// Max chip number value (biggest chip)
    /// </summary>
    public static int maxChipNumber = 10;

    private int _grids;

    public int target;

    [Header("GUI Objects")]
    public GUI guiElements;
    
    public static GameManager Instance { get; private set; }

    private bool _gameStatus = true;

    public List<GridSystem> gridSystems;
    public List<GameObject> activeChips;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _grids = FindObjectsOfType<GridSystem>().Length;
        StartCoroutine(CheckLossLevel());
    }
    private void LateUpdate()
    {
        if (!_gameStatus) return;

        if (target <= 0)
        {
            _gameStatus = false;
            LevelFinished();
        }
        
        guiElements.targetText.text = target.ToString();
    }

    #region Methods

    private void LevelFinished()
    {
        
    }

    private void LevelLossed()
    {
        
    }

    #endregion

    #region Coroutines

    private IEnumerator CheckLossLevel()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (gridSystems.Count == _grids && activeChips.Count == 0)
            {
                Debug.Log("Level loss");
                yield break;
            }
        }
    }

    #endregion
}
