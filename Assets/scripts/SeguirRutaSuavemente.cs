using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeguirRutaSuavemente : MonoBehaviour
{
    public LineRenderer ruta;
    public float velocidad = 5f;
    public bool loop = true;

    [Range(0f, 5f)]
    public float tension = 0.5f; // 0 = recto, 0.5 = estándar, 1 = muy curvo

    private List<Vector3> puntosRuta = new List<Vector3>();
    private float distanciaTotal;
    private float progreso = 0f;

    void Start()
    {
        puntosRuta.Clear();
        for (int i = 0; i < ruta.positionCount; i++)
        {
            puntosRuta.Add(ruta.GetPosition(i));
        }

        if (loop)
        {
            // Agrega los primeros puntos al final para suavizar el cierre
            puntosRuta.Add(puntosRuta[0]);
            puntosRuta.Add(puntosRuta[1]);
            puntosRuta.Add(puntosRuta[2]);
        }

        distanciaTotal = puntosRuta.Count;
    }

    void Update()
    {
        puntosRuta.Clear();
        for (int ei = 0; ei < ruta.positionCount; ei++)
        {
            puntosRuta.Add(ruta.GetPosition(ei));
        }

        // Si hay loop, reañadir los extras
        if (loop && ruta.positionCount >= 3)
        {
            puntosRuta.Add(puntosRuta[0]);
            puntosRuta.Add(puntosRuta[1]);
            puntosRuta.Add(puntosRuta[2]);
        }

        if (puntosRuta.Count < 4) return;

        // Avanza por la curva con velocidad constante
        progreso += Time.deltaTime * velocidad;
        if (progreso >= puntosRuta.Count - 3)
        {
            if (loop)
                progreso %= (puntosRuta.Count - 3);
            else
                progreso = puntosRuta.Count - 3 - 0.001f;
        }

        // Obtener posición en la curva
        int i = Mathf.FloorToInt(progreso);
        float t = progreso - i;

        Vector3 pos = CatmullRom(puntosRuta[i], puntosRuta[i + 1], puntosRuta[i + 2], puntosRuta[i + 3], t);
        transform.position = pos;

        // Rotación suave hacia el siguiente punto
        Vector3 nextPos = CatmullRom(puntosRuta[i], puntosRuta[i + 1], puntosRuta[i + 2], puntosRuta[i + 3], t + 0.01f);
        Vector3 dir = (nextPos - pos).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        
    }

    // Catmull-Rom interpolation
    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // Usa una fórmula con tensión variable: 0.5 = estándar Catmull-Rom
        float t0 = 0f;
        float t1 = GetT(t0, p0, p1);
        float t2 = GetT(t1, p1, p2);
        float t3 = GetT(t2, p2, p3);

        t = Mathf.Lerp(t1, t2, t); // Interpola entre t1 y t2

        Vector3 A1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
        Vector3 A2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
        Vector3 A3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;

        Vector3 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
        Vector3 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;

        Vector3 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;

        return C;
    }

    float GetT(float t, Vector3 p0, Vector3 p1)
    {
        // alpha controla la curvatura: 0 = uniforme (recto), 0.5 = Catmull-Rom, 1 = centrípeta (más curvo)
        float a = tension;
        return t + Mathf.Pow(Vector3.Distance(p0, p1), a);
    }
}