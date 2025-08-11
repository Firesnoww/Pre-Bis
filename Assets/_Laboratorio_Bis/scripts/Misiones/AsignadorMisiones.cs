using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AsignadorMisionesMulti : MonoBehaviour
{
    public MisionesAnidada[] lineasNPC;
    public MainMisiones mainMisiones;

    public Transform contenedorBotones;
    public GameObject botonPrefab;
    public TMP_Text txtTituloPreview;
    public TMP_Text txtDescripcionPreview;
    public TMP_Text txtDificultadPreview;

    [Header("Botón fijo de Aceptar")]
    public Button btnAceptar; // desactívalo por defecto en el Inspector

    private MisionesAnidada lineaSeleccionada;

    void Start()
    {
        // ... (creación de botones como ya lo tienes)
        if (btnAceptar) btnAceptar.interactable = false;
    }

    void SeleccionarLinea(MisionesAnidada linea)
    {
        if (linea == null || linea.misiones == null || linea.misiones.Length == 0) return;

        lineaSeleccionada = linea;
        MostrarPreview(lineaSeleccionada);

        if (btnAceptar) btnAceptar.interactable = true; // ahora puedes aceptar
    }

    // Conecta este método al onClick del botón fijo "Aceptar misión"
    public void AceptarLineaSeleccionada()
    {
        if (lineaSeleccionada == null)
        {
            Debug.LogWarning("No hay línea seleccionada para aceptar.");
            return;
        }
        if (mainMisiones == null)
        {
            Debug.LogError("Falta referencia a MainMisiones.");
            return;
        }

        mainMisiones.CargarLinea(lineaSeleccionada.misiones);
        mainMisiones.ComenzarLinea(); // muestra intro y queda lista la fase 1

        // opcional: bloquear aceptar o cerrar panel
        if (btnAceptar) btnAceptar.interactable = false;
    }

    void MostrarPreview(MisionesAnidada linea)
    {
        var intro = linea.misiones[0];
        if (txtTituloPreview) txtTituloPreview.text = intro.NombreMision;
        if (txtDescripcionPreview) txtDescripcionPreview.text = intro.InfoMision;
        if (txtDificultadPreview) txtDificultadPreview.text = $"Dificultad: {intro.Dificultad}";
    }
}
