using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Lean.Common;
using Lean.Touch;
using Unity.VisualScripting;
using UnityEditor;

public class ChipsManager : MonoBehaviour
{
    [Serializable]
    public class Chip
    {
        public GameObject chip;
        public int chipNumber;
    }
    
    [SerializeField] public GameObject _chipPrefab;
    [SerializeField] private List<Chip> _chips;
    
    [Range(0, 10)]
    [SerializeField] private float rayCastDistance;

    public Vector3 _startPosition;
    public Transform _stayGrid;
    
    public int generateChipTest;
    public int chipPackID;

    public bool isCollectable = false;
    public bool isBusy = false;
    private bool _chipsIncrement = false;

    private GameObject _target;
    
    private void OnEnable()
    {
        LeanSelectable.OnAnyDeselected += PackDeselected;
    }

    private void OnDisable()
    {
        LeanSelectable.OnAnyDeselected -= PackDeselected;
    }
    private void Awake()
    {
        StartCoroutine(PoolChips(generateChipTest, _chipPrefab));
        
        GameManager.spawnedChipsID += 1;
        chipPackID = GameManager.spawnedChipsID;

        _startPosition = transform.position;
    }

    private void Update()
    {
        FloorRayDetector(Vector3.down);
    }

    private void LateUpdate()
    {
        if (_chips.Count == 0) Destroy(this.gameObject);
        
        if (AreAllElementsEqual(_chips) && isCollectable)
        {
            isCollectable = false;
            StartCoroutine(Collected());
        }
        RayDetector(Vector3.forward);
        RayDetector(Vector3.back);
        RayDetector(Vector3.left);
        RayDetector(Vector3.right);
    }

    private IEnumerator Collected()
    {
        yield return new WaitForSeconds(1f);
        float destroyTime = _chips.Count / 20;

        int lastChipNumber = _chips.Last().chipNumber;
        
        for (int i = _chips.Count-1; i >= 0; i--)
        {
            destroyTime -= destroyTime / 10;
            _chips[i].chip.transform.DOScale(new Vector3(0, 0, 0), destroyTime).SetEase(Ease.OutElastic).OnComplete(
                () =>
                {
                    _chips.RemoveAt(i);
                    Destroy(_chips[i].chip);
                });
            yield return new WaitForSeconds(destroyTime);
        }

        if (lastChipNumber <= GameManager.maxChipNumber)
        {
            switch (lastChipNumber)
            {
                case 5:
                    for (int i = 0; i < GUIChipsManager.Instance.chipsPrefabs.chipsPack.Count; i++)
                    {
                        if (GUIChipsManager.Instance.chipsPrefabs.chipsPack[i].number == (lastChipNumber + lastChipNumber))
                        {
                            GUIChipsManager.Instance.SpawnNextPack(transform.position, i);
                            break;
                        }
                    }
                    break;
            }
        }
    }
    private void FloorRayDetector(Vector3 direction)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, direction);
        if (Physics.Raycast(ray, out hit, rayCastDistance, LayerMask.GetMask("Grid")))
        {
            var tag = hit.collider.gameObject.tag;
            if (tag == "Grid" && hit.collider.GetComponent<GridSystem>().isEmpty)
            {
                _stayGrid = hit.collider.gameObject.transform;
            }
        }
        else
        {
            _stayGrid = null;
        }
    }
    private void RayDetector(Vector3 direction)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out hit, rayCastDistance))
        {
            var tag = hit.collider.gameObject.tag;
            ChipsManager otherChips = hit.collider.gameObject.GetComponent<ChipsManager>();
            if (tag == "ChipsStack")
            {
                var otherPackID = otherChips.chipPackID;
                var ownPackID = chipPackID;
                
                if (otherPackID > ownPackID)
                {
                    if (_chips.Count == 0 || otherChips._chips.Count == 0) return;
                    if (isCollectable && otherChips.isCollectable && !_chipsIncrement && !isBusy && !otherChips.isBusy && !CheckForElements(_chips))
                    {
                        StartCoroutine(ChipsIncrement(otherChips));
                    }
                    else if (isCollectable && otherChips.isCollectable && !_chipsIncrement && !isBusy && !otherChips.isBusy && CheckForElements(_chips))
                    {
                        otherChips.StartCoroutine(otherChips.ChipsIncrement(gameObject.GetComponent<ChipsManager>()));
                        //StartCoroutine(ChipsIncrement(otherChips));
                    }
                }
            }
        }
        else
        {
            _target = null;
        }
    }
    
    public void PackDeselected(LeanSelect select, LeanSelectable selectable)
    {
        if (selectable.GetComponent<ChipsManager>()._stayGrid != null)
        {
            GameObject stayGrid = selectable.GetComponent<ChipsManager>()._stayGrid.gameObject;
            if (stayGrid.CompareTag("Untagged"))
            {
                Debug.Log("Grid already exits");
                selectable.transform.DOMove(selectable.GetComponent<ChipsManager>()._startPosition, 0.5f)
                    .SetEase(Ease.OutElastic);
                return;
            }
            selectable.GetComponent<LeanDragTranslateAlong>().enabled = false;
            selectable.transform.DOMove(selectable.GetComponent<ChipsManager>()._stayGrid.position, 0.5f)
                .SetEase(Ease.OutElastic).OnComplete(() =>
                {
                    selectable.GetComponent<ChipsManager>().isCollectable = true;
                    selectable.GetComponent<LeanSelectableByFinger>().enabled = false;
                });
            if (selectable.name == gameObject.name)
            {
                GUIChipsManager.Instance.Reset(selectable.GetComponent<ChipsManager>()._startPosition);
            }
        }
        else
        {
            selectable.transform.DOMove(selectable.GetComponent<ChipsManager>()._startPosition, 0.5f)
                .SetEase(Ease.OutElastic);
        }
    }
    
    private IEnumerator ChipsIncrement(ChipsManager otherChips)
    {
        _chipsIncrement = true;
        otherChips.isBusy = true;
        isBusy = true;
        
        gameObject.tag = "Untagged";
        otherChips.gameObject.tag = "Untagged";
        
        var otherLastChipNumber = otherChips._chips.Last().chipNumber;
        var ownLastChipNumber = _chips.Last().chipNumber;

        for (int i = _chips.Count-1; i >= 0; i--)
        {
            if (_chips[i].chipNumber.Equals(otherLastChipNumber))
            {
                _chips[i].chip.transform.DOMove(new Vector3(otherChips._chips.Last().chip.transform.position.x, otherChips._chips.Last().chip.transform.position.y + 0.05f, otherChips._chips.Last().chip.transform.position.z), 0.15f);
                 _chips[i].chip.transform.DORotate(new Vector3(0, 0, 180), 0.15f, RotateMode.Fast)
                    .SetEase(Ease.OutElastic).OnComplete(() =>
                    {
                        GameObject lastChip = _chips[i].chip;
                        Destroy(lastChip);
                        _chips.RemoveAt(i); 
                        otherChips.StartCoroutine(otherChips.PoolChips(1, lastChip));
                    });
                 yield return new WaitForSeconds(0.15f);
            }
            gameObject.tag = "ChipsStack";
            otherChips.gameObject.tag = "ChipsStack";
            
            otherChips.isBusy = false;
            isBusy = false;
            _chipsIncrement = false;
            yield break;
        }
    }
    public IEnumerator PoolChips(int count, GameObject prefab)
    {
        while (count > 0)
        {
            if (_chips.Count == 0)
            {
                var newPostion = new Vector3(GetComponentInChildren<Transform>().position.x,
                    GetComponentInChildren<Transform>().position.y + 0.05f,
                    GetComponentInChildren<Transform>().position.z);
                GameObject chip = Instantiate(_chipPrefab, newPostion, Quaternion.identity) as GameObject;
                chip.transform.parent = gameObject.transform;
                
                Chip newChip = new Chip();
                newChip.chip = chip;
                newChip.chipNumber = chip.GetComponent<ChipManager>().chipNumber;
                _chips.Add(newChip);
                
            }
            else
            {
                var newPostion = new Vector3(_chips.Last().chip.transform.position.x,
                    _chips.Last().chip.transform.position.y + 0.05f,
                    _chips.Last().chip.transform.position.z);

                GameObject chip = Instantiate(prefab, newPostion, Quaternion.identity) as GameObject; 
                chip.transform.parent = gameObject.transform;

                Canvas chipCanvas = chip.GetComponentInChildren<Canvas>();
                chipCanvas.enabled = true;

                Chip newChip = new Chip();
                newChip.chip = chip;
                newChip.chipNumber = chip.GetComponent<ChipManager>().chipNumber;
                _chips.Add(newChip);
            }

            count--;
            yield return new WaitForFixedUpdate();
        }
    }
    private bool CheckForElements(List<Chip> array)
    {
        if (array.Count == 0)
        {
            Debug.Log("Error by array");
            return false;
        }
        for (int i = 1; i < array.Count; i++)
        {
            if (!array[i].chipNumber.Equals(array[0].chipNumber))
            {
                Debug.Log("Not all elements equal");
                return false;
            }
        }
        return true;
    }
    private bool AreAllElementsEqual(List<Chip> array)
    {
        if (array.Count < 15)
        {
            Debug.Log("Error by array");
            return false;
        }
        for (int i = 1; i < array.Count; i++)
        {
            if (!array[i].chipNumber.Equals(array[0].chipNumber))
            {
                Debug.Log("Not all elements equal");
                return false;
            }
        }
        return true;
    }
    void OnDrawGizmos()
    {
        // Рисование луча в Scene View
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.forward * rayCastDistance);
        Gizmos.DrawRay(transform.position, Vector3.back * rayCastDistance);
        Gizmos.DrawRay(transform.position, Vector3.left * rayCastDistance);
        Gizmos.DrawRay(transform.position, Vector3.right * rayCastDistance);
        Gizmos.DrawRay(transform.position, Vector3.down * rayCastDistance);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ChipsManager))]
public class ChipsManagerEditor : Editor
{ 
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ChipsManager chipsManager = (ChipsManager)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Chipss"))
        {
            chipsManager.StartCoroutine(chipsManager.PoolChips(chipsManager.generateChipTest, chipsManager._chipPrefab));
        }
    }
}
#endif