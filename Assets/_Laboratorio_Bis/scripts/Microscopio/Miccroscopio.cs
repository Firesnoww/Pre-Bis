using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miccroscopio : MonoBehaviour
{
    public Transform[] lente;
    public Transform Zoom;
    public Transform Enfoque;
    public Transform p0;
    public Transform p1;

    [Range(0f, 1f)]
    public float t0;
    [Range(0f, 1f)]
    public float t1;

    public Material m;

    float desenfoque;

    // Velocidad de rotación
    public float velocidadRotacion = 180f; // grados por segundo

    void Update()
    {
        for (int i = 0; i < lente.Length; i++)
        {
            lente[i].position = Vector3.Lerp(p0.position, p1.position, t0);

            desenfoque = (Mathf.Abs(t1 - t0)) * 0.03f;
            m.SetFloat("_Desenfoque", desenfoque);
        }

        // Calcular ángulo objetivo (180° máximo)
        float anguloZoom = t0 * 180f;
        float anguloEnfoque = t1 * 180f;

        // Rotación suave sobre eje X
        Quaternion rotacionZoomObjetivo = Quaternion.Euler(anguloZoom, 0f, 0f);
        Quaternion rotacionEnfoqueObjetivo = Quaternion.Euler(anguloEnfoque, 0f, 0f);

        Zoom.localRotation = Quaternion.RotateTowards(Zoom.localRotation, rotacionZoomObjetivo, velocidadRotacion * Time.deltaTime);
        Enfoque.localRotation = Quaternion.RotateTowards(Enfoque.localRotation, rotacionEnfoqueObjetivo, velocidadRotacion * Time.deltaTime);
    }
}
