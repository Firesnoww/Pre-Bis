using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miccroscopio : MonoBehaviour
{
    public Transform lente;
    public Transform p0;
    public Transform p1;

    [Range(0f, 1f)] 
    public float t0;
    [Range(0f, 1f)]
    public float t1;

    public Material m;


    float desenfoque;


    void Update()
    {
        lente.position = Vector3.Lerp(p0.position, p1.position, t0);

        desenfoque = (Mathf.Abs(t1 - t0))*0.03f;

        m.SetFloat("_Desenfoque", desenfoque);
        
    }
}
