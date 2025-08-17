using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVisibility
{
    Color ReactionColor { get;}
    public void SetVisible(bool visible);

    public bool IsVisible();
    public bool IsLightColorMatching(Color lightColor);
}
