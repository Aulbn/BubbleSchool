using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColorRandomizer : MonoBehaviour
{
    public Renderer Renderer;
    public Color[] Colors;
    void Start()
    {
        Renderer.material.SetColor("_BaseColor", Colors[Random.Range(0, Colors.Length - 1)]);
    }
    
}
