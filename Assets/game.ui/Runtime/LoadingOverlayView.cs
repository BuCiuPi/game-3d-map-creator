namespace Game.UI.Runtime
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class LoadingOverlayView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text txtStatus;
        [SerializeField] private Image progressFill;

        public bool IsVisible => canvasGroup.alpha > 0f;

        public void Show(string status)
        {
            txtStatus.text = status;
            SetProgress(0f);
            SetVisible(true);
        }

        public void SetStatus(string status)
        {
            txtStatus.text = status;
        }

        public void SetProgress(float progress)
        {
            progressFill.fillAmount = Mathf.Clamp01(progress);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        private void SetVisible(bool visible)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }
}
