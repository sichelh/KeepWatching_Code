using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityFilter : MonoBehaviour, IVisibility
{
    [SerializeField] private Color reactionColor;
    private Renderer objectRenderer;

    public Color ReactionColor => reactionColor;

    private bool isVisible = true;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        SetVisible(false);
        PlayerController.Instance.FlashlightController.AddVisibilityFilterObject(this);
    }

    public void SetVisible(bool visible)
    {
        //같은 상태일땐 판단할 필요 없음
        if (isVisible == visible)
            return;

        isVisible = visible;


        if (objectRenderer != null)
            objectRenderer.enabled = visible;
    }

    public bool IsLightColorMatching(Color lightColor)
    {
        return lightColor == reactionColor;
    }

    public bool IsVisible()
    {
        return objectRenderer.enabled;
    }
}