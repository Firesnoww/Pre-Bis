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

        // 📁 Carpeta dentro del proyecto:
        // Assets/_Laboratorio_Bis/SaveData/
        string carpeta = Path.Combine(Application.dataPath, "_Laboratorio_Bis", "SaveData");

        // Crear la carpeta si no existe
        if (!Directory.Exists(carpeta))
        {
            Directory.CreateDirectory(carpeta);
        }

        // Ruta final del archivo JSON
        rutaArchivo = Path.Combine(carpeta, "datos_juego.json");

        Debug.Log("Guardado en: " + rutaArchivo);

        CargarDatos();
    }

    public void CargarDatos()
    {
        if (File.Exists(rutaArchivo))
        {
            string contenido = File.ReadAllText(rutaArchivo);
            Datos = JsonUtility.FromJson<DatosGuardados>(contenido);
            Debug.Log("Datos cargados correctamente.");
        }
        else
        {
            Debug.Log("No existe archivo de guardado → creando nuevo.");
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

        Debug.Log("Datos guardados correctamente en: " + rutaArchivo);
    }

    // --------------------------------------------------------
    // CREAR NUEVO ARCHIVO DE GUARDADO
    // --------------------------------------------------------
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
                    Datos.misiones.progresoFase
                );
            }
        }

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

        // PROGRESO DE RECOLECCIÓN
        Datos.misiones.progresoFase =
            gm.ObtenerProgresoActualComoDiccionario();

        // MISIÓN COMPLETADAS
        Datos.misiones.misionesCompletadas =
            gm.ObtenerMisionesCompletadasArray();
    }
}
