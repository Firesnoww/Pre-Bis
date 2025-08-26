using UnityEngine;
using TMPro;

//
// MissionUIBinder
// Script para el Canvas que escucha MainMisiones y pinta:
// - Título, Descripción
// - Progreso "Fase X/Y · Tipo"
// - Ubicación (si aplica)
//
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
        {
            mainMisiones.OnMissionUI += OnMissionUIChanged;
            mainMisiones.OnMissionSubprogress += OnSubprogress; // opcional
        }
    }

    void OnDisable()
    {
        if (mainMisiones != null)
        {
            mainMisiones.OnMissionUI -= OnMissionUIChanged;
            mainMisiones.OnMissionSubprogress -= OnSubprogress;
        }
    }

    void OnMissionUIChanged(MainMisiones.MissionSnapshot s)
    {
        if (s == null) return;

        if (txtTitulo) txtTitulo.text = s.activa ? s.nombre : "Sin misión activa";
        if (txtDescripcion) txtDescripcion.text = s.activa ? (s.descripcion ?? "") : "";

        if (txtProgreso)
        {
            if (!s.activa || s.fasesTotal <= 0) txtProgreso.text = "";
            else txtProgreso.text = $"Fase {s.faseIndex}/{s.fasesTotal} · {s.tipo}";
        }

        if (txtUbicacion)
        {
            if (!s.activa || string.IsNullOrEmpty(s.ubicacion) || s.ubicacion == "-")
                txtUbicacion.text = "";
            else
                txtUbicacion.text = $"Lugar: {s.ubicacion}";
        }
    }

    // Muestra "(quedan R/T)" junto al texto de progreso (si quieres)
    void OnSubprogress(int restante, int objetivo, string tipo)
    {
        if (txtProgreso == null) return;
        if (objetivo <= 0) return;
        // Añade detalle al final manteniendo lo anterior
        txtProgreso.text = $"{txtProgreso.text} (quedan {restante}/{objetivo})";
    }
}
