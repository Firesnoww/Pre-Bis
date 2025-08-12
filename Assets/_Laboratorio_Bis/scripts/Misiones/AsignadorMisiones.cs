using UnityEngine;
using UnityEngine.UI;
using TMPro;
// >>> NUEVO
using UnityEngine.Events;

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
    public TMP_Text txtConfirmacion;       // opcional: “Si aceptas… perderás el progreso”

    private MisionesAnidada lineaSeleccionada;
    private MisionesAnidada lineaPendiente; // para guardar la que se quiere aceptar

    void Start()
    {
        if (btnAceptar) btnAceptar.interactable = false;
        if (panelConfirmacion) panelConfirmacion.SetActive(false);

        Debug.Log("[Asignador] Start");

        if (lineasNPC == null)
        {
            Debug.LogError("[Asignador] lineasNPC == null");
            return;
        }

        Debug.Log($"[Asignador] lineasNPC.Length = {lineasNPC.Length}");

        if (botonPrefab == null)
        {
            Debug.LogError("[Asignador] botonPrefab no asignado.");
            return;
        }
        if (contenedorBotones == null)
        {
            Debug.LogError("[Asignador] contenedorBotones no asignado.");
            return;
        }

        // (Opcional) desactivar aceptar al inicio
        if (btnAceptar) btnAceptar.interactable = false;

        int creados = 0;
        for (int i = 0; i < lineasNPC.Length; i++)
        {
            var linea = lineasNPC[i];
            if (linea == null)
            {
                Debug.LogWarning($"[Asignador] linea {i} es null.");
                continue;
            }
            if (linea.misiones == null)
            {
                Debug.LogWarning($"[Asignador] linea {i} tiene misiones == null.");
                continue;
            }
            if (linea.misiones.Length == 0)
            {
                Debug.LogWarning($"[Asignador] linea {i} tiene 0 misiones.");
                continue;
            }
            if (linea.misiones[0] == null)
            {
                Debug.LogWarning($"[Asignador] linea {i} misiones[0] == null.");
                continue;
            }

            var intro = linea.misiones[0];
            Debug.Log($"[Asignador] Creando botón para línea {i}: '{intro.NombreMision}'");

            var go = Instantiate(botonPrefab, contenedorBotones);
            var label = go.GetComponentInChildren<TMPro.TMP_Text>();
            if (label == null)
                Debug.LogWarning("[Asignador] El prefab no tiene TMP_Text hijo. Se creará el botón sin texto.");
            else
                label.text = intro.NombreMision;

            var btn = go.GetComponent<UnityEngine.UI.Button>();
            if (btn == null)
                Debug.LogError("[Asignador] El prefab no tiene Button.");
            else
            {
                MisionesAnidada lineaLocal = linea;
                btn.onClick.AddListener(() => SeleccionarLinea(lineaLocal));
                creados++;
            }
        }

        Debug.Log($"[Asignador] Botones creados: {creados}");

        if (lineasNPC.Length > 0 && lineasNPC[0] != null && lineasNPC[0].misiones != null && lineasNPC[0].misiones.Length > 0 && lineasNPC[0].misiones[0] != null)
        {
            MostrarPreview(lineasNPC[0]);
        }
        else
        {
            Debug.LogWarning("[Asignador] No se pudo mostrar preview inicial (primera línea incompleta).");
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
            // >>> NUEVO: distinguir si es la misma línea
            bool esMisma = mainMisiones.EsMismaLinea(lineaSeleccionada.misiones);

            if (esMisma)
            {
                // Mismo set de misiones activo: Seguir o Cancelar
                MostrarPanelConfirmacionCustom(
                    "Esta misma misión ya está activa. ¿Deseas CANCELARLA o SEGUIR?",
                    onSi: () => {
                        // Cancelar (abortamos, queda retomable)
                        mainMisiones.AbortarLineaActual();
                        // Deja el botón Aceptar habilitado para retomarla si quiere
                        if (btnAceptar) btnAceptar.interactable = true;
                        lineaPendiente = null;
                        OcultarPanelConfirmacion();
                    },
                    onNo: () => {
                        // Seguir (no hacemos nada)
                        lineaPendiente = null;
                        OcultarPanelConfirmacion();
                    }
                );
                return;
            }

            // Distinta a la activa: mantener tu confirmación actual
            lineaPendiente = lineaSeleccionada;
            MostrarPanelConfirmacion(true);
            return;
        }

        // Si no hay línea activa, aceptar directo
        EjecutarAceptar(lineaSeleccionada);
    }

    void EjecutarAceptar(MisionesAnidada linea)
    {
        mainMisiones.CargarLinea(linea.misiones);
        mainMisiones.ComenzarLinea();
        if (btnAceptar) btnAceptar.interactable = false;
        MostrarPanelConfirmacion(false);
        Debug.Log("[Asignador] Línea aceptada y enviada a MainMisiones");
    }

    // ----- Confirmación existente (para CAMBIO de línea) -----
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

    // >>> NUEVO: confirmación personalizada (para "Seguir / Cancelar" en la misma línea)
    void MostrarPanelConfirmacionCustom(string mensaje, UnityAction onSi, UnityAction onNo)
    {
        if (panelConfirmacion == null)
        {
            Debug.LogWarning("No hay panel de confirmación asignado.");
            return;
        }
        panelConfirmacion.SetActive(true);

        if (txtConfirmacion) txtConfirmacion.text = mensaje;

        if (btnConfirmarSi) btnConfirmarSi.onClick.RemoveAllListeners();
        if (btnConfirmarNo) btnConfirmarNo.onClick.RemoveAllListeners();

        if (btnConfirmarSi) btnConfirmarSi.onClick.AddListener(() => onSi?.Invoke());
        if (btnConfirmarNo) btnConfirmarNo.onClick.AddListener(() => onNo?.Invoke());
    }

    // >>> NUEVO: helper para cerrar panel
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
