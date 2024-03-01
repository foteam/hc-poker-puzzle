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

    private void LateUpdate()
    {
        if (!isEmpty) gameObject.tag = "Untagged";
        if (isEmpty) gameObject.tag = "Grid";
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Chips"))
        {
            if (other.gameObject.GetComponent<ChipsManager>().isCollectable)
            {
                isEmpty = false;
                obj = other.gameObject;
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Chips"))
        {
            isEmpty = true;
            obj = null;
        }
    }
}
