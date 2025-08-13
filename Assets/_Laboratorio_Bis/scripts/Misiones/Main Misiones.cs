using UnityEngine;
using System;


public class MainMisiones : MonoBehaviour
{
    [Serializable]
    public class MissionSnapshot
    {
        public bool activa;            // �hay una l�nea activa?
        public string nombre;          // Nombre de la misi�n/fase actual (si aplica)
        public string descripcion;     // Descripci�n/Info de la fase
        public int faseIndex;          // �ndice de fase actual (1-based para UI)
        public int fasesTotal;         // Total de fases en la l�nea
        public string tipo;            // "Inicio", "Captura", "Recolecci�n", "Exploraci�n", "Microscopio", "Resultado"
        public string ubicacion;       // Si hay lugares (Exploraci�n), un texto breve
    }
    public Misiones[] misiones;
    public int misionActual = 0;
    private bool lineaActiva = false;
    public event Action<MissionSnapshot> OnMissionUI;
    public bool HayLineaActiva => lineaActiva;

    // >>> NUEVO: exposici�n segura de la referencia actual
    public Misiones[] LineaActual => misiones;

    // >>> NUEVO: helper para comparar si es la misma l�nea por referencia

    // Qu� espera la fase actual (en memoria)
    private string _esperaTipo = "";   // "Captura", "Recoleccion", "Exploracion", "Microscopio", "Inicio", "Resultado"
    private int _restante = 0;         // cu�ntos faltan para completar la fase

    // Evento: notificar progreso de la fase actual al UI
    public event System.Action<int, int, string> OnMissionSubprogress;
    // firma: (restante, objetivoTotal, tipo)

    public bool EsMismaLinea(Misiones[] otraLinea)
    {
        return otraLinea != null && misiones == otraLinea;
    }

    public void CargarLinea(Misiones[] linea)
    {
        misiones = linea;
        misionActual = 0;
        lineaActiva = false;
        Debug.Log("MainMisiones: l�nea cargada. Lista para comenzar.");
        EmitirUI(); // <-- NUEVO: para que el UI muestre "lista pero no iniciada"
    }

    public void ComenzarLinea()
    {
        if (misiones == null || misiones.Length == 0) { Debug.LogWarning("No hay misiones cargadas."); return; }
        lineaActiva = true;

        EmitirUI();                // ya lo ten�as del parche anterior
        PrepararObjetivoFaseActual(); // <-- NUEVO (armar objetivo)
        EjecutarMisionActual();

        Debug.Log("MainMisiones: intro mostrada. Esperando completar fase 1...");
    }

    public void AbortarLineaActual()
    {
        if (!lineaActiva) return;
        Debug.Log("MainMisiones: abortando l�nea activa. Se perder� el progreso.");
        lineaActiva = false;
        misiones = null;
        misionActual = 0;
        EmitirUI(); // <-- NUEVO: UI pasa a "Sin misi�n activa"
    }

    public void CompletarFaseActual()
    {
        if (!lineaActiva) { Debug.LogWarning("No hay l�nea activa."); return; }
        EjecutarMisionActual();
        EmitirUI(); // <-- NUEVO: tras avanzar, refrescar UI
    }

    void EjecutarMisionActual()
    {
        if (misiones == null || misiones.Length == 0) return;
        if (misionActual < 0 || misionActual >= misiones.Length)
        {
            Debug.Log("L�nea completada.");
            lineaActiva = false;
            EmitirUI(); // <-- NUEVO: refleja finalizaci�n
            return;
        }

        Misiones m = misiones[misionActual];

        if (m.InicioMision)
        {
            Debug.Log($"--- Misi�n: {m.NombreMision} ---");
            Debug.Log($"Dificultad: {m.Dificultad}");
            Debug.Log($"Descripci�n general:\n{m.InfoMision}");
            // Importante: emitir (ya se emite fuera) y luego avanzar �ndice
            misionActual++;
            PrepararObjetivoFaseActual(); // <-- NUEVO
            return;
        }

        if (m.EsCaptura)
        {
            Debug.Log("[FASE CAPTURA ACTIVADA]");
            Debug.Log($"Info captura: {m.InfoCaptura}");
            Debug.Log($"Lib�lulas Low: {m.LibelulaLow.Length} | High: {m.LibelulaHigt.Length}");
            misionActual++;
            return;
        }

        if (m.EsRecoleccion)
        {
            Debug.Log("[FASE RECOLECCI�N ACTIVADA]");
            Debug.Log($"Info recolecci�n: {m.InfoRecoleccion}");
            Debug.Log($"Muestras: {m.Muestra.Length}");
            misionActual++;
            PrepararObjetivoFaseActual(); // <-- NUEVO
            return;
        }

        if (m.EsExploracion)
        {
            Debug.Log("[FASE EXPLORACI�N ACTIVADA]");
            Debug.Log($"Info exploraci�n: {m.InfoExploracion}");
            Debug.Log($"Zonas a visitar: {m.Lugar.Length}");
            misionActual++;
            PrepararObjetivoFaseActual(); // <-- NUEVO
            return;
        }

        if (m.EsMicroscopio)
        {
            Debug.Log("[FASE MICROSCOPIO ACTIVADA]");
            Debug.Log($"Info microscopio: {m.InfoMicroscopio}");
            misionActual++;
            PrepararObjetivoFaseActual(); // <-- NUEVO
            return;
        }

        if (m.EsResultado)
        {
            Debug.Log("[FASE RESULTADO FINAL ACTIVADA]");
            Debug.Log($"Info final: {m.InfoFinal}");
            Debug.Log($"Datos desbloqueados: {m.InfoDesbloqueada}");
            Debug.Log(">> (BD) Registrar misi�n completada y desbloquear info.");
            lineaActiva = false;
            EmitirUI(); // <-- NUEVO: refleja finalizaci�n
            return;
        }

        Debug.Log("Estado ACTUAL MISION = " + misionActual);
    }

    // API m�nima para objetos externos (luego la conectamos a objetivos reales)
    public void ReportarProgreso(string objetivoId, int delta = 1)
    {
        Debug.Log($"Reporte recibido: {objetivoId} (+{delta}). Completando fase actual.");
        CompletarFaseActual();
    }

    public void RegistrarCaptura(int cantidad = 1)
    {
        if (_esperaTipo != "Captura" || !lineaActiva) return;
        _restante -= Mathf.Max(1, cantidad);
        OnMissionSubprogress?.Invoke(Mathf.Max(0, _restante), Mathf.Max(1, misiones[misionActual].CapturasObjetivo), "Captura");
        if (_restante <= 0) CompletarFaseActual();
    }

    public void RegistrarRecoleccion(int cantidad = 1)
    {
        if (_esperaTipo != "Recoleccion" || !lineaActiva) return;
        _restante -= Mathf.Max(1, cantidad);
        OnMissionSubprogress?.Invoke(Mathf.Max(0, _restante), Mathf.Max(1, misiones[misionActual].RecoleccionesObjetivo), "Recoleccion");
        if (_restante <= 0) CompletarFaseActual();
    }

    public void RegistrarZonaExplorada(int cantidad = 1)
    {
        if (_esperaTipo != "Exploracion" || !lineaActiva) return;
        _restante -= Mathf.Max(1, cantidad);
        OnMissionSubprogress?.Invoke(Mathf.Max(0, _restante), Mathf.Max(1, misiones[misionActual].ZonasObjetivo), "Exploracion");
        if (_restante <= 0) CompletarFaseActual();
    }

    public void RegistrarAnalisisMicroscopio(int cantidad = 1)
    {
        if (_esperaTipo != "Microscopio" || !lineaActiva) return;
        _restante -= Mathf.Max(1, cantidad);
        OnMissionSubprogress?.Invoke(Mathf.Max(0, _restante), Mathf.Max(1, misiones[misionActual].AnalisisObjetivo), "Microscopio");
        if (_restante <= 0) CompletarFaseActual();
    }

    // Para la fase de Inicio o gen�rica: puedes �continuar� con un bot�n
    public void ContinuarInicio()
    {
        if (_esperaTipo != "Inicio" || !lineaActiva) return;
        _restante = 0;
        OnMissionSubprogress?.Invoke(0, 1, "Inicio");
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
            s.nombre = "Sin misi�n activa";
            s.descripcion = "";
            s.faseIndex = 0;
            s.fasesTotal = 0;
            s.tipo = "";
            s.ubicacion = "";
            return s;
        }

        // Proteger �ndice
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
            s.tipo = "Recolecci�n";
            s.nombre = m.NombreMision;
            s.descripcion = string.IsNullOrEmpty(m.InfoRecoleccion) ? m.InfoMision : m.InfoRecoleccion;
        }
        else if (m.EsExploracion)
        {
            s.tipo = "Exploraci�n";
            s.nombre = m.NombreMision;
            s.descripcion = string.IsNullOrEmpty(m.InfoExploracion) ? m.InfoMision : m.InfoExploracion;

            // Ubicaci�n amigable si definiste lugares
            if (m.Lugar != null && m.Lugar.Length > 0 && m.Lugar[0] != null)
                s.ubicacion = m.Lugar[0].name; // puedes mejorar esto luego (p.ej. el m�s cercano al jugador)
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

        // Fallback ubicaci�n si no es exploraci�n
        if (string.IsNullOrEmpty(s.ubicacion)) s.ubicacion = "-";

        return s;
    }

    private void PrepararObjetivoFaseActual()
    {
        _esperaTipo = "";
        _restante = 0;

        if (!lineaActiva || misiones == null || misiones.Length == 0) return;
        if (misionActual < 0 || misionActual >= misiones.Length) return;

        var m = misiones[misionActual];

        if (m.InicioMision) { _esperaTipo = "Inicio"; _restante = 1; }
        else if (m.EsCaptura) { _esperaTipo = "Captura"; _restante = Mathf.Max(1, m.CapturasObjetivo); }
        else if (m.EsRecoleccion) { _esperaTipo = "Recoleccion"; _restante = Mathf.Max(1, m.RecoleccionesObjetivo); }
        else if (m.EsExploracion) { _esperaTipo = "Exploracion"; _restante = Mathf.Max(1, m.ZonasObjetivo); }
        else if (m.EsMicroscopio) { _esperaTipo = "Microscopio"; _restante = Mathf.Max(1, m.AnalisisObjetivo); }
        else if (m.EsResultado) { _esperaTipo = "Resultado"; _restante = 1; }
        else { _esperaTipo = "Fase"; _restante = 1; }

        // Notificar subprogreso inicial (objetivo total = _restante)
        OnMissionSubprogress?.Invoke(_restante, _restante, _esperaTipo);
    }
}
