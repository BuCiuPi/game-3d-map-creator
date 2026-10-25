namespace Game.Core.Runtime.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Stores homogeneous rows and builds an immutable runtime index on first lookup.
    /// </summary>
    /// <typeparam name="TRow">Serializable row type with a stable ID.</typeparam>
    public abstract class DataTable<TRow> : DataTableBase
        where TRow : DataTableRow
    {
        [SerializeField]
        private string tableName;

        [SerializeField]
        private TRow[] rows = Array.Empty<TRow>();

        [NonSerialized]
        private Dictionary<string, TRow> rowsById;

        public override string TableName => tableName;

        public IReadOnlyList<TRow> Rows => rows ?? Array.Empty<TRow>();

        public bool TryGetRow(string rowId, out TRow row)
        {
            row = null;
            if (string.IsNullOrWhiteSpace(rowId))
            {
                return false;
            }

            EnsureIndex();
            return rowsById.TryGetValue(rowId, out row);
        }

        public TRow GetRequiredRow(string rowId)
        {
            if (TryGetRow(rowId, out var row))
            {
                return row;
            }

            throw new KeyNotFoundException(
                $"Row '{rowId}' does not exist in data table '{name}'.");
        }

        public override IReadOnlyList<string> GetValidationErrors()
        {
            var source = rows ?? Array.Empty<TRow>();
            var errors = new List<string>();
            var seenIds = new HashSet<string>(StringComparer.Ordinal);

            for (var index = 0; index < source.Length; index++)
            {
                var row = source[index];
                if (row == null)
                {
                    errors.Add($"Row at index {index} is null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(row.RowId))
                {
                    errors.Add($"Row at index {index} has an empty ID.");
                    continue;
                }

                if (!seenIds.Add(row.RowId))
                {
                    errors.Add($"Duplicate row ID '{row.RowId}'.");
                }
            }

            return errors;
        }

        private void EnsureIndex()
        {
            if (rowsById != null)
            {
                return;
            }

            ValidateOrThrow();

            var source = rows ?? Array.Empty<TRow>();
            rowsById = new Dictionary<string, TRow>(source.Length, StringComparer.Ordinal);
            foreach (var row in source)
            {
                rowsById.Add(row.RowId, row);
            }
        }

        private void OnEnable()
        {
            rowsById = null;
        }

#if UNITY_EDITOR
        public void EditorSetTableName(string value)
        {
            tableName = value;
        }

        public void EditorSetRows(TRow[] value)
        {
            rows = value ?? Array.Empty<TRow>();
            rowsById = null;
        }

        private void OnValidate()
        {
            rowsById = null;
        }
#endif
    }
}
