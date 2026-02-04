using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestorMisiones : MonoBehaviour
{
    public static GestorMisiones instancia;

    [Header("Listado global de misiones")]
    public DatosDeMision[] todasLasMisiones;   // <- lo llenas desde el Inspector

    private DatosDeMision misionActual;
    private int indiceFaseActual = 0;

    private Coroutine rutinaActual;

    public bool[] misionesCompletadas = new bool[1000];

    // Recolección
    private int[] progresoRecoleccionActual;
    private FaseRecoleccion faseRecoleccionActual;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ------------------------------------------------------
    // INICIAR MISIÓN NORMAL (desde NPC)
    // ------------------------------------------------------
    public void IniciarMision(DatosDeMision nuevaMision)
    {
        misionActual = nuevaMision;
        indiceFaseActual = 0;
        progresoRecoleccionActual = null;
        faseRecoleccionActual = null;

        Debug.Log("Misión iniciada: " + misionActual.nombreMision);

        UI_MisionActiva.instancia.MostrarMision(misionActual, misionActual.fases[0]);

        InterpretarFaseActual();

        // Guardado automático al iniciar
        //if (SistemaGuardado.instancia != null)
            //SistemaGuardado.instancia.GuardarDatos();-----------------------------------Temporal---------------------------------------------------
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
        ActualizarObjetivosEnUI();

        Debug.Log("Iniciando fase: " + fase.nombreFase);

        // Cancelar rutina anterior
        if (rutinaActual != null)
            StopCoroutine(rutinaActual);

        // -------------------------------
        // FASE DE RECOLECCIÓN
        // -------------------------------
        if (fase is FaseRecoleccion)
        {
            faseRecoleccionActual = (FaseRecoleccion)fase;
            int totalObjetivos = faseRecoleccionActual.objetivos.Length;

            // 🔥 SOLO crear array si no existe o si está corrupto
            if (progresoRecoleccionActual == null ||
                progresoRecoleccionActual.Length != totalObjetivos)
            {
                progresoRecoleccionActual = new int[totalObjetivos];
            }

            Debug.Log("Fase RECOLECCIÓN restaurada. Progreso actual:");
            for (int i = 0; i < progresoRecoleccionActual.Length; i++)
                Debug.Log($"Objetivo {i}: {progresoRecoleccionActual[i]}");

            ActualizarObjetivosEnUI();

            return;
        }

        if (SistemaGuardado.instancia != null)
            SistemaGuardado.instancia.GuardarDatos();
        // -------------------------------
        // FASES CON TEMPORIZADOR FAKE
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
        }
        else
        {
            InterpretarFaseActual();
        }

        // 🔹 Guardar cada vez que cambia de fase
        if (SistemaGuardado.instancia != null)
            SistemaGuardado.instancia.GuardarDatos();
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
        progresoRecoleccionActual = null;
        faseRecoleccionActual = null;

        UI_MisionActiva.instancia.OcultarMision();
        UI_MisionActiva.instancia.textoObjetivos.text = "";

        // Guardar estado sin misión activa
        if (SistemaGuardado.instancia != null)
            SistemaGuardado.instancia.GuardarDatos();
    }

    // ------------------------------------------------------
    // FUNCIONES PARA NPC / GUARDADO
    // ------------------------------------------------------
    public bool HayMisionActiva() => misionActual != null;

    public int MisionActualID()
    {
        if (misionActual == null) return -1;
        return misionActual.idMision;
    }

    public bool MisionYaCompletada(int id) => misionesCompletadas[id];

    public int FaseActualIndex() => indiceFaseActual;

    public int[] ObtenerMisionesCompletadasArray()
    {
        List<int> lista = new List<int>();

        for (int i = 0; i < misionesCompletadas.Length; i++)
        {
            if (misionesCompletadas[i])
                lista.Add(i);
        }

        return lista.ToArray();
    }

    // ------------------------------------------------------
    // RECOLECCIÓN REAL
    // ------------------------------------------------------
    public bool RecogerObjeto(ObjetoRecoleccion objeto)
    {
        if (misionActual == null) return false;

        if (!(misionActual.fases[indiceFaseActual] is FaseRecoleccion))
            return false;

        for (int i = 0; i < faseRecoleccionActual.objetivos.Length; i++)
        {
            if (faseRecoleccionActual.objetivos[i].objeto == objeto)
            {
                // Si ya está completo este objetivo, no sumamos más
                if (progresoRecoleccionActual[i] >= faseRecoleccionActual.objetivos[i].cantidadRequerida)
                    return false;

                progresoRecoleccionActual[i]++;

                Debug.Log($"Recolectado {objeto.nombreObjeto} ({progresoRecoleccionActual[i]}/{faseRecoleccionActual.objetivos[i].cantidadRequerida})");

                ActualizarObjetivosEnUI();

                // 🔹 Guardamos progreso cada vez que cambia
                if (SistemaGuardado.instancia != null)
                    SistemaGuardado.instancia.GuardarDatos();

                // ¿Completó TODOS los objetivos?
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
    // UI DE OBJETIVOS
    // ------------------------------------------------------
    private void ActualizarObjetivosEnUI()
    {
        if (misionActual == null) return;

        FaseBase fase = misionActual.fases[indiceFaseActual];

        if (fase is FaseRecoleccion)
        {
            var fr = (FaseRecoleccion)fase;

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
        else
        {
            UI_MisionActiva.instancia.textoObjetivos.text = "";
        }
    }

    // ------------------------------------------------------
    // RECOLECCIÓN: ¿OBJETO PERTENECE A LA FASE?
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

    // ------------------------------------------------------
    // FUNCIONES PARA SISTEMA DE GUARDADO
    // ------------------------------------------------------
    public SerializableDictionary<string, int> ObtenerProgresoActualComoDiccionario()
    {
        var dict = new SerializableDictionary<string, int>();

        Debug.Log("Guardando progreso fase...");

        if (progresoRecoleccionActual == null)
        {
            Debug.Log("PROGRESO NULL. NO SE PUEDE GUARDAR.");
            return dict;
        }

        Debug.Log("PROGRESO EXISTE. GUARDANDO...");

        for (int i = 0; i < progresoRecoleccionActual.Length; i++)
        {
            Debug.Log("Objetivo " + i + ": " + progresoRecoleccionActual[i]);
            dict[i.ToString()] = progresoRecoleccionActual[i];
        }

        return dict;
    }

    public DatosDeMision BuscarMisionPorID(int id)
    {
        foreach (var m in todasLasMisiones)
        {
            if (m != null && m.idMision == id)
                return m;
        }
        return null;
    }

    public void CargarMisionDesdeDatos(
     DatosDeMision mision,
     int faseGuardada,
     SerializableDictionary<string, int> progresoGuardado)
    {
        misionActual = mision;

        indiceFaseActual = Mathf.Clamp(faseGuardada, 0, misionActual.fases.Length - 1);

        Debug.Log("Cargando misión desde datos: " + misionActual.nombreMision);

        FaseBase fase = misionActual.fases[indiceFaseActual];

        UI_MisionActiva.instancia.MostrarMision(misionActual, fase);

        // ===========================================
        //          NO RESETEAR NADA AQUÍ
        // ===========================================

        if (fase is FaseRecoleccion)
        {
            faseRecoleccionActual = (FaseRecoleccion)fase;
            progresoRecoleccionActual = new int[faseRecoleccionActual.objetivos.Length];

            foreach (var kv in progresoGuardado)
            {
                int index = int.Parse(kv.Key);
                if (index >= 0 && index < progresoRecoleccionActual.Length)
                    progresoRecoleccionActual[index] = kv.Value;
            }
        }
        else
        {
            progresoRecoleccionActual = null;
            faseRecoleccionActual = null;
        }

        // 🔹 SOLO actualizar UI
        ActualizarObjetivosEnUI();

        // ❌ NO interpretar fase (esto resetea todo)
        // InterpretarFaseActual();
    }
    public List<int> ObtenerProgresoActualComoLista()
    {
        var lista = new List<int>();

        if (progresoRecoleccionActual == null)
            return lista;

        for (int i = 0; i < progresoRecoleccionActual.Length; i++)
            lista.Add(progresoRecoleccionActual[i]);

        return lista;
    }

    public void CargarMisionDesdeDatos(
    DatosDeMision mision,
    int faseGuardada,
    List<int> progresoGuardado)
    {
        misionActual = mision;
        indiceFaseActual = Mathf.Clamp(faseGuardada, 0, mision.fases.Length - 1);

        FaseBase fase = mision.fases[indiceFaseActual];
        UI_MisionActiva.instancia.MostrarMision(misionActual, fase);

        if (fase is FaseRecoleccion fr)
        {
            faseRecoleccionActual = fr;
            progresoRecoleccionActual = new int[fr.objetivos.Length];

            for (int i = 0; i < progresoGuardado.Count && i < progresoRecoleccionActual.Length; i++)
                progresoRecoleccionActual[i] = progresoGuardado[i];
        }

        ActualizarObjetivosEnUI();
    }

    public void MarcarMisionComoCompletada(int id)
    {
        if (id >= 0 && id < misionesCompletadas.Length)
            misionesCompletadas[id] = true;
    }

    public void ForzarResetMisionActual()
    {
        misionActual = null;
        indiceFaseActual = 0;
        progresoRecoleccionActual = null;
        faseRecoleccionActual = null;
    }

    public void CargarMisionesCompletadas(int[] ids)
    {
        // Limpiar todo primero
        for (int i = 0; i < misionesCompletadas.Length; i++)
            misionesCompletadas[i] = false;

        // Marcar solo las que estaban en JSON
        foreach (int id in ids)
        {
            if (id >= 0 && id < misionesCompletadas.Length)
                misionesCompletadas[id] = true;
        }

        Debug.Log("Misiones completadas cargadas desde JSON.");
    }
}
