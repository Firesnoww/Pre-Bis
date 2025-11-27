using UnityEngine;

[CreateAssetMenu(fileName = "FaseCaptura", menuName = "Misiones/Fases/Captura")]
public class FaseCaptura : FaseBase
{
    [System.Serializable]
    public class ObjetivoCaptura
    {
        public ObjetoLibelula libelula;
        public int cantidadRequerida;
    }

    public ObjetivoCaptura[] objetivos;
}
