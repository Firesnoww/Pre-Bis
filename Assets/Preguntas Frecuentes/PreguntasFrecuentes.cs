using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Dependencia para usar TextMeshPro

public class PreguntasFrecuentes : MonoBehaviour
{
    public TextAsset csv; // Archivo con las preguntas y respuestas
    public GameObject botonPrefab; // Prefab del botón (debe tener un TMP_Text como hijo)
    public Transform contenedorBotones; // Panel donde se instancian los botones
    public TMP_Text textoRespuesta; // Campo de texto fijo para mostrar la respuesta
    public LinkExtractor linke;

    [System.Serializable]
    public class PreguntaRespuesta
    {
        public string pregunta;
        public string respuesta;
    }

    public List<PreguntaRespuesta> listaQA = new List<PreguntaRespuesta>();

    void Start()
    {
        CargarDatos();
        InstanciarBotones();

    }

    void CargarDatos()
    {
        if (csv == null)
        {
            Debug.LogError("Archivo CSV no asignado.");
            return;
        }

        string[] lineas = csv.text.Split("%n%");

        foreach (string linea in lineas)
        {
            if (string.IsNullOrWhiteSpace(linea)) continue;

            string[] columnas = linea.Split(';');

            if (columnas.Length >= 2)
            {
                listaQA.Add(new PreguntaRespuesta
                {
                    pregunta = (columnas[0] + ". " + columnas[1]).Trim(),
                    respuesta = columnas[2].Trim().Replace("%p%", ";")
                }) ;
            }
            if (string.IsNullOrWhiteSpace(linea)) continue;
        }

    }

    void InstanciarBotones()
    {
        foreach (var qa in listaQA)
        {
            GameObject nuevoBoton = Instantiate(botonPrefab, contenedorBotones);

            // Buscamos el TMP_Text dentro del botón instanciado (como hijo)
            TMP_Text textoBoton = nuevoBoton.GetComponentInChildren<TMP_Text>();

            if (textoBoton != null)
                textoBoton.text = qa.pregunta;

            Button boton = nuevoBoton.GetComponent<Button>();
            if (boton != null)
            {
                string respuesta = qa.respuesta; // Necesario para el closure
                boton.onClick.AddListener(() =>
                {
                    textoRespuesta.text = respuesta;
                    linke.CrearEnlaces(respuesta);
                });
                
            }
        }
    }
}