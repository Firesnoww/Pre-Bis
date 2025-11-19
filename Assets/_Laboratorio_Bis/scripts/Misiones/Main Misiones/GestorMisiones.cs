using UnityEngine;

public class GestorMisiones : MonoBehaviour
{
    public static GestorMisiones instancia;

    private DatosDeMision misionActual;
    private int indiceFaseActual = 0;

    private void Awake()
    {
        instancia = this;
    }

    // -------------------------------
    // INICIAR MISIÓN
    // -------------------------------
    public void IniciarMision(DatosDeMision nuevaMision)
    {
        misionActual = nuevaMision;
        indiceFaseActual = 0;

        Debug.Log("Misión iniciada: " + misionActual.nombreMision);
        MostrarFaseActual();
    }

    // -------------------------------
    // MOSTRAR FASE ACTUAL
    // -------------------------------
    public void MostrarFaseActual()
    {
        if (misionActual == null)
        {
            Debug.Log("No hay misión activa.");
            return;
        }

        if (indiceFaseActual >= misionActual.fases.Length)
        {
            Debug.Log("No hay más fases.");
            return;
        }

        FaseBase fase = misionActual.fases[indiceFaseActual];

        Debug.Log("Fase " + indiceFaseActual + " | " + fase.nombreFase);
        Debug.Log(fase.descripcionFase);
    }

    // -------------------------------
    // AVANZAR FASE
    // -------------------------------
    public void CompletarFaseActual()
    {
        if (misionActual == null)
        {
            Debug.Log("No hay misión activa para avanzar.");
            return;
        }

        indiceFaseActual++;

        if (indiceFaseActual >= misionActual.fases.Length)
        {
            FinalizarMision();
            return;
        }

        MostrarFaseActual();
    }

    // -------------------------------
    // FINALIZAR MISIÓN
    // -------------------------------
    private void FinalizarMision()
    {
        Debug.Log("Misión completada: " + misionActual.nombreMision);

        misionActual = null;
        indiceFaseActual = 0;
    }
}
