using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MisionActiva : MonoBehaviour
{
    public static UI_MisionActiva instancia;

    [Header("Panel de misión activa")]
    public GameObject panel;

    [Header("Campos de UI (TextMeshPro)")]
    public TMP_Text textoNombreMision;
    public TMP_Text textoFase;
    public TMP_Text textoDescripcionFase;
    public TMP_Text textoObjetivos;

    private void Awake()
    {
        instancia = this;

        if (panel != null)
            panel.SetActive(false);
    }

    // -------------------------------------------
    // Cuando el gestor inicia misión → se llama esto
    // -------------------------------------------
    public void MostrarMision(DatosDeMision mision, FaseBase fase)
    {
        if (panel != null)
            panel.SetActive(true);

        textoNombreMision.text = mision.nombreMision;
        textoFase.text = fase.nombreFase;
        textoDescripcionFase.text = fase.descripcionFase;

    }

    // -------------------------------------------
    // Cuando el gestor cambia fase → se llama esto
    // -------------------------------------------
    public void ActualizarFase(FaseBase fase)
    {
        textoFase.text = fase.nombreFase;
        textoDescripcionFase.text = fase.descripcionFase;
    }

    // -------------------------------------------
    // Cuando la misión termina → ocultar UI
    // -------------------------------------------
    public void OcultarMision()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
