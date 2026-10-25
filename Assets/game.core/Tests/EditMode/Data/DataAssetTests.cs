namespace Game.Core.Tests.EditMode.Data
{
    using System;
    using Game.Core.Runtime.Data;
    using NUnit.Framework;
    using UnityEngine;

    public sealed class DataAssetTests
    {
        [Test]
        public void Data_ReturnsTheTypedPayload()
        {
            var asset = ScriptableObject.CreateInstance<TestDataAsset>();
            var payload = new TestData
            {
                Value = 42,
            };

            asset.EditorSetData(payload);

            Assert.That(asset.Data, Is.SameAs(payload));
            Assert.That(asset.Data.Value, Is.EqualTo(42));

            UnityEngine.Object.DestroyImmediate(asset);
        }

        [Serializable]
        public sealed class TestData
        {
            public int Value;
        }

        public sealed class TestDataAsset : DataAsset<TestData>
        {
        }
    }
}
