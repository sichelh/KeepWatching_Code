using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadSceneObject : CutEventAble
{
    public override void PlayEvent()
    {
    }

    public override void SubEvent()
    {
        GameManager.Instance.DeadScene.OnCameraEffect();
        AudioManager.Instance.PlaySFX(SFX.FearBoom);
    }
}