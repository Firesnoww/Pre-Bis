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

    public SerializableDictionary<string, int> progresoFase =
        new SerializableDictionary<string, int>();

    public int[] misionesCompletadas = new int[0];
}
#endregion


#region --- OBJETOS ---
[Serializable]
public class ObjetosData
{
    public SerializableDictionary<string, ObjetoGuardado> objetos =
        new SerializableDictionary<string, ObjetoGuardado>();
}

[Serializable]
public class ObjetoGuardado
{
    public bool descubierto = false;
    public bool infoDesbloqueada = false;
    public int cantidad = 0;
}
#endregion


#region --- ANALISIS ---
[Serializable]
public class AnalisisData
{
    public SerializableDictionary<string, AnalisisGuardado> analisis =
        new SerializableDictionary<string, AnalisisGuardado>();
}

[Serializable]
public class AnalisisGuardado
{
    public bool encontrado = false;
    public bool analizado = false;
    public string descripcion = "";
}
#endregion


#region --- CAPTURA ---
[Serializable]
public class CapturaData
{
    public SerializableDictionary<string, CapturaGuardado> libelulas =
        new SerializableDictionary<string, CapturaGuardado>();
}

[Serializable]
public class CapturaGuardado
{
    public bool capturada = false;
    public bool analizada = false;
}
#endregion


#region --- SERIALIZABLE DICTIONARY ---
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue> { }
#endregion
