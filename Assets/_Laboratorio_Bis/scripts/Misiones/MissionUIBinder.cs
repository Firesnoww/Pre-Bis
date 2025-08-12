// MissionUIBinder.cs  (NUEVO)
using UnityEngine;
using TMPro;

public class MissionUIBinder : MonoBehaviour
{
    [Header("Refs")]
    public MainMisiones mainMisiones;

    [Header("UI")]
    public TMP_Text txtTitulo;
    public TMP_Text txtDescripcion;
    public TMP_Text txtProgreso;   // "Fase X/Y · Tipo"
    public TMP_Text txtUbicacion;  // "Lugar: ..."

    void OnEnable()
    {
        if (mainMisiones != null)
            mainMisiones.OnMissionUI += OnMissionUIChanged;
    }

    void OnDisable()
    {
        if (mainMisiones != null)
            mainMisiones.OnMissionUI -= OnMissionUIChanged;
    }

    // Callback de evento
    void OnMissionUIChanged(MainMisiones.MissionSnapshot s)
    {
        if (s == null) return;

        if (txtTitulo) txtTitulo.text = s.activa ? s.nombre : "Sin misión activa";
        if (txtDescripcion) txtDescripcion.text = s.activa ? (s.descripcion ?? "") : "";

        if (txtProgreso)
        {
            if (!s.activa || s.fasesTotal <= 0)
                txtProgreso.text = "";
            else
                txtProgreso.text = $"Fase {s.faseIndex}/{s.fasesTotal} · {s.tipo}";
        }

        if (txtUbicacion)
        {
            if (!s.activa || string.IsNullOrEmpty(s.ubicacion) || s.ubicacion == "-")
                txtUbicacion.text = "";
            else
                txtUbicacion.text = $"Lugar: {s.ubicacion}";
        }
    }
}
