using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Dependencia para usar TextMeshPro


public class Adicional : MonoBehaviour
{
    public string url;
    public TMP_Text txtURL;
    public static int indice = 1;

    public void Inicializar(string _url)
    {
        url         = _url;
        txtURL.text = indice + ". " + ((_url.Length > 10)? url.Replace("https://", "").Replace("http://", "").Substring(0,10): url) + "(...)";
        indice++;
    }
    public void AbrirEnlace()
    {
        Application.OpenURL(url);
    }
}
