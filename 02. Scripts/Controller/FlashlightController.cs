using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] private GameObject flashlight;
    [SerializeField] private float lightDistance; // 감지 거리
    [SerializeField] private float lightRadius;   // 감지 범위
    private LayerMask layerMask;
    private readonly List<VisibilityFilter> trackedFilters = new List<VisibilityFilter>();

    private Light flashlightLight;
    public bool IsOn { get; private set; }

    private void Awake()
    {
        flashlightLight = flashlight.GetComponent<Light>();
    }

    private void Start()
    {
        layerMask = LayerMask.GetMask("Interactable");
        DisableTrackedFilters();
    }

    private void Update()
    {
        if (!IsOn)
            return;

        DisableTrackedFilters();
        RaycastHit[] hits = Physics.SphereCastAll(flashlight.transform.position, lightRadius, flashlight.transform.forward, lightDistance, layerMask);
        foreach (RaycastHit hit in hits)
        {
            if (!PlayerController.Instance.FlashlightController.IsOn || !hit.transform.TryGetComponent<IVisibility>(out var visibleObject))
                continue;

            if (visibleObject.IsLightColorMatching(flashlightLight.color))
            {
                visibleObject.SetVisible(true);
            }
        }
    }

    public void AddVisibilityFilterObject(VisibilityFilter filter)
    {
        trackedFilters.Add(filter);
    }

    public void ToggleFlashlight()
    {
        flashlight.SetActive(!IsOn);
        IsOn = !IsOn;

        if (IsOn == false)
        {
            DisableTrackedFilters();
        }
    }

    private void DisableTrackedFilters()
    {
        trackedFilters.ForEach(x => x.SetVisible(false));
    }
}