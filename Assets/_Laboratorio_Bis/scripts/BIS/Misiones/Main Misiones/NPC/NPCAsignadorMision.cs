using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCAsignadorMisionMulti : MonoBehaviour
{
    [Header("Misiones que este NPC entrega")]
    public DatosDeMision[] misionesDisponibles;

    [Header("Canvas de interacción (presiona E)")]
    public GameObject canvasProximidad;

    [Header("Canvas con la lista de misiones")]
    public GameObject canvasListaMisiones;

    [Header("Canvas que muestra info de la misión seleccionada")]
    public GameObject canvasInfoMision;

    [Header("Campos de UI para info")]
    public TMP_Text textoNombre;
    public TMP_Text textoDescripcion;

    [Header("Canvas de confirmación si se cambia misión")]
    public GameObject canvasConfirmacion;

    [Header("Tag del jugador")]
    public string tagJugador = "Player";

    private DatosDeMision misionSeleccionada;
    private bool jugadorDentro = false;


    private void Start()
    {
        if (canvasProximidad) canvasProximidad.SetActive(false);
        if (canvasListaMisiones) canvasListaMisiones.SetActive(false);
        if (canvasInfoMision) canvasInfoMision.SetActive(false);
        if (canvasConfirmacion) canvasConfirmacion.SetActive(false);
    }

    // ------------------------------------------------------------
    // DETECTAR PROXIMIDAD
    // ------------------------------------------------------------
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        jugadorDentro = true;

        if (!canvasProximidad.activeSelf)
            canvasProximidad.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        jugadorDentro = false;

        canvasProximidad.SetActive(false);
        canvasListaMisiones.SetActive(false);
        canvasInfoMision.SetActive(false);
        canvasConfirmacion.SetActive(false);
    }

    private void Update()
    {
        if (!jugadorDentro) return;

        // El jugador debe presionar E para abrir el menú de misiones
        if (Input.GetKeyDown(KeyCode.E))
        {
            canvasProximidad.SetActive(false);
            canvasListaMisiones.SetActive(true);
        }
    }

    // ------------------------------------------------------------
    // SELECCIONAR MISIÓN (desde botones de la lista)
    // ------------------------------------------------------------
    public void SeleccionarMision(int indice)
    {
        if (indice < 0 || indice >= misionesDisponibles.Length)
            return;

        DatosDeMision mis = misionesDisponibles[indice];

        // 1. BLOQUEO: ¿ya está completada?
        if (GestorMisiones.instancia.MisionYaCompletada(mis.idMision))
        {
            Debug.Log("Esta misión ya está completada. Bloqueada.");
            return;
        }

        misionSeleccionada = mis;

        // 2. Mostrar datos de la misión en el canvas de info
        textoNombre.text = mis.nombreMision;
        textoDescripcion.text = mis.descripcionMision;

        canvasInfoMision.SetActive(true);
    }

    // ------------------------------------------------------------
    // BOTÓN: ACEPTAR MISIÓN
    // ------------------------------------------------------------
    public void AceptarMision()
    {
        // Si hay misión activa Y es distinta → confirmación
        if (GestorMisiones.instancia.HayMisionActiva() &&
            GestorMisiones.instancia.MisionActualID() != misionSeleccionada.idMision)
        {
            canvasConfirmacion.SetActive(true);
        }
        else
        {
            EntregarMisionFinal();
        }
    }

    // ------------------------------------------------------------
    // BOTÓN: CANCELAR
    // ------------------------------------------------------------
    public void CancelarInfo()
    {
        misionSeleccionada = null;
        canvasInfoMision.SetActive(false);
    }

    // ------------------------------------------------------------
    // CONFIRMAR CAMBIO DE MISIÓN
    // ------------------------------------------------------------
    public void ConfirmarCambio()
    {
        EntregarMisionFinal();
        canvasConfirmacion.SetActive(false);
    }

    public void CancelarCambio()
    {
        canvasConfirmacion.SetActive(false);
    }

    // ------------------------------------------------------------
    // ENTREGA FINAL
    // ------------------------------------------------------------
    private void EntregarMisionFinal()
    {
        GestorMisiones.instancia.IniciarMision(misionSeleccionada);
        canvasInfoMision.SetActive(false);
        canvasListaMisiones.SetActive(false);
        Debug.Log("Misión iniciada: " + misionSeleccionada.nombreMision);
    }
}
