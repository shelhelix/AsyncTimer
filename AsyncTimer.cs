using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AsyncUtils {
	public class AsyncTimer {
		float _leftTime;

		CancellationTokenSource _tokenSource;
		
		public float LeftTime => _leftTime;

		public event Action<AsyncTimer> OnTimerTick;
		public event Action<AsyncTimer> OnTimerEnd;
		
		public void Start(float timeSec) {
			if ( _tokenSource != null ) {
				Stop();
			}
			_leftTime          = timeSec;
			_tokenSource       = new CancellationTokenSource();
			UniTask.Void(Tick);
		}

		public void Stop() {
			_tokenSource?.Cancel();
		}

		async UniTaskVoid Tick() {
			OnTimerTick?.Invoke(this);
			while ( _leftTime > 0 ) {
				try {
					await UniTask.Delay(1000, cancellationToken: _tokenSource.Token);
				} catch ( OperationCanceledException ) {
					// Ignore exception
				}
				if ( _tokenSource.IsCancellationRequested ) {
					_tokenSource = null;
					return;
				}
				_leftTime -= 1f;
				OnTimerTick?.Invoke(this);
			}
			Stop();
			OnTimerEnd?.Invoke(this);
		}
	}
}