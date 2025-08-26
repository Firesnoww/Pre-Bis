using UnityEngine;

//
// NotificadorMicroscopio
// Llama a MainMisiones cuando el jugador completa un an�lisis en el minijuego.
//
public class NotificadorMicroscopio : MonoBehaviour
{
    [SerializeField] private MainMisiones main;

    // Llama esto cuando el an�lisis sea exitoso
    public void OnAnalisisCompletado(int cantidad = 1)
    {
        if (main != null) main.RegistrarAnalisisMicroscopio(cantidad);
    }
}
