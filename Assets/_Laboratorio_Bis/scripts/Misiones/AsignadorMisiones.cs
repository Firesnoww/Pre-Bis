using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

//
// AsignadorMisionesMulti
// Genera botones para cada "MisionesAnidada" (línea).
// Muestra preview y permite ACEPTAR la línea seleccionada.
// - Si NO hay línea activa: acepta directo.
// - Si hay una activa y seleccionas OTRA: confirmación "perderás el progreso".
// - Si hay una activa y seleccionas la MISMA: confirmación "¿Cancelar o Seguir?".
//
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
    public Button btnAceptar;

    [Header("Confirmación")]
    public GameObject panelConfirmacion;   // Panel con texto y 2 botones
    public Button btnConfirmarSi;
    public Button btnConfirmarNo;
    public TMP_Text txtConfirmacion;       // Texto informativo del diálogo

    private MisionesAnidada lineaSeleccionada;
    private MisionesAnidada lineaPendiente; // para guardar la que se quiere aceptar (otra distinta a la activa)

    void Start()
    {
        if (btnAceptar) btnAceptar.interactable = false;
        if (panelConfirmacion) panelConfirmacion.SetActive(false);

        if (lineasNPC == null || botonPrefab == null || contenedorBotones == null)
        {
            Debug.LogError("[Asignador] Faltan referencias.");
            return;
        }

        int creados = 0;
        for (int i = 0; i < lineasNPC.Length; i++)
        {
            var linea = lineasNPC[i];
            if (linea == null || linea.misiones == null || linea.misiones.Length == 0 || linea.misiones[0] == null)
            {
                Debug.LogWarning($"[Asignador] línea {i} incompleta.");
                continue;
            }

            var intro = linea.misiones[0];
            var go = Instantiate(botonPrefab, contenedorBotones);
            var label = go.GetComponentInChildren<TMP_Text>();
            if (label) label.text = intro.NombreMision;

            var btn = go.GetComponent<Button>();
            if (btn != null)
            {
                MisionesAnidada lineaLocal = linea;
                btn.onClick.AddListener(() => SeleccionarLinea(lineaLocal));
                creados++;
            }
        }
        Debug.Log($"[Asignador] Botones creados: {creados}");

        // Preview inicial si existe
        if (lineasNPC.Length > 0 && lineasNPC[0] != null && lineasNPC[0].misiones != null && lineasNPC[0].misiones.Length > 0 && lineasNPC[0].misiones[0] != null)
        {
            MostrarPreview(lineasNPC[0]);
        }
    }

    void SeleccionarLinea(MisionesAnidada linea)
    {
        if (linea == null || linea.misiones == null || linea.misiones.Length == 0) return;
        lineaSeleccionada = linea;
        MostrarPreview(lineaSeleccionada);
        if (btnAceptar) btnAceptar.interactable = true;
    }

    // onClick del botón fijo “Aceptar misión”
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

        if (mainMisiones.HayLineaActiva)
        {
            // ¿Es la misma línea activa?
            bool esMisma = mainMisiones.EsMismaLinea(lineaSeleccionada.misiones);

            if (esMisma)
            {
                // Mismo set de misiones activo: Seguir o Cancelar
                MostrarPanelConfirmacionCustom(
                    "Esta misma misión ya está activa. ¿Deseas CANCELARLA o SEGUIR?",
                    onSi: () => {
                        // Cancelar (Abortar) — queda retomable
                        mainMisiones.AbortarLineaActual();
                        if (btnAceptar) btnAceptar.interactable = true; // permitir retomarla
                        lineaPendiente = null;
                        OcultarPanelConfirmacion();
                    },
                    onNo: () => {
                        // Seguir — no hace nada
                        lineaPendiente = null;
                        OcultarPanelConfirmacion();
                    }
                );
                return;
            }

            // Distinta a la activa: confirmación original
            lineaPendiente = lineaSeleccionada;
            MostrarPanelConfirmacion(true);
            return;
        }

        // No hay línea activa: aceptar directo (MainMisiones hará el auto‑salto del Inicio si corresponde)
        EjecutarAceptar(lineaSeleccionada);
    }

    void EjecutarAceptar(MisionesAnidada linea)
    {
        mainMisiones.CargarLinea(linea.misiones);
        mainMisiones.ComenzarLinea(); // aquí dentro, si la fase 0 es Inicio y AutoSaltarInicioAlAceptar = true, saltará a la primera tarea
        if (btnAceptar) btnAceptar.interactable = false;
        MostrarPanelConfirmacion(false);
        Debug.Log("[Asignador] Línea aceptada y enviada a MainMisiones");
    }

    // ---------------- Confirmación (cambiar de línea) ----------------
    void MostrarPanelConfirmacion(bool mostrar)
    {
        if (panelConfirmacion == null) { if (mostrar) Debug.LogWarning("No hay panel de confirmación asignado."); return; }
        panelConfirmacion.SetActive(mostrar);

        if (mostrar)
        {
            if (txtConfirmacion)
                txtConfirmacion.text = "Si aceptas esta misión, perderás el progreso de la misión actual. ¿Deseas continuar?";

            if (btnConfirmarSi) btnConfirmarSi.onClick.RemoveAllListeners();
            if (btnConfirmarNo) btnConfirmarNo.onClick.RemoveAllListeners();

            if (btnConfirmarSi)
                btnConfirmarSi.onClick.AddListener(() =>
                {
                    // Abortar actual y aceptar la nueva
                    mainMisiones.AbortarLineaActual();
                    if (lineaPendiente != null) EjecutarAceptar(lineaPendiente);
                });

            if (btnConfirmarNo)
                btnConfirmarNo.onClick.AddListener(() =>
                {
                    lineaPendiente = null;
                    MostrarPanelConfirmacion(false);
                });
        }
    }

    // ---------------- Confirmación personalizada (misma línea) ----------------
    void MostrarPanelConfirmacionCustom(string mensaje, UnityAction onSi, UnityAction onNo)
    {
        if (panelConfirmacion == null)
        {
            Debug.LogWarning("No hay panel de confirmación asignado.");
            return;
        }
        panelConfirmacion.SetActive(true);

        if (txtConfirmacion) txtConfirmacion.text = mensaje;

        if (btnConfirmarSi) { btnConfirmarSi.onClick.RemoveAllListeners(); btnConfirmarSi.onClick.AddListener(() => onSi?.Invoke()); }
        if (btnConfirmarNo) { btnConfirmarNo.onClick.RemoveAllListeners(); btnConfirmarNo.onClick.AddListener(() => onNo?.Invoke()); }
    }

    void OcultarPanelConfirmacion()
    {
        if (panelConfirmacion) panelConfirmacion.SetActive(false);
    }

    void MostrarPreview(MisionesAnidada linea)
    {
        var intro = linea.misiones[0];
        if (txtTituloPreview) txtTituloPreview.text = intro.NombreMision;
        if (txtDescripcionPreview) txtDescripcionPreview.text = intro.InfoMision;
        if (txtDificultadPreview) txtDificultadPreview.text = $"Dificultad: {intro.Dificultad}";
    }
}
