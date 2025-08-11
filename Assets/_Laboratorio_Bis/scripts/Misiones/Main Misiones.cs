using UnityEngine;

public class MainMisiones : MonoBehaviour
{
    public Misiones[] misiones;
    public int misionActual = 0;
    private bool lineaActiva = false;

    // Cargar desde el Asignador
    public void CargarLinea(Misiones[] linea)
    {
        misiones = linea;
        misionActual = 0;
        lineaActiva = false;
        Debug.Log("MainMisiones: l�nea cargada. Lista para comenzar.");
    }

    // Mostrar intro y quedar listos para fase 1
    public void ComenzarLinea()
    {
        if (misiones == null || misiones.Length == 0) { Debug.LogWarning("No hay misiones cargadas."); return; }

        lineaActiva = true;
        EjecutarMisionActual(); // esto mostrar� la intro (InicioMision = true) y har� misionActual++
        Debug.Log("MainMisiones: intro mostrada. Esperando completar fase 1...");
    }

    // Llamar cuando la fase actual se cumpla (por ahora manual; luego por eventos)
    public void CompletarFaseActual()
    {
        if (!lineaActiva) { Debug.LogWarning("No hay l�nea activa."); return; }
        EjecutarMisionActual(); // procesa la siguiente fase
    }

    void EjecutarMisionActual()
    {
        if (misiones == null || misiones.Length == 0) return;
        if (misionActual < 0 || misionActual >= misiones.Length) { Debug.Log("L�nea completada."); lineaActiva = false; return; }

        Misiones m = misiones[misionActual];

        if (m.InicioMision)
        {
            Debug.Log($"--- Misi�n: {m.NombreMision} ---");
            Debug.Log($"Dificultad: {m.Dificultad}");
            Debug.Log($"Descripci�n general:\n{m.InfoMision}");
            misionActual++;
            return;
        }

        if (m.EsCaptura)
        {
            Debug.Log("[FASE CAPTURA ACTIVADA]");
            Debug.Log($"Info captura: {m.InfoCaptura}");
            Debug.Log($"Lib�lulas Low: {m.LibelulaLow.Length} | High: {m.LibelulaHigt.Length}");
            // Aqu� esperas a que te avisen que se captur� -> luego llamas CompletarFaseActual()
            misionActual++;
            return;
        }

        if (m.EsRecoleccion)
        {
            Debug.Log("[FASE RECOLECCI�N ACTIVADA]");
            Debug.Log($"Info recolecci�n: {m.InfoRecoleccion}");
            Debug.Log($"Muestras: {m.Muestra.Length}");
            misionActual++;
            return;
        }

        if (m.EsExploracion)
        {
            Debug.Log("[FASE EXPLORACI�N ACTIVADA]");
            Debug.Log($"Info exploraci�n: {m.InfoExploracion}");
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
            Debug.Log(">> (BD) Registrar misi�n completada y desbloquear info.");
            lineaActiva = false;
            return;
        }

        Debug.Log("Estado ACTUAL MISION = " + misionActual);
    }

    // API m�nima para objetos externos (luego la conectamos a objetivos reales)
    public void ReportarProgreso(string objetivoId, int delta = 1)
    {
        // Por ahora, demo: si recibes un reporte, completas la fase:
        Debug.Log($"Reporte recibido: {objetivoId} (+{delta}). Completando fase actual.");
        CompletarFaseActual();
    }
}
