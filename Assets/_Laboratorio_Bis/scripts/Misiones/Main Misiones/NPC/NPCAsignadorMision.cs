using UnityEngine;

public class NPCAsignadorMision : MonoBehaviour
{
    [Header("Misión que este NPC debe entregar")]
    public DatosDeMision misionAEntregar;

    [Header("Canvas UI que se muestra cuando el jugador está cerca")]
    public GameObject canvasInteraccion;

    [Header("Tag del jugador")]
    public string tagJugador = "Player";

    private bool jugadorDentro = false;

    private void Start()
    {
        // Asegurar que el canvas empiece oculto
        if (canvasInteraccion != null)
            canvasInteraccion.SetActive(false);
    }

    // -----------------------------------
    // DETECTAR QUE EL JUGADOR ESTÁ EN LA ZONA
    // -----------------------------------
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            if (!jugadorDentro)
            {
                jugadorDentro = true;

                if (canvasInteraccion != null)
                    canvasInteraccion.SetActive(true);
            }
        }
    }

    // -----------------------------------
    // OCULTAR CANVAS AL SALIR
    // -----------------------------------
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            jugadorDentro = false;

            if (canvasInteraccion != null)
                canvasInteraccion.SetActive(false);
        }
    }

    // -----------------------------------
    // ENTREGAR LA MISIÓN (botón del canvas llama esto)
    // -----------------------------------
    public void EntregarMision()
    {
        if (misionAEntregar == null)
        {
            Debug.LogError("NPC no tiene una misión asignada.");
            return;
        }

        GestorMisiones.instancia.IniciarMision(misionAEntregar);
    }
}
