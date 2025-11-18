using TMPro;
using UnityEngine;

namespace Quests
{
    /// <summary>
    /// Enlaza un Canvas con el MissionRunner para mostrar:
    /// - Título de la línea / fase
    /// - Descripción
    /// - Progreso (fase X/Y · restantes/objetivo)
    /// </summary>
    public class MissionUIBinderTMP : MonoBehaviour
    {
        [Header("Refs")]
        public MissionRunner runner;

        [Header("UI")]
        public TMP_Text txtLineTitle;
        public TMP_Text txtPhaseTitle;
        public TMP_Text txtDesc;
        public TMP_Text txtProgress;

        void OnEnable()
        {
            if (runner != null) runner.OnSnapshot += HandleSnapshot;
        }

        void OnDisable()
        {
            if (runner != null) runner.OnSnapshot -= HandleSnapshot;
        }

        void HandleSnapshot(MissionSnapshot s)
        {
            if (!s.active)
            {
                if (txtLineTitle) txtLineTitle.text = "Sin misión activa";
                if (txtPhaseTitle) txtPhaseTitle.text = "";
                if (txtDesc) txtDesc.text = "";
                if (txtProgress) txtProgress.text = "";
                return;
            }

            if (txtLineTitle) txtLineTitle.text = s.lineName;
            if (txtPhaseTitle) txtPhaseTitle.text = $"{s.phaseTitle} · {s.phaseType}";
            if (txtDesc) txtDesc.text = s.phaseDesc;

            if (txtProgress)
            {
                string baseProg = $"Fase {s.phaseIndex1}/{s.phasesTotal}";
                if (s.target > 1) baseProg += $" · {s.target - s.remaining}/{s.target}";
                txtProgress.text = baseProg;
            }
        }
    }
}
