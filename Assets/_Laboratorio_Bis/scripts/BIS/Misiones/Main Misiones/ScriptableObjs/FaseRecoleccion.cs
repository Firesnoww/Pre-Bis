using UnityEngine;

[CreateAssetMenu(fileName = "FaseRecoleccion", menuName = "Misiones/Fases/Recoleccion")]
public class FaseRecoleccion : FaseBase
{
    [System.Serializable]
    public class ObjetivoRecoleccion
    {
        public ObjetoRecoleccion objeto;
        public int cantidadRequerida;
    }

    public ObjetivoRecoleccion[] objetivos;

}
