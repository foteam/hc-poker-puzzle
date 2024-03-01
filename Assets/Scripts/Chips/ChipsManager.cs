using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Lean.Touch;
using UnityEditor;

public class ChipsManager : MonoBehaviour
{
    [Serializable]
    public class Chip
    {
        public GameObject chip;
        public int chipNumber;
    }
    
    [SerializeField] private GameObject _chipPrefab;
    [SerializeField] private List<Chip> _chips;
    
    [Range(0, 10)]
    [SerializeField] private float rayCastDistance;

    public int generateChipTest;
    public int chipPackID;

    private void Start()
    {
        StartCoroutine(PoolChips(generateChipTest, _chipPrefab));
        ChipsCalculate();
        GameManager.spawnedChipsID += 1;
        chipPackID = GameManager.spawnedChipsID;
    }

    private void LateUpdate()
    {
        if (_chips.Count == 0) Destroy(this.gameObject);
        RayDetector(Vector3.forward);
        RayDetector(Vector3.back);
        RayDetector(Vector3.left);
        RayDetector(Vector3.right);
    }
    private void RayDetector(Vector3 direction)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out hit, rayCastDistance))
        {
            var tag = hit.collider.gameObject.tag;
            if (tag == "Chips")
            {
                ChipsManager otherChips = hit.collider.gameObject.GetComponent<ChipsManager>();
                if (_chips.Count == 0) return;
                StartCoroutine(ChipsIncrement(otherChips));
            }
        }
    }

    private async Task ChipsCalculate()
    {
        while (true)
        {
            await Task.Delay(1500);
            if (_chips.Count > 15)
            {
                List<Chip> lastChips = new List<Chip>();
                lastChips = _chips.GetRange(0, 15);
                
            }
        }

    }
    private IEnumerator ChipsIncrement(ChipsManager otherChips)
    {
        if (otherChips.chipPackID > chipPackID & otherChips._chips.Last().chipNumber == _chips.Last().chipNumber)
        {
            _chips.Last().chip.transform.DOMove(new Vector3(otherChips._chips.Last().chip.transform.position.x, otherChips._chips.Last().chip.transform.position.y + 0.25f, otherChips._chips.Last().chip.transform.position.z), 0.25f);
            _chips.Last().chip.transform.DORotate(new Vector3(0, 0, 180), 0.25f, RotateMode.Fast).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                Destroy(_chips.Last().chip);
                _chips.Remove(_chips.Last());
                otherChips.StartCoroutine(otherChips.PoolChips(1, _chips.Last().chip));
            });
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
                GameObject chip = Instantiate(_chipPrefab, newPostion, Quaternion.identity);
                chip.transform.parent = gameObject.transform;
                chip.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 2);

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
                GameObject chip = Instantiate(prefab, newPostion, Quaternion.identity);
                chip.transform.parent = gameObject.transform;
                chip.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 2);
                
                Chip newChip = new Chip();
                newChip.chip = chip;
                newChip.chipNumber = chip.GetComponent<ChipManager>().chipNumber;
                _chips.Add(newChip);
            }

            count--;
            yield return new WaitForFixedUpdate();
        }
    }
    
    void OnDrawGizmos()
    {
        // Рисование луча в Scene View
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.forward * rayCastDistance);
        Gizmos.DrawRay(transform.position, Vector3.back * rayCastDistance);
        Gizmos.DrawRay(transform.position, Vector3.left * rayCastDistance);
        Gizmos.DrawRay(transform.position, Vector3.right * rayCastDistance);
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
            chipsManager.StartCoroutine(chipsManager.PoolChips(chipsManager.generateChipTest, chipsManager.gameObject));
        }
    }
}
#endif