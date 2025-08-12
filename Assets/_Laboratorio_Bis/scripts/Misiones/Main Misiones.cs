using UnityEngine;
using System;


public class MainMisiones : MonoBehaviour
{
    [Serializable]
    public class MissionSnapshot
    {
        public bool activa;            // ¿hay una línea activa?
        public string nombre;          // Nombre de la misión/fase actual (si aplica)
        public string descripcion;     // Descripción/Info de la fase
        public int faseIndex;          // Índice de fase actual (1-based para UI)
        public int fasesTotal;         // Total de fases en la línea
        public string tipo;            // "Inicio", "Captura", "Recolección", "Exploración", "Microscopio", "Resultado"
        public string ubicacion;       // Si hay lugares (Exploración), un texto breve
    }
    public Misiones[] misiones;
    public int misionActual = 0;
    private bool lineaActiva = false;
    public event Action<MissionSnapshot> OnMissionUI;
    public bool HayLineaActiva => lineaActiva;

    // >>> NUEVO: exposición segura de la referencia actual
    public Misiones[] LineaActual => misiones;

    // >>> NUEVO: helper para comparar si es la misma línea por referencia
    public bool EsMismaLinea(Misiones[] otraLinea)
    {
        return otraLinea != null && misiones == otraLinea;
    }

    public void CargarLinea(Misiones[] linea)
    {
        misiones = linea;
        misionActual = 0;
        lineaActiva = false;
        Debug.Log("MainMisiones: línea cargada. Lista para comenzar.");
        EmitirUI(); // <-- NUEVO: para que el UI muestre "lista pero no iniciada"
    }

    public void ComenzarLinea()
    {
        if (misiones == null || misiones.Length == 0) { Debug.LogWarning("No hay misiones cargadas."); return; }
        lineaActiva = true;
        // Antes de ejecutar, emite snapshot de la fase 1 (índice 0)
        EmitirUI(); // <-- NUEVO
        EjecutarMisionActual();
        Debug.Log("MainMisiones: intro mostrada. Esperando completar fase 1...");
    }

    public void AbortarLineaActual()
    {
        if (!lineaActiva) return;
        Debug.Log("MainMisiones: abortando línea activa. Se perderá el progreso.");
        lineaActiva = false;
        misiones = null;
        misionActual = 0;
        EmitirUI(); // <-- NUEVO: UI pasa a "Sin misión activa"
    }

    public void CompletarFaseActual()
    {
        if (!lineaActiva) { Debug.LogWarning("No hay línea activa."); return; }
        EjecutarMisionActual();
        EmitirUI(); // <-- NUEVO: tras avanzar, refrescar UI
    }

    void EjecutarMisionActual()
    {
        if (misiones == null || misiones.Length == 0) return;
        if (misionActual < 0 || misionActual >= misiones.Length)
        {
            Debug.Log("Línea completada.");
            lineaActiva = false;
            EmitirUI(); // <-- NUEVO: refleja finalización
            return;
        }

        Misiones m = misiones[misionActual];

        if (m.InicioMision)
        {
            Debug.Log($"--- Misión: {m.NombreMision} ---");
            Debug.Log($"Dificultad: {m.Dificultad}");
            Debug.Log($"Descripción general:\n{m.InfoMision}");
            // Importante: emitir (ya se emite fuera) y luego avanzar índice
            misionActual++;
            return;
        }

        if (m.EsCaptura)
        {
            Debug.Log("[FASE CAPTURA ACTIVADA]");
            Debug.Log($"Info captura: {m.InfoCaptura}");
            Debug.Log($"Libélulas Low: {m.LibelulaLow.Length} | High: {m.LibelulaHigt.Length}");
            misionActual++;
            return;
        }

        if (m.EsRecoleccion)
        {
            Debug.Log("[FASE RECOLECCIÓN ACTIVADA]");
            Debug.Log($"Info recolección: {m.InfoRecoleccion}");
            Debug.Log($"Muestras: {m.Muestra.Length}");
            misionActual++;
            return;
        }

        if (m.EsExploracion)
        {
            Debug.Log("[FASE EXPLORACIÓN ACTIVADA]");
            Debug.Log($"Info exploración: {m.InfoExploracion}");
            Debug.Log($"Zonas a visitar: {m.Lugar.Length}");
            misionActual++;
            return;
        }

        if (m.EsMicroscopio)
        {
            Debug.Log("[FASE MICROSCOPIO ACTIVADA]");
            Debug.Log($"Info microscopio: {m.InfoMicroscopio}");
            misionActual++;
            return;
        }

        if (m.EsResultado)
        {
            Debug.Log("[FASE RESULTADO FINAL ACTIVADA]");
            Debug.Log($"Info final: {m.InfoFinal}");
            Debug.Log($"Datos desbloqueados: {m.InfoDesbloqueada}");
            Debug.Log(">> (BD) Registrar misión completada y desbloquear info.");
            lineaActiva = false;
            EmitirUI(); // <-- NUEVO: refleja finalización
            return;
        }

        Debug.Log("Estado ACTUAL MISION = " + misionActual);
    }

    // API mínima para objetos externos (luego la conectamos a objetivos reales)
    public void ReportarProgreso(string objetivoId, int delta = 1)
    {
        Debug.Log($"Reporte recibido: {objetivoId} (+{delta}). Completando fase actual.");
        CompletarFaseActual();
    }

    private void EmitirUI()
    {
        var snap = ConstruirSnapshot();
        OnMissionUI?.Invoke(snap);
    }

    private MissionSnapshot ConstruirSnapshot()
    {
        var s = new MissionSnapshot();
        s.activa = lineaActiva;

        if (!lineaActiva || misiones == null || misiones.Length == 0)
        {
            s.nombre = "Sin misión activa";
            s.descripcion = "";
            s.faseIndex = 0;
            s.fasesTotal = 0;
            s.tipo = "";
            s.ubicacion = "";
            return s;
        }

        // Proteger índice
        int idx = Mathf.Clamp(misionActual, 0, Mathf.Max(0, misiones.Length - 1));
        Misiones m = misiones[idx];

        s.faseIndex = idx + 1;        // 1-based para UI
        s.fasesTotal = misiones.Length;

        // Determinar tipo + campos de texto
        if (m.InicioMision)
        {
            s.tipo = "Inicio";
            s.nombre = m.NombreMision;
            s.descripcion = m.InfoMision;
        }
        else if (m.EsCaptura)
        {
            s.tipo = "Captura";
            s.nombre = m.NombreMision;
            s.descripcion = string.IsNullOrEmpty(m.InfoCaptura) ? m.InfoMision : m.InfoCaptura;
        }
        else if (m.EsRecoleccion)
        {
            s.tipo = "Recolección";
            s.nombre = m.NombreMision;
            s.descripcion = string.IsNullOrEmpty(m.InfoRecoleccion) ? m.InfoMision : m.InfoRecoleccion;
        }
        else if (m.EsExploracion)
        {
            s.tipo = "Exploración";
            s.nombre = m.NombreMision;
            s.descripcion = string.IsNullOrEmpty(m.InfoExploracion) ? m.InfoMision : m.InfoExploracion;

            // Ubicación amigable si definiste lugares
            if (m.Lugar != null && m.Lugar.Length > 0 && m.Lugar[0] != null)
                s.ubicacion = m.Lugar[0].name; // puedes mejorar esto luego (p.ej. el más cercano al jugador)
        }
        else if (m.EsMicroscopio)
        {
            s.tipo = "Microscopio";
            s.nombre = m.NombreMision;
            s.descripcion = string.IsNullOrEmpty(m.InfoMicroscopio) ? m.InfoMision : m.InfoMicroscopio;
        }
        else if (m.EsResultado)
        {
            s.tipo = "Resultado";
            s.nombre = m.NombreMision;
            s.descripcion = string.IsNullOrEmpty(m.InfoFinal) ? m.InfoMision : m.InfoFinal;
        }
        else
        {
            s.tipo = "Fase";
            s.nombre = m.NombreMision;
            s.descripcion = m.InfoMision;
        }

        // Fallback ubicación si no es exploración
        if (string.IsNullOrEmpty(s.ubicacion)) s.ubicacion = "-";

        return s;
    }
}
