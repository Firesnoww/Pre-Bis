using UnityEngine;

[CreateAssetMenu(fileName = "NuevaMision", menuName = "Misiones/Mision")]
public class DatosDeMision : ScriptableObject
{
    [Header("Info general")]
    public int idMision;
    public string nombreMision;
    [TextArea] public string descripcionMision;

    [Header("Fases en orden")]
    public FaseBase[] fases;
}
