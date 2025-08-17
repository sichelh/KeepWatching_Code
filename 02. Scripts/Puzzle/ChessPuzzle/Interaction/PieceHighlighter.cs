using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceHighlighter : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private float emissionMin = 0.5f;
    [SerializeField] private float emissionMax = 2f;

    private Material instanceMat;
    private Color baseEmissionColor;
    private bool pulsing = false;

    private void Start()
    {
        if (highlightMaterial != null)
        {
            instanceMat = new Material(highlightMaterial);
            baseEmissionColor = instanceMat.GetColor("_EmissionColor");
        }
    }

    private void Update()
    {
        if (pulsing && instanceMat != null)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            float emission = Mathf.Lerp(emissionMin, emissionMax, t);
            instanceMat.SetColor("_EmissionColor", baseEmissionColor * emission);
        }
    }

    public void StartHighlight(Renderer targetRenderer)
    {
        if (instanceMat == null) return;
        pulsing = true;
        targetRenderer.material = instanceMat;
    }

    public void StopHighlight(Renderer targetRenderer, Material original)
    {
        pulsing = false;
        targetRenderer.material = original;
    }

}
