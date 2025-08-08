using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using TMPro; // Dependencia para usar TextMeshPro

public class PreguntasFrecuentes : MonoBehaviour
{
    public TextAsset csv; // Archivo con las preguntas y respuestas
    public GameObject botonPrefab; // Prefab del bot�n (debe tener un TMP_Text como hijo)
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
        //CargarDatos();
        StartCoroutine(LlamadoBase());
        //InstanciarBotones();

    }
    //<color=#50DCEF>   "texto"     </color>
    /// <summary>
    /// Elimina caracteres invisibles problemáticos del texto.
    /// </summary>
    string LimpiarTexto(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return texto;

        // Expresión regular para remover:
        // - Espacios duros (U+00A0)
        // - Zero-width spaces, non-joiners, etc. (U+200B - U+200F)
        // - Caracteres de control invisibles (0x00 - 0x1F, 0x7F)
        // - Otros invisibles como LRM/RLM y direccionales (U+202A - U+202E)
        string limpio = Regex.Replace(texto, @"[\u0000-\u001F\u007F\u00A0\u00AD\u200B-\u200F\u202A-\u202E]", "\n");

        return limpio;
    }

    IEnumerator LlamadoBase()
    {

        UnityWebRequest www = UnityWebRequest.Get("https://amvi.pascualbravo.edu.co/pfcis/api.php");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
     
            Debug.Log(www.downloadHandler.text);

            string[] lineas = LimpiarTexto(www.downloadHandler.text).Split("%n%");

            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;

                string[] columnas = linea.Split('|');

                if (columnas.Length >= 2)
                {
                    listaQA.Add(new PreguntaRespuesta
                    {
                        pregunta = (columnas[0] + ". " + columnas[1]).Trim(),
                        respuesta = columnas[2].Trim().Replace("%p%", ";")
                    });
                }
                if (string.IsNullOrWhiteSpace(linea)) continue;
            }
            InstanciarBotones();
        }
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

            // Buscamos el TMP_Text dentro del bot�n instanciado (como hijo)
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