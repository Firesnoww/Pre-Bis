
using UnityEngine;

public class CamaraOrbital : MonoBehaviour
{
    public Transform jugador;             // Transform del personaje a seguir
    public float distancia = 1.76f; // Distancia de la cámara al personaje
    public float altura = 1.0f; // Altura desde el centro del personaje
    public float sensibilidadMouse = 7.0f; // Sensibilidad al mover el mouse
    public float anguloMin = -20f; // Límite inferior del ángulo vertical
    public float anguloMax = 80f; // Límite superior del ángulo vertical
    public bool detenerRotacion; // Para controlar si rota o no

    private float rotX = 0f; // Rotación acumulada en X (horizontal)
    private float rotY = 0f; // Rotación acumulada en Y (vertical)
    
    void Start()
    {
        Vector3 angulos = transform.eulerAngles; // Obtener rotación inicial de la cámara
        rotX = angulos.y; // Guardar rotación horizontal
        rotY = angulos.x; // Guardar rotación vertical
        //Cursor.lockState = CursorLockMode.Locked; // Bloquear el cursor al centro de la pantalla
        //Cursor.visible = false; // Opcional si queremos ocultar el cursor
    }

    void LateUpdate()
    {
        if (jugador == null) return; // SI no hay jugador asignado se detiene ejecucion

        if (!detenerRotacion)
        {
            // Acumular movimiento del mouse en X e Y
            rotX += Input.GetAxis("Mouse X") * sensibilidadMouse;
            rotY -= Input.GetAxis("Mouse Y") * sensibilidadMouse;

            // Limitar la rotación vertical para evitar giros extraños
            rotY = Mathf.Clamp(rotY, anguloMin, anguloMax);

            // Crear la rotación basada en los ángulos calculados
            Quaternion rotacion = Quaternion.Euler(rotY, rotX, 0f);

            // Calcular la posición de la cámara desde el objetivo con rotación aplicada
            Vector3 offset = rotacion * new Vector3(0, 0, -distancia);

            // Posición base del objetivo + altura deseada
            Vector3 posicionObjetivo = jugador.position + Vector3.up * altura;

            // Asignar nueva posición y rotación a la cámara
            transform.position = posicionObjetivo + offset;
            transform.rotation = rotacion;
        }     
    }
}
