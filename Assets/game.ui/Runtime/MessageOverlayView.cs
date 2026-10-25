namespace Game.UI.Runtime
{
    using R3;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class MessageOverlayView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text txtTitle;
        [SerializeField] private TMP_Text txtMessage;
        [SerializeField] private Button btnPrimary;
        [SerializeField] private TMP_Text txtPrimary;
        [SerializeField] private Button btnSecondary;
        [SerializeField] private TMP_Text txtSecondary;

        public Observable<Unit> OnPrimaryClicked => btnPrimary.OnClickAsObservable();
        public Observable<Unit> OnSecondaryClicked => btnSecondary.OnClickAsObservable();
        public bool IsVisible => canvasGroup.alpha > 0f;

        public void Show(
            string title,
            string message,
            string primaryAction,
            string secondaryAction = null)
        {
            txtTitle.text = title;
            txtMessage.text = message;
            ConfigureAction(btnPrimary, txtPrimary, primaryAction);
            ConfigureAction(btnSecondary, txtSecondary, secondaryAction);
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        private static void ConfigureAction(Button button, TMP_Text label, string action)
        {
            var hasAction = !string.IsNullOrWhiteSpace(action);
            button.gameObject.SetActive(hasAction);
            label.text = hasAction ? action : string.Empty;
        }

        private void SetVisible(bool visible)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }
}
