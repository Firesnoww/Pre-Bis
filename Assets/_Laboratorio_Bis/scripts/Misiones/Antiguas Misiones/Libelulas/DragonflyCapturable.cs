using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DragonflyCapturable : MonoBehaviour
{
    [Header("Opcional: referenciar el notificador de esta lib�lula")]
    public NotificadorCapturaEspecie notificador;

    [Header("Estado (solo lectura)")]
    public bool enZonaCaptura = false;

    private void Reset()
    {
        // Por si el collider no es trigger (la lib�lula puede tener collider normal)
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = false;
    }

    // Estos dos m�todos los invoca la zona de captura
    public void MarcarEnZona(bool valor)
    {
        enZonaCaptura = valor;
    }

    // Helper para capturar (lo llama la Red)
    public void Capturar(int cantidad = 1)
    {
        if (notificador != null)
        {
            notificador.OnCapturaConfirmada(cantidad);
        }
        // Si no hay notificador, no hacemos nada para respetar tu arquitectura.
        // (Puedes a�adir aqu� devolver al pool/desactivar si quieres)
    }
}
