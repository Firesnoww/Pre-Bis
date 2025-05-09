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
    private float progreso = 0f;

    private RutaVisual rutaVisual;

    private bool ralentizado = false;
    private float duracionRalentizacion = 1f;
    private float tiempoRalentizado = 0f;

    private int ultimoPuntoCapturaIndex = -1;

    void Start()
    {
        rutaVisual = ruta.GetComponent<RutaVisual>();

        puntosRuta.Clear();
        for (int i = 0; i < ruta.positionCount; i++)
        {
            puntosRuta.Add(ruta.GetPosition(i));
        }

        if (loop && puntosRuta.Count >= 3)
        {
            puntosRuta.Add(puntosRuta[0]);
            puntosRuta.Add(puntosRuta[1]);
            puntosRuta.Add(puntosRuta[2]);
        }
    }

    void Update()
    {
        puntosRuta.Clear();
        for (int ei = 0; ei < ruta.positionCount; ei++)
        {
            puntosRuta.Add(ruta.GetPosition(ei));
        }

        if (loop && ruta.positionCount >= 3)
        {
            puntosRuta.Add(puntosRuta[0]);
            puntosRuta.Add(puntosRuta[1]);
            puntosRuta.Add(puntosRuta[2]);
        }

        if (puntosRuta.Count < 4) return;

        int b = Mathf.FloorToInt(progreso);
        float c = progreso - b;

        float mod1 = rutaVisual != null && b < rutaVisual.modificadoresVelocidad.Length ? rutaVisual.modificadoresVelocidad[b] : 1f;
        float mod2 = rutaVisual != null && b + 1 < rutaVisual.modificadoresVelocidad.Length ? rutaVisual.modificadoresVelocidad[b + 1] : 1f;
        float velocidadInterpolada = Mathf.Lerp(mod1, mod2, c);

        // Aplicar ralentización temporal si está activo
        float velocidadActual = velocidadInterpolada;
        if (ralentizado)
        {
            velocidadActual *= 0.2f; // Por ejemplo, reducir al 20% de la velocidad original
            tiempoRalentizado -= Time.deltaTime;
            if (tiempoRalentizado <= 0f)
            {
                ralentizado = false;
                tiempoRalentizado = 0f;
            }
        }

        progreso += Time.deltaTime * velocidad * velocidadActual;

        if (progreso >= puntosRuta.Count - 3)
        {
            if (loop)
                progreso %= (puntosRuta.Count - 3);
            else
                progreso = puntosRuta.Count - 3 - 0.001f;
        }

        int i = Mathf.FloorToInt(progreso);
        float t = progreso - i;

        Vector3 pos = CatmullRom(puntosRuta[i], puntosRuta[i + 1], puntosRuta[i + 2], puntosRuta[i + 3], t);
        transform.position = pos;

        Vector3 nextPos = CatmullRom(puntosRuta[i], puntosRuta[i + 1], puntosRuta[i + 2], puntosRuta[i + 3], t + 0.01f);
        Vector3 dir = (nextPos - pos).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Revisar si estamos sobre un punto de captura
        if (rutaVisual != null && rutaVisual.puntosCaptura.Contains(i) && i != ultimoPuntoCapturaIndex)
        {
            // Activar ralentización
            ralentizado = true;
            tiempoRalentizado = duracionRalentizacion;
            ultimoPuntoCapturaIndex = i;
        }
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t0 = 0f;
        float t1 = GetT(t0, p0, p1);
        float t2 = GetT(t1, p1, p2);
        float t3 = GetT(t2, p2, p3);

        t = Mathf.Lerp(t1, t2, t);

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
        float a = tension;
        return t + Mathf.Pow(Vector3.Distance(p0, p1), a);
    }
}