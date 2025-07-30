using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Dependencia para usar TextMeshPro

public class LinkExtractor : MonoBehaviour
{
    /// <summary>
    /// Extrae todos los enlaces (http, https, www) encontrados en el texto proporcionado.
    /// </summary>
    /// <param name="inputText">Texto del cual extraer los enlaces.</param>
    /// <returns>Lista de enlaces encontrados.</returns>
    /// 
    [Multiline(4)]
    public string       texto;
    public TMP_Text     txtVisual;
    public GameObject   botonEnlace;
    public Transform    padre;

    List<GameObject> botonesEnlaces = new List<GameObject>();
    public List<string> ExtractLinks(string inputText)
    {
        List<string> links = new List<string>();

        // Expresión regular para encontrar URLs
        string pattern = @"((http|https):\/\/[^\s]+)|(www\.[^\s]+)";
        MatchCollection matches = Regex.Matches(inputText, pattern, RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            links.Add(match.Value);
        }

        return links;
    }

    // Ejemplo de uso en Unity
    private void Start()
    {
        CrearEnlaces();
    }
    public void CrearEnlaces(string txt)
    {
        texto = txt;
        CrearEnlaces();
    }

    void CrearEnlaces()
    {
        List<string> foundLinks = ExtractLinks(texto);

        for (int i = 0; i < foundLinks.Count; i++)
        {   
            texto = texto.Replace(foundLinks[i], "<color=#50DCEF>"+ foundLinks[i]+ "</color>"); 
        }

        txtVisual.text = texto;

        for (int i = 0; i < botonesEnlaces.Count; i++)
        {
            Destroy(botonesEnlaces[i]);
        }
        botonesEnlaces.Clear();
        Adicional.indice = 1;

        foreach (string link in foundLinks)
        {
            Debug.Log("Enlace encontrado: " + link);
            Adicional a = Instantiate(botonEnlace, padre).GetComponent<Adicional>();
            a.Inicializar(link);
            botonesEnlaces.Add(a.gameObject);
        }


    }
}
