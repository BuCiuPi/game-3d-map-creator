namespace Game.UI.Runtime
{
    using System.Collections;
    using TMPro;
    using UnityEngine;

    public sealed class ToastView : MonoBehaviour
    {
        private const float DefaultDuration = 2.5f;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text txtMessage;

        private Coroutine hideRoutine;

        public bool IsVisible => canvasGroup.alpha > 0f;

        public void Show(string message, float duration = DefaultDuration)
        {
            txtMessage.text = message;
            SetVisible(true);

            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
            }

            hideRoutine = StartCoroutine(HideAfterDelay(duration));
        }

        public void Hide()
        {
            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
                hideRoutine = null;
            }

            SetVisible(false);
        }

        private IEnumerator HideAfterDelay(float duration)
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, duration));
            hideRoutine = null;
            SetVisible(false);
        }

        private void SetVisible(bool visible)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
