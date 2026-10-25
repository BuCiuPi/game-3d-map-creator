namespace Game.UI.Runtime
{
    using UnityEngine;

    public sealed class GlobalUiShellView : MonoBehaviour
    {
        [SerializeField] private ToastView toast;
        [SerializeField] private MessageOverlayView messageOverlay;

        public ToastView Toast => toast;
        public MessageOverlayView MessageOverlay => messageOverlay;
    }
}
