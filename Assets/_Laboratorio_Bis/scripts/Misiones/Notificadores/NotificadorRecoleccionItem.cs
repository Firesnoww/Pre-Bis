using UnityEngine;

//
// NotificadorRecoleccionItem
// Pégalo a cada ítem recolectable. Llama a MainMisiones cuando se recoge.
// - itemID debe coincidir con RecoleccionIDs en el ScriptableObject de la fase.
//
public class NotificadorRecoleccionItem : MonoBehaviour
{
    [SerializeField] private MainMisiones main; // arrástralo en el Inspector
    [SerializeField] private string itemID = "Flor"; // ID esperado en la fase
    [SerializeField] private bool desactivarAlRecoger = true;

    // Llama esto cuando tu lógica de gameplay confirme la recolección
    public void OnRecogido(int cantidad = 1)
    {
        if (main != null)
        {
            if (!string.IsNullOrEmpty(itemID)) main.RegistrarRecoleccion(itemID, cantidad);
            else main.RegistrarRecoleccion(cantidad); // suma simple si no usas IDs
        }
        if (desactivarAlRecoger) gameObject.SetActive(false);
    }
}
