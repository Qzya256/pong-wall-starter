using UniRx;
using TMPro;
using UnityEngine;
using Zenject;

namespace PongWall
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreLabel;

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void Construct(GameSession gameSession)
        {
            gameSession.Score
                .Subscribe(UpdateScore)
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void OnValidate()
        {
            Debug.Assert(_scoreLabel != null, $"{nameof(_scoreLabel)} is not assigned.");
        }

        private void UpdateScore(int score)
        {
            _scoreLabel.text = score.ToString();
        }
    }
}
