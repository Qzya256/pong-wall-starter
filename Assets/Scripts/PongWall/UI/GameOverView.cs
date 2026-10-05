using DG.Tweening;
using UniRx;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PongWall
{
    public class GameOverView : MonoBehaviour, IInitializable
    {
        [SerializeField] private CanvasGroup _panel;
        [SerializeField] private TextMeshProUGUI _finalScoreLabel;
        [SerializeField] private Button _restartButton;

        private GameSession _gameSession;
        private GameRestarter _gameRestarter;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void Construct(GameSession gameSession, GameRestarter gameRestarter)
        {
            _gameSession = gameSession;
            _gameRestarter = gameRestarter;

            gameSession.State
                .Subscribe(OnStateChanged)
                .AddTo(_disposables);

            gameSession.Score
                .Subscribe(UpdateFinalScore)
                .AddTo(_disposables);
        }

        public void Initialize()
        {
            _finalScoreLabel.raycastTarget = false;
            _restartButton.onClick.AddListener(OnRestartClicked);
            HideImmediate();
        }

        private void OnDestroy()
        {
            _restartButton.onClick.RemoveListener(OnRestartClicked);
            _disposables.Dispose();
            _panel.DOKill();
        }

        private void OnValidate()
        {
            Debug.Assert(_panel != null, $"{nameof(_panel)} is not assigned.");
            Debug.Assert(_finalScoreLabel != null, $"{nameof(_finalScoreLabel)} is not assigned.");
            Debug.Assert(_restartButton != null, $"{nameof(_restartButton)} is not assigned.");
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
                Show(_gameSession.LastEndWasImmediate);
            else
                HideImmediate();
        }

        private void UpdateFinalScore(int score)
        {
            _finalScoreLabel.text = $"Score: {score}";
        }

        private void OnRestartClicked()
        {
            _gameRestarter.Restart();
        }

        private void Show(bool immediate)
        {
            _panel.DOKill();
            _panel.interactable = true;
            _panel.blocksRaycasts = true;
            _restartButton.interactable = true;

            if (immediate)
            {
                _panel.alpha = 1f;
                _panel.transform.localScale = Vector3.one;
                return;
            }

            _panel.alpha = 0f;
            _panel.transform.localScale = Vector3.one * 0.9f;
            _panel.DOFade(1f, 0.25f);
            _panel.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
        }

        private void HideImmediate()
        {
            _panel.DOKill();
            _panel.alpha = 0f;
            _panel.interactable = false;
            _panel.blocksRaycasts = false;
            _panel.transform.localScale = Vector3.one;
        }
    }
}
