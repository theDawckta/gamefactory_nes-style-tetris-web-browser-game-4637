using UnityEngine;
using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class BaseScreen : MonoBehaviour
    {
        private UIDocument _document;

        protected VisualElement DocumentRoot => _document?.rootVisualElement;

        protected virtual void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            if (_document != null && _document.rootVisualElement != null)
                _document.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public virtual void Hide()
        {
            if (_document != null && _document.rootVisualElement != null)
                _document.rootVisualElement.style.display = DisplayStyle.None;
            gameObject.SetActive(false);
        }

        public VisualElement GetElement(string name)
        {
            if (_document?.rootVisualElement == null)
                return null;
            return _document.rootVisualElement.Q<VisualElement>(name);
        }
    }
}
