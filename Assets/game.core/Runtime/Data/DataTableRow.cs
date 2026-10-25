namespace Game.Core.Runtime.Data
{
    using System;
    using UnityEngine;

    [Serializable]
    public abstract class DataTableRow
    {
        [SerializeField]
        private string rowId;

        public string RowId => rowId;

#if UNITY_EDITOR
        public void EditorSetRowId(string value)
        {
            rowId = value;
        }
#endif
    }
}
