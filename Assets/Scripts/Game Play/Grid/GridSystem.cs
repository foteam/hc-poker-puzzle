using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class GridSystem : MonoBehaviour
{
    public GameObject obj;
    public bool isEmpty = true;

    private Vector3 _actualPosition;

    private void Start()
    {
        _actualPosition = transform.position;
        transform.position = new Vector3(_actualPosition.x, -5, _actualPosition.z);
        transform.DOMove(_actualPosition, Random.Range(0.1f, 1f)).SetEase(Ease.OutCubic);
    }

    private void Update()
    {
        if (obj != null)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            gameObject.tag = "Untagged"; 
            isEmpty = false;
            return;
        }

        if (obj == null)
        {
            gameObject.layer = LayerMask.NameToLayer("Grid");
            gameObject.tag = "Grid"; 
            isEmpty = true;
            return;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Chips"))
        {
            if (other.gameObject.GetComponent<ChipsManager>().isCollectable)
            {
                obj = other.gameObject;
                obj.tag = "ChipsStack";
            }
        }
    }
}
