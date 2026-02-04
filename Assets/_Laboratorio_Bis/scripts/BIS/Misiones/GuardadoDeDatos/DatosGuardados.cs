using System;
using System.Collections.Generic;

[Serializable]
public class DatosGuardados
{
    public MisionesData misiones = new MisionesData();
    public ObjetosData objetos = new ObjetosData();
    public AnalisisData analisis = new AnalisisData();
    public CapturaData captura = new CapturaData();
}


#region --- MISIONES ---
[Serializable]
public class MisionesData
{
    public int misionActivaID = -1;
    public int faseActiva = 0;

    // Antes era un diccionario ❌
    // AHORA es una lista SERIALIZABLE de enteros ✔️
    public List<int> progresoFase = new List<int>();

    public List<int> misionesCompletadas = new List<int>();
}

#endregion


#region --- OBJETOS ---
[Serializable]
public class ObjetosData
{
    public List<ObjetoGuardado> objetos = new List<ObjetoGuardado>();
}

[Serializable]
public class ObjetoGuardado
{
    public string idObjeto = "";
    public bool descubierto = false;
    public bool infoDesbloqueada = false;
    public int cantidad = 0;
}

#endregion


#region --- ANALISIS ---
[Serializable]
public class AnalisisData
{
    public List<AnalisisGuardado> analisis = new List<AnalisisGuardado>();
}

[Serializable]
public class AnalisisGuardado
{
    public string idObjeto = "";
    public bool encontrado = false;
    public bool analizado = false;
    public string descripcion = "";
}

#endregion


#region --- CAPTURA ---
[Serializable]
public class CapturaData
{
    public List<CapturaGuardado> libelulas = new List<CapturaGuardado>();
}

[Serializable]
public class CapturaGuardado
{
    public string idLibelula = "";
    public bool capturada = false;
    public bool analizada = false;
}

#endregion


#region --- SERIALIZABLE DICTIONARY ---
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue> { }
#endregion
