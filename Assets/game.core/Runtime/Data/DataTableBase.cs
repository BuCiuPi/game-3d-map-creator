namespace Game.Core.Runtime.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class DataTableBase : ScriptableObject
    {
        public abstract string TableName { get; }

        public abstract IReadOnlyList<string> GetValidationErrors();

        public void ValidateOrThrow()
        {
            var errors = GetValidationErrors();
            if (errors.Count == 0)
            {
                return;
            }

            throw new InvalidOperationException(
                $"Data table '{name}' is invalid:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
        }
    }
}
