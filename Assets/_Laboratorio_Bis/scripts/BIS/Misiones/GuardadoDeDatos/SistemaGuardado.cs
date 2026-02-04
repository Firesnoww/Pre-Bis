using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SistemaGuardado : MonoBehaviour
{
    public static SistemaGuardado instancia;

    private string rutaArchivo;
    public DatosGuardados Datos { get; private set; }

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        string carpeta = Path.Combine(Application.dataPath, "_Laboratorio_Bis", "SaveData");

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        rutaArchivo = Path.Combine(carpeta, "datos_juego.json");

        CargarDatos();
    }

    public void CargarDatos()
    {
        if (File.Exists(rutaArchivo))
        {
            string contenido = File.ReadAllText(rutaArchivo);
            Datos = JsonUtility.FromJson<DatosGuardados>(contenido);
        }
        else
        {
            Datos = new DatosGuardados();
            GuardarDatos();
        }

        AplicarDatosAlJuego();
    }

    public void GuardarDatos()
    {
        RecogerDatosDelJuego();

        string contenido = JsonUtility.ToJson(Datos, true);
        File.WriteAllText(rutaArchivo, contenido);

        Debug.Log("Datos guardados en: " + rutaArchivo);
    }

    public void CrearArchivoNuevo()
    {
        Datos = new DatosGuardados();
        GuardarDatos();
    }


    // --------------------------------------------------------
    // PASAR DATOS → JUEGO
    // --------------------------------------------------------
    private void AplicarDatosAlJuego()
    {
        // MISIÓN ACTIVA
        if (Datos.misiones.misionActivaID != -1)
        {
            var mision = GestorMisiones.instancia.BuscarMisionPorID(Datos.misiones.misionActivaID);

            if (mision != null)
            {
                GestorMisiones.instancia.CargarMisionDesdeDatos(
                    mision,
                    Datos.misiones.faseActiva,
                    Datos.misiones.progresoFase  // ← ahora es List<int>
                );
            }
        }

        foreach (int id in Datos.misiones.misionesCompletadas)
        {
            GestorMisiones.instancia.MarcarMisionComoCompletada(id);
        }

        //GestorMisiones.instancia.CargarMisionesCompletadas(Datos.misiones.misionesCompletadas);--------------aca esta el "problema"-------
        // Más adelante:
        // - Aplicar captura
        // - Aplicar info de objetos
        // - Aplicar zonas desbloqueadas
    }


    // --------------------------------------------------------
    // PASAR JUEGO → DATOS
    // --------------------------------------------------------
    public void RecogerDatosDelJuego()
    {
        var gm = GestorMisiones.instancia;

        // MISIÓN ACTIVA
        Datos.misiones.misionActivaID = gm.MisionActualID();
        Datos.misiones.faseActiva = gm.FaseActualIndex();

        // PROGRESO DE RECOLECCIÓN (List<int>)
        Datos.misiones.progresoFase =
            gm.ObtenerProgresoActualComoLista();

        // MISIONES COMPLETADAS (List<int>)
        Datos.misiones.misionesCompletadas =
            new List<int>(gm.ObtenerMisionesCompletadasArray());
    }

    // --------------------------------------------------------
    // 🔥 BORRAR SOLO LOS DATOS DE LAS MISIONES (DEBUG)
    // --------------------------------------------------------
    [ContextMenu("DEBUG: Borrar solo datos de misiones")]
    public void BorrarSoloDatosDeMisiones()
    {
        Debug.Log("⚠ Borrando SOLO datos de misiones...");

        // Reiniciar datos de misión
        Datos.misiones.misionActivaID = -1;
        Datos.misiones.faseActiva = 0;
        Datos.misiones.progresoFase.Clear();
        Datos.misiones.misionesCompletadas.Clear();

        // Guardar inmediatamente
        GuardarDatos();

        Debug.Log("✔ Datos de misiones limpiados.");
    }

}
