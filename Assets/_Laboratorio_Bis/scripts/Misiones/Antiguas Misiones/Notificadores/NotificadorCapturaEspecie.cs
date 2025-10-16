using UnityEngine;

//
// NotificadorCapturaEspecie
// P�galo a cada lib�lula (o al manager que confirme la captura).
// - especieID debe coincidir con CapturaIDs de la fase si usas modo avanzado.
//
public class NotificadorCapturaEspecie : MonoBehaviour
{
    [SerializeField] private MainMisiones main;
    [SerializeField] private string especieID = "Low"; // Debe empatar con CapturaIDs

    public void OnCapturaConfirmada(int cantidad = 1)
    {
        if (main != null)
        {
            if (!string.IsNullOrEmpty(especieID)) main.RegistrarCaptura(especieID, cantidad);
            else main.RegistrarCaptura(cantidad);
        }
        // aqu� destruye o devuelve al pool la lib�lula si aplica
    }
}
