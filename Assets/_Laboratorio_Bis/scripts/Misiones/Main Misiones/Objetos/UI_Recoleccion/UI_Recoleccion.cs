using UnityEngine;
using TMPro;

public class UI_Recoleccion : MonoBehaviour
{
    public static UI_Recoleccion instancia;

    public GameObject panel;
    public TMP_Text texto;

    private void Awake()
    {
        instancia = this;
        panel.SetActive(false);
    }

    public void MostrarMensaje(string mensaje)
    {
        texto.text = mensaje;
        panel.SetActive(true);
    }

    public void OcultarMensaje()
    {
        panel.SetActive(false);
    }
}
