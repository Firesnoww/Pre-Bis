using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RutaVisual : MonoBehaviour
{
    public Transform[] puntosRuta; // Puntos de la ruta en orden
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DibujarRuta(); // <- se actualiza en tiempo real
    }

    void DibujarRuta()
    {
        if (puntosRuta.Length < 2) return;

        lineRenderer.positionCount = puntosRuta.Length;

        for (int i = 0; i < puntosRuta.Length; i++)
        {
            if (puntosRuta[i] != null)
                lineRenderer.SetPosition(i, puntosRuta[i].position);
        }
    }
}

