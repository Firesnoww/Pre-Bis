using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibelulaCapturable : MonoBehaviour
{
    public LibelulaData datos;
    private bool enPuntoDeCaptura = false;
    private bool enRed = false;
    private RutaVisual rutaVisual; // Referencia a su ruta

    public void SetRuta(RutaVisual ruta)
    {
        rutaVisual = ruta;
    }

    public void SetEnPuntoDeCaptura(bool estado)
    {
        enPuntoDeCaptura = estado;
    }

    public void Update()
    {
        Debug.Log("cambia? = " + enPuntoDeCaptura);
       
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Capturable " + enPuntoDeCaptura);
        if (enPuntoDeCaptura && other.CompareTag("RedCaptura"))
        {
            Capturar();
        }
    }   


    private void Capturar()
    {
        Debug.Log("Capturaste a: " + datos.nombre);
        Destroy(gameObject);
        // Desactivar visualmente la ruta

        // Aquí podrías enviar datos a un sistema de almacenamiento más grande si lo necesitas
    }
}