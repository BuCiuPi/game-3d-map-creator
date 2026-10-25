namespace Game.Bootstrap.Tests.PlayMode
{
    using System.Collections;
    using Game.Bootstrap.Runtime;
    using Game.UI.Runtime;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.TestTools;

    public sealed class TemplateBootPlayModeTests
    {
        [UnityTest]
        public IEnumerator LoadingScene_BootsNeutralShell()
        {
            var loadOperation = SceneManager.LoadSceneAsync("00_LoadingScene", LoadSceneMode.Single);
            Assert.That(loadOperation, Is.Not.Null);
            yield return loadOperation;

            var timeout = Time.realtimeSinceStartup + 15f;
            while ((TemplateBootstrap.State is TemplateBootState.NotStarted or TemplateBootState.LoadingShell) &&
                   Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(TemplateBootstrap.State, Is.EqualTo(TemplateBootState.Ready), TemplateBootstrap.LastError);
            Assert.That(Object.FindFirstObjectByType<GlobalUiShellView>(), Is.Not.Null);
        }
    }
}
