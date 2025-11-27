using UnityEngine;
using System.Collections;

public class GestorMisiones : MonoBehaviour
{
    public static GestorMisiones instancia;

    private DatosDeMision misionActual;
    private int indiceFaseActual = 0;

    private Coroutine rutinaActual;

    private bool[] misionesCompletadas = new bool[1000];

    // Datos de recolección
    private int[] progresoRecoleccionActual;
    private FaseRecoleccion faseRecoleccionActual;

    private void Awake()
    {
        instancia = this;
    }

    // ------------------------------------------------------
    // INICIAR MISIÓN
    // ------------------------------------------------------
    public void IniciarMision(DatosDeMision nuevaMision)
    {
        misionActual = nuevaMision;
        indiceFaseActual = 0;

        Debug.Log("Misión iniciada: " + misionActual.nombreMision);

        UI_MisionActiva.instancia.MostrarMision(misionActual, misionActual.fases[0]);

        InterpretarFaseActual();
    }

    // ------------------------------------------------------
    // INTERPRETAR FASE ACTUAL
    // ------------------------------------------------------
    private void InterpretarFaseActual()
    {
        if (misionActual == null) return;

        if (indiceFaseActual >= misionActual.fases.Length)
        {
            FinalizarMision();
            return;
        }

        FaseBase fase = misionActual.fases[indiceFaseActual];

        UI_MisionActiva.instancia.ActualizarFase(fase);
        ActualizarObjetivosEnUI(); // <- seguro aquí no da error

        Debug.Log("Iniciando fase: " + fase.nombreFase);

        // Cancelar tiempo anterior
        if (rutinaActual != null)
            StopCoroutine(rutinaActual);

        // -------------------------------
        // FASE DE RECOLECCIÓN (REAL)
        // -------------------------------
        if (fase is FaseRecoleccion)
        {
            faseRecoleccionActual = (FaseRecoleccion)fase;

            int totalObjetivos = faseRecoleccionActual.objetivos.Length;
            progresoRecoleccionActual = new int[totalObjetivos];

            Debug.Log("Fase RECOLECCIÓN iniciada.");
            ActualizarObjetivosEnUI();
            return; // NO usar temporizador
        }

        // -------------------------------
        // FASES FALSAS (TEMPORIZADOR)
        // -------------------------------
        float tiempo = 2f;

        if (fase is FaseCaptura) tiempo = 3f;
        if (fase is FaseExploracion) tiempo = 2f;
        if (fase is FaseMicroscopio) tiempo = 5f;

        rutinaActual = StartCoroutine(FakeCompletarFase(tiempo));
    }

    private IEnumerator FakeCompletarFase(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        CompletarFaseActual();
    }

    // ------------------------------------------------------
    // COMPLETAR FASE
    // ------------------------------------------------------
    public void CompletarFaseActual()
    {
        if (misionActual == null) return;

        Debug.Log("Fase completada: " + misionActual.fases[indiceFaseActual].nombreFase);

        indiceFaseActual++;

        if (indiceFaseActual >= misionActual.fases.Length)
        {
            FinalizarMision();
            return;
        }

        InterpretarFaseActual();
    }

    // ------------------------------------------------------
    // FINALIZAR MISIÓN
    // ------------------------------------------------------
    private void FinalizarMision()
    {
        Debug.Log("¡Misión COMPLETADA!: " + misionActual.nombreMision);

        misionesCompletadas[misionActual.idMision] = true;

        misionActual = null;
        indiceFaseActual = 0;

        UI_MisionActiva.instancia.OcultarMision();
        UI_MisionActiva.instancia.textoObjetivos.text = "";
    }

    // ------------------------------------------------------
    // FUNCIONES PARA NPC
    // ------------------------------------------------------
    public bool HayMisionActiva() => misionActual != null;

    public int MisionActualID()
    {
        if (misionActual == null) return -1;
        return misionActual.idMision;
    }

    public bool MisionYaCompletada(int id) => misionesCompletadas[id];

    // ------------------------------------------------------
    // RECOLECCIÓN REAL
    // ------------------------------------------------------
    public bool RecogerObjeto(ObjetoRecoleccion objeto)
    {
        if (misionActual == null) return false;

        if (!(misionActual.fases[indiceFaseActual] is FaseRecoleccion))
            return false; // <- MUY IMPORTANTE

        // Buscar objetivo válido
        for (int i = 0; i < faseRecoleccionActual.objetivos.Length; i++)
        {
            if (faseRecoleccionActual.objetivos[i].objeto == objeto)
            {
                progresoRecoleccionActual[i]++;

                Debug.Log($"Recolectado {objeto.nombreObjeto} ({progresoRecoleccionActual[i]}/{faseRecoleccionActual.objetivos[i].cantidadRequerida})");

                ActualizarObjetivosEnUI();

                // Verificar si completó la fase
                for (int j = 0; j < progresoRecoleccionActual.Length; j++)
                {
                    if (progresoRecoleccionActual[j] < faseRecoleccionActual.objetivos[j].cantidadRequerida)
                        return true;
                }

                Debug.Log("Todos los objetos recolectados. Fase completada.");

                CompletarFaseActual();
                return true;
            }
        }

        return false;
    }

    // ------------------------------------------------------
    // ACTUALIZAR OBJETIVOS EN UI
    // ------------------------------------------------------
    private void ActualizarObjetivosEnUI()
    {
        if (misionActual == null) return;

        FaseBase fase = misionActual.fases[indiceFaseActual];

        // PRIMERO: revisamos si es recolección
        if (!(fase is FaseRecoleccion))
        {
            // No es fase de recolección → limpiar UI y salir
            UI_MisionActiva.instancia.textoObjetivos.text = "";
            return;
        }

        // AHORA SÍ podemos hacer cast seguro
        FaseRecoleccion fr = (FaseRecoleccion)fase;

        // PROTECCIÓN CRÍTICA
        if (progresoRecoleccionActual == null ||
            progresoRecoleccionActual.Length != fr.objetivos.Length)
            return;

        string info = "";

        for (int i = 0; i < fr.objetivos.Length; i++)
        {
            info += $"• {fr.objetivos[i].objeto.nombreObjeto} ({progresoRecoleccionActual[i]}/{fr.objetivos[i].cantidadRequerida})\n";
        }

        UI_MisionActiva.instancia.textoObjetivos.text = info;
    }


    // ------------------------------------------------------
    // VER SI UN OBJETO PERTENECE A LA FASE
    // ------------------------------------------------------
    public bool ObjetoEsParteDeRecoleccion(ObjetoRecoleccion obj)
    {
        if (misionActual == null) return false;

        FaseBase fase = misionActual.fases[indiceFaseActual];
        if (!(fase is FaseRecoleccion)) return false;

        var fr = (FaseRecoleccion)fase;

        foreach (var objetivo in fr.objetivos)
        {
            if (objetivo.objeto == obj)
                return true;
        }

        return false;
    }

    public bool ObjetivoDeRecoleccionYaCompleto(ObjetoRecoleccion objeto)
    {
        if (misionActual == null) return false;

        if (!(misionActual.fases[indiceFaseActual] is FaseRecoleccion))
            return false;

        var fr = (FaseRecoleccion)misionActual.fases[indiceFaseActual];

        // PROTECCIÓN CRÍTICA
        if (progresoRecoleccionActual == null ||
            progresoRecoleccionActual.Length != fr.objetivos.Length)
            return false;

        for (int i = 0; i < fr.objetivos.Length; i++)
        {
            if (fr.objetivos[i].objeto == objeto)
            {
                return progresoRecoleccionActual[i] >= fr.objetivos[i].cantidadRequerida;
            }
        }

        return false;
    }
    public bool FaseActualEsRecoleccion()
    {
        if (misionActual == null) return false;
        if (indiceFaseActual >= misionActual.fases.Length) return false;

        return misionActual.fases[indiceFaseActual] is FaseRecoleccion;
    }
    
}
