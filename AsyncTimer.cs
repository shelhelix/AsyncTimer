using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AsyncUtils {
    public class AsyncTimer {
        float _leftTime;

        CancellationTokenSource _tokenSource;
		
        public float LeftTime => _leftTime;

        public event Action OnTimerTick;
        public event Action OnTimerEnd;
		
		
        public void Start(float timeSec) {
            if ( _tokenSource != null ) {
                throw new InvalidOperationException("Timer is already started");
            }
            _leftTime          = timeSec;
            _tokenSource       = new CancellationTokenSource();
			
            Tick(_tokenSource.Token).Forget();
        }

        public void Stop() {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = null;
        }

        async UniTaskVoid Tick(CancellationToken token) {
            OnTimerTick?.Invoke();
            while ( _leftTime > 0 ) {
                try {
                    await UniTask.Delay(1000, cancellationToken: token);
                } catch ( OperationCanceledException ) {
                    return;
                }
                _leftTime -= 1f;
                OnTimerTick?.Invoke();
            }
            Stop();
            OnTimerEnd?.Invoke();
        }
    }
}