using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaCrasher : MonoBehaviour, ICrashable
{
    // 원하는 투명도
    public float targetAlpha = 0.0f;

    private Renderer render;
    private Material[] materials;

    private void Awake()
    {
        render = GetComponent<Renderer>();
        materials = render.materials;

        foreach (var material in materials)
        {
            // 알파 블렌딩 모드 설정(뭔가 Standard라서 안 됨)
            material.SetFloat("_Mode", 3); // 3 = Transparent
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }


    }

    public void Crash()
    {
        foreach (var material in materials)
        {
            if(material.HasProperty("_Color"))
            {
                Color color = material.color;
                color.a = targetAlpha;
                material.color = color;
            }
        }
        // 렌더러 끔
        render.enabled = false; 
    }
}
