using UnityEngine;
using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class GameScreen : BaseScreen
    {
        public VisualElement HudRegion => GetElement("hud-region");

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnEnterPlaying += Show;
                GameStateManager.Instance.OnEnterStart += Hide;
                GameStateManager.Instance.OnEnterGameOver += Hide;
            }
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnEnterPlaying -= Show;
                GameStateManager.Instance.OnEnterStart -= Hide;
                GameStateManager.Instance.OnEnterGameOver -= Hide;
            }
        }
    }
}
