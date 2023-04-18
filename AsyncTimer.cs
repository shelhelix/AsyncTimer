using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AsyncUtils {
    public class AsyncTimer {
        float _leftTime;
        float _tickTimeSec;

        CancellationTokenSource _tokenSource;
		
        public float LeftTime => _leftTime;

        public event Action OnTimerTick;
        public event Action OnTimerEnd;
		
		
        public void Start(float timeSec, float tickTime = 1f) {
            if ( _tokenSource != null ) {
                throw new InvalidOperationException("Timer is already started");
            }
            _leftTime          = timeSec;
            _tokenSource       = new CancellationTokenSource();
            _tickTimeSec       = tickTime;
			
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
                    var msTickTime = (int)(_tickTimeSec * 1000);
                    await UniTask.Delay(msTickTime, cancellationToken: token);
                } catch ( OperationCanceledException ) {
                    return;
                }
                _leftTime -= _tickTimeSec;
                OnTimerTick?.Invoke();
            }
            Stop();
            OnTimerEnd?.Invoke();
        }
    }
}