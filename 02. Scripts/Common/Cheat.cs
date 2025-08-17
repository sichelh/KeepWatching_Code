using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheat : MonoBehaviour
{
    [SerializeField] List<ItemSO> items = new List<ItemSO>();

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("아이템 추가");
            foreach (var item in items)
            {
                InventoryManager.Instance.AddItem(item);
                Debug.Log($"{item}");
            }
        }
    }
}