using System.IO;
using UnityEngine;

public class SistemaGuardado : MonoBehaviour
{
    public static SistemaGuardado instancia;

    private string rutaArchivo;
    public DatosGuardados Datos { get; private set; }

    private void Awake()
    {
        // Singleton
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

        rutaArchivo = Application.persistentDataPath + "/datos_juego.json";
        CargarDatos();
    }

    // --------------------------------------------------------
    // CARGAR DATOS
    // --------------------------------------------------------
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
            CrearArchivoNuevo();
        }

        // Sincronizar con sistemas del juego
        AplicarDatosAlJuego();
    }

    // --------------------------------------------------------
    // GUARDAR DATOS
    // --------------------------------------------------------
    public void GuardarDatos()
    {
        // Actualizar datos con lo que esté pasando en el juego
        RecogerDatosDelJuego();

        string contenido = JsonUtility.ToJson(Datos, true);
        File.WriteAllText(rutaArchivo, contenido);

        Debug.Log("Datos guardados correctamente.");
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
