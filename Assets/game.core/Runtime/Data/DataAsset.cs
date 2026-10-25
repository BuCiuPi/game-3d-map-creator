namespace Game.Core.Runtime.Data
{
    using UnityEngine;

    /// <summary>
    /// Stores one strongly typed, serialized data payload.
    /// Loading and release lifetime belong to the caller, not to the data asset.
    /// </summary>
    /// <typeparam name="TData">Serializable payload type.</typeparam>
    public abstract class DataAsset<TData> : ScriptableObject
    {
        [SerializeField]
        private TData data;

        public TData Data => data;

#if UNITY_EDITOR
        public void EditorSetData(TData value)
        {
            data = value;
        }
#endif
    }
}
