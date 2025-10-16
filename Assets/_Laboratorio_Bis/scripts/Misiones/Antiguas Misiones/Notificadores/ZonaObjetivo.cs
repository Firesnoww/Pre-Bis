using UnityEngine;

//
// ZonaObjetivo
// Pégalo a un GameObject con Collider (isTrigger) que represente una zona a visitar.
// Al entrar el jugador, notifica a MainMisiones.
//
[RequireComponent(typeof(Collider))]
public class ZonaObjetivo : MonoBehaviour
{
    [SerializeField] private MainMisiones main;
    [SerializeField] private bool consumirAlEntrar = true;
    [SerializeField] private string tagJugador = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;
        if (main != null) main.RegistrarZonaExplorada(1);
        if (consumirAlEntrar) gameObject.SetActive(false);
    }
}
