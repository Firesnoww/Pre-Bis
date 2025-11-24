using UnityEngine;
using System.Collections;

public class GestorMisiones : MonoBehaviour
{
    public static GestorMisiones instancia;

    private DatosDeMision misionActual;
    private int indiceFaseActual = 0;

    // Control interno del temporizador
    private Coroutine rutinaActual;

    private bool[] misionesCompletadas = new bool[1000]; // Cambia tamaño si necesitas más

    private void Awake()
    {
        instancia = this;
    }

    // -----------------------------------
    // INICIAR MISIÓN
    // -----------------------------------
    public void IniciarMision(DatosDeMision nuevaMision)
    {
        misionActual = nuevaMision;
        indiceFaseActual = 0;

        Debug.Log("Misión iniciada: " + misionActual.nombreMision);
        UI_MisionActiva.instancia.MostrarMision(misionActual, misionActual.fases[0]);

        InterpretarFaseActual();
    }

    // -----------------------------------
    // INTERPRETAR TIPO DE FASE
    // (Opción C: solo tiempo fake)
    // -----------------------------------
    private void InterpretarFaseActual()
    {
        if (misionActual == null)
        {
            Debug.LogWarning("No hay misión activa.");
            return;
        }

        if (indiceFaseActual >= misionActual.fases.Length)
        {
            FinalizarMision();
            return;
        }

        FaseBase fase = misionActual.fases[indiceFaseActual];
        UI_MisionActiva.instancia.ActualizarFase(fase);


        Debug.Log("Iniciando fase " + indiceFaseActual + ": " + fase.nombreFase);

        // Cancelar rutina anterior, si existe
        if (rutinaActual != null)
            StopCoroutine(rutinaActual);

        // Elegir tiempo según tipo de fase
        float tiempo = 2f; // default

        if (fase is FaseCaptura)
        {
            tiempo = 3f;
            Debug.Log("Fase tipo CAPTURA → tiempo fake: " + tiempo);
        }
        else if (fase is FaseExploracion)
        {
            tiempo = 2f;
            Debug.Log("Fase tipo EXPLORACIÓN → tiempo fake: " + tiempo);
        }
        else if (fase is FaseRecoleccion)
        {
            tiempo = 4f;
            Debug.Log("Fase tipo RECOLECCIÓN → tiempo fake: " + tiempo);
        }
        else if (fase is FaseMicroscopio)
        {
            tiempo = 5f;
            Debug.Log("Fase tipo MICROSCOPIO → tiempo fake: " + tiempo);
        }

        // Ejecutar finalización falsa de fase
        rutinaActual = StartCoroutine(FakeCompletarFase(tiempo));
    }


    // -----------------------------------
    // CORUTINA QUE ESPERA TIEMPO FAKE
    // -----------------------------------
    private IEnumerator FakeCompletarFase(float segundos)
    {
        Debug.Log("Completando fase en " + segundos + " segundos...");
        yield return new WaitForSeconds(segundos);

        CompletarFaseActual();
    }


    // -----------------------------------
    // COMPLETAR FASE
    // -----------------------------------
    public void CompletarFaseActual()
    {
        if (misionActual == null)
        {
            Debug.LogWarning("Intento de completar fase sin misión activa.");
            return;
        }

        Debug.Log("Fase completada: " + misionActual.fases[indiceFaseActual].nombreFase);

        indiceFaseActual++;

        if (indiceFaseActual >= misionActual.fases.Length)
        {
            FinalizarMision();
            return;
        }

        InterpretarFaseActual();
    }

    // -----------------------------------
    // FINALIZAR MISIÓN
    // -----------------------------------
    private void FinalizarMision()
    {
        Debug.Log("¡Misión COMPLETADA!: " + misionActual.nombreMision);

        misionesCompletadas[misionActual.idMision] = true;

        misionActual = null;
        indiceFaseActual = 0;
        UI_MisionActiva.instancia.OcultarMision();

    }


    // -----------------------------------
    // FUNCIONES QUE USA EL NPC
    // -----------------------------------
    public bool HayMisionActiva()
    {
        return misionActual != null;
    }

    public int MisionActualID()
    {
        if (misionActual == null) return -1;
        return misionActual.idMision;
    }

    public bool MisionYaCompletada(int id)
    {
        return misionesCompletadas[id];
    }
}
