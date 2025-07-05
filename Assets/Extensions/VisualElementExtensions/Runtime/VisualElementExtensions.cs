using UnityEngine.UIElements;

namespace OMG.Extensions.VisualElementExtensions.Runtime
{
    public static class ____/*VisualElementExtensions*/
    {
        public static void Display(this VisualElement visualElement, bool shouldDisplay) {
            if (shouldDisplay)
                visualElement.Display();
            else
                visualElement.DisplayNot();
        }

        public static void Display(this VisualElement visualElement) {
            visualElement.style.display = DisplayStyle.Flex;
            visualElement.MarkDirtyRepaint();
        }

        public static void DisplayNot(this VisualElement visualElement) {
            visualElement.style.display = DisplayStyle.None;
            visualElement.MarkDirtyRepaint();
        }
    }
}