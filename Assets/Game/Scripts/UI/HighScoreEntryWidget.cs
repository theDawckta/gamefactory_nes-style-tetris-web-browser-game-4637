using UnityEngine;
using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class HighScoreEntryWidget : MonoBehaviour
    {
        private static readonly char[] CharSequence = BuildCharSequence();

        private Label[] _slotLabels = new Label[3];
        private VisualElement[] _cursors = new VisualElement[3];
        private Label _confirmLabel;

        private int[] _charIndices = new int[3];
        private int _activeSlot;
        private bool _confirmFocused;
        private int _score;

        private float _blinkTimer;
        private bool _cursorVisible = true;

        private InputHandler _inputHandler;

        public bool IsActive { get; private set; }

        public char GetSlotChar(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= 3) return ' ';
            return CharSequence[_charIndices[slotIndex]];
        }

        private static char[] BuildCharSequence()
        {
            var chars = new char[37]; // space, A-Z, 0-9
            chars[0] = ' ';
            for (int i = 0; i < 26; i++) chars[1 + i] = (char)('A' + i);
            for (int i = 0; i < 10; i++) chars[27 + i] = (char)('0' + i);
            return chars;
        }

        private void Start()
        {
            var doc = GetComponent<UIDocument>();
            if (doc != null && doc.rootVisualElement != null)
            {
                var root = doc.rootVisualElement;
                for (int i = 0; i < 3; i++)
                {
                    _slotLabels[i] = root.Q<Label>("slot-char-" + i);
                    _cursors[i] = root.Q<VisualElement>("cursor-" + i);
                }
                _confirmLabel = root.Q<Label>("confirm-label");
            }

            _inputHandler = Object.FindFirstObjectByType<InputHandler>();
            if (_inputHandler != null)
                _inputHandler.OnActionDown += OnActionDown;
        }

        private void OnDestroy()
        {
            if (_inputHandler != null)
                _inputHandler.OnActionDown -= OnActionDown;
        }

        public void Activate(int score)
        {
            _score = score;
            IsActive = true;
            _activeSlot = 0;
            _confirmFocused = false;
            _blinkTimer = 0f;
            _cursorVisible = true;
            for (int i = 0; i < 3; i++) _charIndices[i] = 0;

            if (_confirmLabel != null) _confirmLabel.style.display = DisplayStyle.None;
            UpdateDisplay();
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private void Update()
        {
            if (!IsActive) return;

            _blinkTimer += Time.deltaTime;
            if (_blinkTimer >= 0.5f)
            {
                _blinkTimer -= 0.5f;
                _cursorVisible = !_cursorVisible;
                UpdateCursorDisplay();
            }
        }

        private void OnActionDown(string actionName)
        {
            if (!IsActive) return;

            if (_confirmFocused)
            {
                if (actionName == "NavDown") ConfirmEntry();
                return;
            }

            switch (actionName)
            {
                case "NavLeft":
                    _charIndices[_activeSlot] = (_charIndices[_activeSlot] + 36) % 37;
                    UpdateDisplay();
                    break;
                case "NavRight":
                    _charIndices[_activeSlot] = (_charIndices[_activeSlot] + 1) % 37;
                    UpdateDisplay();
                    break;
                case "NavUp":
                    _charIndices[_activeSlot] = 0;
                    UpdateDisplay();
                    break;
                case "NavDown":
                    AdvanceSlot();
                    break;
            }
        }

        private void AdvanceSlot()
        {
            if (_activeSlot < 2)
            {
                _activeSlot++;
                _blinkTimer = 0f;
                _cursorVisible = true;
                UpdateDisplay();
            }
            else
            {
                _confirmFocused = true;
                if (_confirmLabel != null) _confirmLabel.style.display = DisplayStyle.Flex;
                UpdateCursorDisplay();
            }
        }

        private void ConfirmEntry()
        {
            IsActive = false;
            string initials = GetSlotChar(0).ToString() + GetSlotChar(1).ToString() + GetSlotChar(2).ToString();
            if (LeaderboardService.Instance != null)
                LeaderboardService.Instance.PostScore(
                    initials,
                    _score,
                    _ => GameStateManager.Instance?.GoToStart(),
                    err =>
                    {
                        Debug.LogWarning("HighScoreEntryWidget: PostScore failed: " + err);
                        GameStateManager.Instance?.GoToStart();
                    });
            else
                GameStateManager.Instance?.GoToStart();
        }

        private void UpdateDisplay()
        {
            for (int i = 0; i < 3; i++)
            {
                if (_slotLabels[i] != null)
                {
                    char c = CharSequence[_charIndices[i]];
                    _slotLabels[i].text = c == ' ' ? "" : c.ToString();
                }
            }
            UpdateCursorDisplay();
        }

        private void UpdateCursorDisplay()
        {
            for (int i = 0; i < 3; i++)
            {
                if (_cursors[i] == null) continue;
                bool show = !_confirmFocused && i == _activeSlot && _cursorVisible;
                _cursors[i].style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }
}
