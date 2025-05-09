using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscilarSuavemente : MonoBehaviour
{
    [Header("Rangos aleatorios")]
    public Vector2 rangoAmplitud = new Vector2(0.15f, 0.4f);
    public Vector2 rangoVelocidad = new Vector2(0.5f, 2f);

    private float amplitud;
    private float velocidad;
    private Vector3 posicionBase;
    private Vector3 ultimaPosicion;
    private float offsetFase;

    void Start()
    {
        amplitud = Random.Range(rangoAmplitud.x, rangoAmplitud.y);
        velocidad = Random.Range(rangoVelocidad.x, rangoVelocidad.y);
        offsetFase = Random.Range(0f, 2f * Mathf.PI);

        posicionBase = transform.position;
        ultimaPosicion = posicionBase;
    }

    void Update()
    {
        // Actualiza posición base si el objeto ha sido movido
        if (transform.position != ultimaPosicion)
        {
            posicionBase = transform.position;
            ultimaPosicion = transform.position;
        }

        float desplazamientoY = Mathf.Sin(Time.time * velocidad + offsetFase) * amplitud;
        transform.position = posicionBase + Vector3.up * desplazamientoY;
    }
}
