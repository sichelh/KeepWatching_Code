using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LightControlledVisibility : MonoBehaviour
{
    /*
        스크립트를 FlishLightController와 합쳤습니다.
        왜냐하면 Update에서 매번 검사 하는것 보단 Flash가 On일때만 검사해주고
        Off일때 trackedFilters의 SetVisible(false)만 호출해주는 형식으로 바꿨습니다.
    */


    // [SerializeField] private Light lanternLight;
    // [SerializeField] private float lightDistance; // 감지 거리
    // [SerializeField] private float lightRadius;   // 감지 범위
    // private LayerMask layerMask;
    // private readonly List<VisibilityFilter> trackedFilters = new List<VisibilityFilter>();
    //
    // private void Start()
    // {
    //     layerMask = LayerMask.GetMask("Interactable");
    // }
    //
    // private void Update()
    // {
    //     foreach (var filter in trackedFilters)
    //     {
    //         filter.SetVisible(false);
    //     }
    //
    //     RaycastHit[] hits = Physics.SphereCastAll(lanternLight.transform.position, lightRadius, lanternLight.transform.forward, lightDistance, layerMask);
    //     foreach (var hit in hits)
    //     {
    //         if (hit.transform.TryGetComponent<IVisibility>(out var visibleObject))
    //         {
    //             if (visibleObject.IsLightColorMatching(lanternLight.color) && lanternLight.gameObject.activeSelf)
    //             {
    //                 visibleObject.SetVisible(true);
    //             }
    //         }
    //     }
    // }
    //
    // public void AddVisibilityFilterObject(VisibilityFilter filter)
    // {
    //     trackedFilters.Add(filter);
    // }
}