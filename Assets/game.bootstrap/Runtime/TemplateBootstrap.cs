namespace Game.Bootstrap.Runtime
{
    using System;
    using System.Collections;
    using Game.UI.Runtime;
    using R3;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.SceneManagement;

    public sealed class TemplateBootstrap : MonoBehaviour
    {
        [SerializeField] private LoadingOverlayView loadingOverlay;
        [SerializeField] private MessageOverlayView recoveryOverlay;
        [SerializeField] private AssetReference globalUiShell;
        [SerializeField] private Transform shellRoot;

        private AsyncOperationHandle<GameObject> shellHandle;
        private bool ownsShellHandle;

        public static TemplateBootState State { get; private set; } = TemplateBootState.NotStarted;
        public static string LastError { get; private set; } = string.Empty;
        public static GlobalUiShellView Shell { get; private set; }

        private void Awake()
        {
            State = TemplateBootState.NotStarted;
            LastError = string.Empty;
            Shell = null;
            recoveryOverlay.Hide();
            recoveryOverlay.OnPrimaryClicked
                .Subscribe(_ => ReloadBootScene())
                .AddTo(this);
        }

        private IEnumerator Start()
        {
            State = TemplateBootState.LoadingShell;
            loadingOverlay.Show("Preparing template shell...");

            try
            {
                shellHandle = globalUiShell.InstantiateAsync(shellRoot);
                ownsShellHandle = true;
            }
            catch (Exception exception)
            {
                Fail(exception.Message);
                yield break;
            }

            while (!shellHandle.IsDone)
            {
                loadingOverlay.SetProgress(shellHandle.PercentComplete);
                yield return null;
            }

            if (shellHandle.Status != AsyncOperationStatus.Succeeded || shellHandle.Result == null)
            {
                Fail(shellHandle.OperationException?.Message ?? "The global UI shell could not be loaded.");
                yield break;
            }

            Shell = shellHandle.Result.GetComponent<GlobalUiShellView>();
            if (Shell == null)
            {
                Fail("The global UI shell prefab is missing GlobalUiShellView.");
                yield break;
            }

            loadingOverlay.SetProgress(1f);
            loadingOverlay.Hide();
            State = TemplateBootState.Ready;
        }

        private void OnDestroy()
        {
            Shell = null;

            if (!ownsShellHandle || !shellHandle.IsValid())
            {
                return;
            }

            if (shellHandle.Status == AsyncOperationStatus.Succeeded && shellHandle.Result != null)
            {
                Addressables.ReleaseInstance(shellHandle);
            }
            else
            {
                Addressables.Release(shellHandle);
            }

            ownsShellHandle = false;
        }

        private void Fail(string error)
        {
            LastError = error;
            State = TemplateBootState.Failed;
            loadingOverlay.Hide();
            recoveryOverlay.Show(
                "Unable to start",
                error,
                "Retry");
        }

        private static void ReloadBootScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
