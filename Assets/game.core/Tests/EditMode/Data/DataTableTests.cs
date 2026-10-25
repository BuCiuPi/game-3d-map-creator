namespace Game.Core.Tests.EditMode.Data
{
    using System;
    using Game.Core.Runtime.Data;
    using NUnit.Framework;
    using UnityEngine;

    public sealed class DataTableTests
    {
        private TestDataTable table;

        [SetUp]
        public void SetUp()
        {
            table = ScriptableObject.CreateInstance<TestDataTable>();
            table.EditorSetTableName("test-table");
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(table);
        }

        [Test]
        public void TryGetRow_BuildsIndexAndReturnsTypedRow()
        {
            var first = CreateRow("first", 10);
            var second = CreateRow("second", 20);
            table.EditorSetRows(new[] { first, second });

            Assert.That(table.TryGetRow("second", out var row), Is.True);
            Assert.That(row, Is.SameAs(second));
            Assert.That(row.Value, Is.EqualTo(20));
        }

        [Test]
        public void TryGetRow_ReturnsFalseForUnknownOrEmptyId()
        {
            table.EditorSetRows(new[] { CreateRow("known", 1) });

            Assert.That(table.TryGetRow("missing", out _), Is.False);
            Assert.That(table.TryGetRow(string.Empty, out _), Is.False);
            Assert.That(table.TryGetRow(null, out _), Is.False);
        }

        [Test]
        public void ValidateOrThrow_RejectsDuplicateIds()
        {
            table.EditorSetRows(new[]
            {
                CreateRow("duplicate", 1),
                CreateRow("duplicate", 2),
            });

            var exception = Assert.Throws<InvalidOperationException>(() => table.ValidateOrThrow());
            StringAssert.Contains("Duplicate row ID 'duplicate'", exception.Message);
        }

        [Test]
        public void ValidateOrThrow_RejectsMissingIds()
        {
            table.EditorSetRows(new[] { CreateRow(" ", 1) });

            var exception = Assert.Throws<InvalidOperationException>(() => table.ValidateOrThrow());
            StringAssert.Contains("empty ID", exception.Message);
        }

        [Test]
        public void EditorSetRows_InvalidatesAnExistingIndex()
        {
            table.EditorSetRows(new[] { CreateRow("old", 1) });
            Assert.That(table.TryGetRow("old", out _), Is.True);

            table.EditorSetRows(new[] { CreateRow("new", 2) });

            Assert.That(table.TryGetRow("old", out _), Is.False);
            Assert.That(table.TryGetRow("new", out var row), Is.True);
            Assert.That(row.Value, Is.EqualTo(2));
        }

        private static TestDataTableRow CreateRow(string rowId, int value)
        {
            var row = new TestDataTableRow
            {
                Value = value,
            };
            row.EditorSetRowId(rowId);
            return row;
        }

        [Serializable]
        public sealed class TestDataTableRow : DataTableRow
        {
            public int Value;
        }

        public sealed class TestDataTable : DataTable<TestDataTableRow>
        {
        }
    }
}
