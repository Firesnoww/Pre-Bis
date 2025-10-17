using System;
using System.Collections.Generic; // para Dictionary en multi‑ítem
using UnityEngine;

//
// MainMisiones
// Núcleo que ejecuta líneas de misión (Misiones[]) de forma lineal.
// - CargarLinea(linea): establece una nueva línea (no activa).
// - ComenzarLinea(): activa la línea y posiciona en la fase 0.
// - CompletarFaseActual(): avanza a la siguiente fase.
// - AbortarLineaActual(): cancela la línea activa (limpia progreso).
//
// Eventos para UI:
//   - OnMissionUI(MissionSnapshot): snapshot de estado actual (para Canvas).
//   - OnMissionSubprogress(restante, objetivo, tipo): sub‑progreso de la fase.
//
// APIs para gameplay:
//   RegistrarCaptura(...), RegistrarRecoleccion(...), RegistrarZonaExplorada(...),
//   RegistrarAnalisisMicroscopio(...), ContinuarInicio()
//
public class MainMisiones : MonoBehaviour
{
    // ===================== Configuración de comportamiento =====================
    [Header("Comportamiento")]
    [Tooltip("Si la primera fase es 'InicioMision', al comenzar se avanza automáticamente a la primera TAREA.")]
    public bool AutoSaltarInicioAlAceptar = true;

    // Línea y estado
    [Header("Estado runtime")]
    public Misiones[] misiones;
    public int misionActual = 0;
    private bool lineaActiva = false;

    public bool HayLineaActiva => lineaActiva;

    // Acceso de solo lectura para comparar referencia en Asignador
    public Misiones[] LineaActual => misiones;
    public bool FaseCapturaActiva => HayLineaActiva && (_esperaTipo == "Captura");

    // Helper para saber si una línea (array) es la misma referencia
    public bool EsMismaLinea(Misiones[] otraLinea)
    {
        return otraLinea != null && misiones == otraLinea;
    }

    // ===================== EVENTOS PARA UI =====================

    [Serializable]
    public class MissionSnapshot
    {
        public bool activa;       // ¿hay misión activa?
        public string nombre;     // Nombre de la fase/misión actual
        public string descripcion;// Descripción/Info
        public int faseIndex;     // 1-based
        public int fasesTotal;    // total en la línea
        public string tipo;       // "Inicio", "Captura", "Recolección", "Exploración", "Microscopio", "Resultado"
        public string ubicacion;  // texto simple (ej. primer "Lugar" si aplica)
    }

    // Emite snapshot para el Canvas
    public event Action<MissionSnapshot> OnMissionUI;

    // Sub‑progreso de la fase actual: (restante, objetivo, tipo)
    public event Action<int, int, string> OnMissionSubprogress;

    // ===================== RUNTIME DE FASE =====================

    // Qué “tipo” espera la fase (para validar reportes)
    private string _esperaTipo = "";  // "Inicio","Captura","Recoleccion","Exploracion","Microscopio","Resultado","Fase"

    // Modo simple (total sin distinguir item)
    private int _restante = 0;

    // Modo multi‑ítem (por ID)
    private Dictionary<string, int> _pendientesPorId = null;
    private int _totalObjetivoActual = 0;  // suma de todos los ítems
    private int _totalRestanteActual = 0;  // suma restante (para UI)

    // ===================== API PRINCIPAL =====================

    /// <summary>
    /// Carga una línea (array de 'Misiones') pero NO la activa.
    /// </summary>
    public void CargarLinea(Misiones[] linea)
    {
        misiones = linea;
        misionActual = 0;
        lineaActiva = false;
        Debug.Log("MainMisiones: línea cargada. Lista para comenzar.");
        EmitirUI(); // UI puede mostrar "lista pero no iniciada"
    }

    /// <summary>
    /// Activa la línea y posiciona en la fase 0.
    /// Si 'AutoSaltarInicioAlAceptar' está activo y la fase 0 es 'InicioMision',
    /// se avanza automáticamente a la primera TAREA.
    /// </summary>
    public void ComenzarLinea()
    {
        if (misiones == null || misiones.Length == 0) { Debug.LogWarning("No hay misiones cargadas."); return; }
        lineaActiva = true;

        // 1) Snapshot inicial (fase 0)
        EmitirUI();

        // 2) Preparar lo que espera la fase 0
        PrepararObjetivoFaseActual();

        // 3) Ejecuta la fase (logs/activaciones)
        EjecutarMisionActual();

        // 4) Si la fase 0 es de 'Inicio' y queremos auto-saltar, avanzar de inmediato.
        if (AutoSaltarInicioAlAceptar && _esperaTipo == "Inicio")
        {
            // Opción A: avanzar "limpiamente" usando la API existente (queda todo consistente)
            ContinuarInicio(); // esto llama internamente a CompletarFaseActual()

            // Nota: si prefieres mostrar la info 1 frame antes de saltar,
            // podrías hacerlo en un coroutine con un pequeño delay.
        }

        Debug.Log("MainMisiones: línea comenzada.");
    }

    /// <summary>
    /// Cancela la línea activa (limpia progreso). Queda retomable cargando/aceptando de nuevo.
    /// </summary>
    public void AbortarLineaActual()
    {
        if (!lineaActiva) return;
        Debug.Log("MainMisiones: abortando línea activa. Se perderá el progreso.");
        lineaActiva = false;
        misiones = null;
        misionActual = 0;

        // Limpia runtime
        _esperaTipo = "";
        _restante = 0;
        _pendientesPorId = null;
        _totalObjetivoActual = 0;
        _totalRestanteActual = 0;

        EmitirUI(); // Canvas: "Sin misión activa"
    }

    /// <summary>
    /// Avanza la fase actual (se invoca cuando la fase cumple su objetivo).
    /// </summary>
    public void CompletarFaseActual()
    {
        if (!lineaActiva) { Debug.LogWarning("No hay línea activa."); return; }

        // Avanza índice y re‑prepara siguiente fase
        misionActual++;
        PrepararObjetivoFaseActual();

        EmitirUI(); // Refresca UI tras el avance
        EjecutarMisionActual();
    }

    // ===================== EJECUCIÓN/EMISIÓN =====================

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

        int idx = Mathf.Clamp(misionActual, 0, Mathf.Max(0, misiones.Length - 1));
        Misiones m = misiones[idx];

        s.faseIndex = idx + 1;
        s.fasesTotal = misiones.Length;

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
            if (m.Lugar != null && m.Lugar.Length > 0 && m.Lugar[0] != null)
                s.ubicacion = m.Lugar[0].name;
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

        if (string.IsNullOrEmpty(s.ubicacion)) s.ubicacion = "-";
        return s;
    }

    /// <summary>
    /// Define qué espera la fase actual (contadores simples o multi‑ítem).
    /// </summary>
    private void PrepararObjetivoFaseActual()
    {
        _esperaTipo = "";
        _restante = 0;
        _pendientesPorId = null;
        _totalObjetivoActual = 0;
        _totalRestanteActual = 0;

        if (!lineaActiva || misiones == null || misiones.Length == 0) return;
        if (misionActual < 0 || misionActual >= misiones.Length) return;

        var m = misiones[misionActual];

        if (m.InicioMision)
        {
            _esperaTipo = "Inicio";
            _restante = 1;
            _totalObjetivoActual = _restante;
            _totalRestanteActual = _restante;
        }
        else if (m.EsCaptura)
        {
            _esperaTipo = "Captura";

            // Modo avanzado (IDs)
            if (m.CapturaIDs != null && m.CapturaCant != null &&
                m.CapturaIDs.Length == m.CapturaCant.Length && m.CapturaIDs.Length > 0)
            {
                _pendientesPorId = new Dictionary<string, int>();
                for (int i = 0; i < m.CapturaIDs.Length; i++)
                {
                    int cant = Mathf.Max(1, m.CapturaCant[i]);
                    if (cant <= 0 || string.IsNullOrEmpty(m.CapturaIDs[i])) continue;
                    _pendientesPorId[m.CapturaIDs[i]] = cant;
                    _totalObjetivoActual += cant;
                }
                _totalRestanteActual = _totalObjetivoActual;
            }
            else
            {
                // Modo simple
                _restante = Mathf.Max(1, m.CapturasObjetivo);
                _totalObjetivoActual = _restante;
                _totalRestanteActual = _restante;
            }
        }
        else if (m.EsRecoleccion)
        {
            _esperaTipo = "Recoleccion";

            // Modo avanzado (IDs)
            if (m.RecoleccionIDs != null && m.RecoleccionCant != null &&
                m.RecoleccionIDs.Length == m.RecoleccionCant.Length && m.RecoleccionIDs.Length > 0)
            {
                _pendientesPorId = new Dictionary<string, int>();
                for (int i = 0; i < m.RecoleccionIDs.Length; i++)
                {
                    int cant = Mathf.Max(1, m.RecoleccionCant[i]);
                    if (cant <= 0 || string.IsNullOrEmpty(m.RecoleccionIDs[i])) continue;
                    _pendientesPorId[m.RecoleccionIDs[i]] = cant;
                    _totalObjetivoActual += cant;
                }
                _totalRestanteActual = _totalObjetivoActual;
            }
            else
            {
                // Modo simple
                _restante = Mathf.Max(1, m.RecoleccionesObjetivo);
                _totalObjetivoActual = _restante;
                _totalRestanteActual = _restante;
            }
        }
        else if (m.EsExploracion)
        {
            _esperaTipo = "Exploracion";
            _restante = Mathf.Max(1, m.ZonasObjetivo);
            _totalObjetivoActual = _restante;
            _totalRestanteActual = _restante;
        }
        else if (m.EsMicroscopio)
        {
            _esperaTipo = "Microscopio";
            _restante = Mathf.Max(1, m.AnalisisObjetivo);
            _totalObjetivoActual = _restante;
            _totalRestanteActual = _restante;
        }
        else if (m.EsResultado)
        {
            _esperaTipo = "Resultado";
            _restante = 1;
            _totalObjetivoActual = 1;
            _totalRestanteActual = 1;
        }
        else
        {
            _esperaTipo = "Fase";
            _restante = 1;
            _totalObjetivoActual = 1;
            _totalRestanteActual = 1;
        }

        // Notificar sub‑progreso inicial
        OnMissionSubprogress?.Invoke(_totalRestanteActual, _totalObjetivoActual, _esperaTipo);
    }

    /// <summary>
    /// Muestra logs/activaciones según el tipo de fase.
    /// </summary>
    private void EjecutarMisionActual()
    {
        if (misiones == null || misiones.Length == 0) return;

        // Si pasó del final, se cerró la línea
        if (misionActual < 0 || misionActual >= misiones.Length)
        {
            Debug.Log("Línea completada.");
            lineaActiva = false;
            EmitirUI(); // reflejar finalización
            return;
        }

        Misiones m = misiones[misionActual];

        if (m.InicioMision)
        {
            Debug.Log($"--- Misión: {m.NombreMision} ---");
            Debug.Log($"Dificultad: {m.Dificultad}");
            Debug.Log($"Descripción general:\n{m.InfoMision}");
            // Si AutoSaltarInicioAlAceptar = true, ContinuarInicio() se llamará en ComenzarLinea().
            return;
        }

        if (m.EsCaptura)
        {
            Debug.Log("[FASE CAPTURA ACTIVADA]");
            Debug.Log($"Info captura: {m.InfoCaptura}");
            Debug.Log($"Libélulas Low: {m.LibelulaLow.Length} | High: {m.LibelulaHigt.Length}");
            return;
        }

        if (m.EsRecoleccion)
        {
            Debug.Log("[FASE RECOLECCIÓN ACTIVADA]");
            Debug.Log($"Info recolección: {m.InfoRecoleccion}");
            Debug.Log($"Muestras: {m.Muestra.Length}");
            return;
        }

        if (m.EsExploracion)
        {
            Debug.Log("[FASE EXPLORACIÓN ACTIVADA]");
            Debug.Log($"Info exploración: {m.InfoExploracion}");
            Debug.Log($"Zonas a visitar: {m.Lugar.Length}");
            return;
        }

        if (m.EsMicroscopio)
        {
            Debug.Log("[FASE MICROSCOPIO ACTIVADA]");
            Debug.Log($"Info microscopio: {m.InfoMicroscopio}");
            return;
        }

        if (m.EsResultado)
        {
            Debug.Log("[FASE RESULTADO FINAL ACTIVADA]");
            Debug.Log($"Info final: {m.InfoFinal}");
            Debug.Log($"Datos desbloqueados: {m.InfoDesbloqueada}");
            Debug.Log(">> (BD) Registrar misión completada y desbloquear info.");
            lineaActiva = false;
            EmitirUI(); // Finalización
            return;
        }

        Debug.Log("Estado ACTUAL MISION = " + misionActual);
    }

    // ===================== APIS PARA GAMEPLAY =====================

    /// <summary>
    /// Para la fase de 'Inicio': avanza a la siguiente fase (tareas).
    /// Se usa automáticamente al comenzar si 'AutoSaltarInicioAlAceptar' está activo.
    /// </summary>
    public void ContinuarInicio()
    {
        if (_esperaTipo != "Inicio" || !lineaActiva) return;
        _restante = 0;
        OnMissionSubprogress?.Invoke(0, 1, "Inicio");
        CompletarFaseActual();
    }

    // Captura (modo simple)
    public void RegistrarCaptura(int cantidad = 1)
    {
        if (_esperaTipo != "Captura" || !lineaActiva) return;
        if (_pendientesPorId != null)
        {
            Debug.LogWarning("Esta fase usa Captura por ID. Usa RegistrarCaptura(string id, int cantidad).");
            return;
        }
        _restante -= Mathf.Max(1, cantidad);
        OnMissionSubprogress?.Invoke(Mathf.Max(0, _restante), _totalObjetivoActual, "Captura");
        if (_restante <= 0) CompletarFaseActual();
    }

    // Captura (por especie/ID)
    public void RegistrarCaptura(string id, int cantidad = 1)
    {
        if (_esperaTipo != "Captura" || !lineaActiva) return;

        if (_pendientesPorId != null)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (!_pendientesPorId.ContainsKey(id)) return; // ID no requerido -> ignorar

            int rest = _pendientesPorId[id] - Mathf.Max(1, cantidad);
            _pendientesPorId[id] = Mathf.Max(0, rest);

            // Recalcular total restante
            _totalRestanteActual = 0;
            foreach (var kv in _pendientesPorId) _totalRestanteActual += kv.Value;

            OnMissionSubprogress?.Invoke(_totalRestanteActual, _totalObjetivoActual, "Captura");
            if (_totalRestanteActual <= 0) CompletarFaseActual();
        }
        else
        {
            // Fallback simple
            RegistrarCaptura(cantidad);
        }
    }

    // Recolección (modo simple)
    public void RegistrarRecoleccion(int cantidad = 1)
    {
        if (_esperaTipo != "Recoleccion" || !lineaActiva) return;
        if (_pendientesPorId != null)
        {
            Debug.LogWarning("Esta fase usa Recolección por ID. Usa RegistrarRecoleccion(string id, int cantidad).");
            return;
        }
        _restante -= Mathf.Max(1, cantidad);
        OnMissionSubprogress?.Invoke(Mathf.Max(0, _restante), _totalObjetivoActual, "Recoleccion");
        if (_restante <= 0) CompletarFaseActual();
    }

    // Recolección (por ID)
    public void RegistrarRecoleccion(string id, int cantidad = 1)
    {
        if (_esperaTipo != "Recoleccion" || !lineaActiva) return;

        if (_pendientesPorId != null)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (!_pendientesPorId.ContainsKey(id)) return; // ID no requerido -> ignorar

            int rest = _pendientesPorId[id] - Mathf.Max(1, cantidad);
            _pendientesPorId[id] = Mathf.Max(0, rest);

            _totalRestanteActual = 0;
            foreach (var kv in _pendientesPorId) _totalRestanteActual += kv.Value;

            OnMissionSubprogress?.Invoke(_totalRestanteActual, _totalObjetivoActual, "Recoleccion");
            if (_totalRestanteActual <= 0) CompletarFaseActual();
        }
        else
        {
            // Fallback simple
            RegistrarRecoleccion(cantidad);
        }
    }

    // Exploración (visitar zonas)
    public void RegistrarZonaExplorada(int cantidad = 1)
    {
        if (_esperaTipo != "Exploracion" || !lineaActiva) return;
        _restante -= Mathf.Max(1, cantidad);
        OnMissionSubprogress?.Invoke(Mathf.Max(0, _restante), _totalObjetivoActual, "Exploracion");
        if (_restante <= 0) CompletarFaseActual();
    }

    // Microscopio (analizar muestras)
    public void RegistrarAnalisisMicroscopio(int cantidad = 1)
    {
        if (_esperaTipo != "Microscopio" || !lineaActiva) return;
        _restante -= Mathf.Max(1, cantidad);
        OnMissionSubprogress?.Invoke(Mathf.Max(0, _restante), _totalObjetivoActual, "Microscopio");
        if (_restante <= 0) CompletarFaseActual();
    }

    // Reporte genérico (demo) — sigue disponible
    public void ReportarProgreso(string objetivoId, int delta = 1)
    {
        Debug.Log($"Reporte recibido: {objetivoId} (+{delta}). Completando fase actual.");
        CompletarFaseActual();
    }
}
