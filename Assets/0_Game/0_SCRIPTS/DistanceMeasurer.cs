using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class DistanceMeasurer : MonoBehaviour
{
    private CancellationTokenSource _cts;
    private bool isStarted = false;
    private float measureInterval = 0.1f;
    Player _player;
    PlayerStats _playerStats;   // todo Испоользовать Статы или нет
    EventBus _eventBus;
    private float measuredDistance;
    private float startPlayerZPosition;



    [Inject]
    public void Construct(Player player, PlayerStats playerStats,EventBus eventBus)
    {
        _player = player;
        _playerStats = playerStats;
        _eventBus = eventBus;
    }       

    public void StartMeasure()
    {
        if (isStarted) return;

        startPlayerZPosition = _player.transform.position.z;

        Debug.Log($"startPlayerZPosition {startPlayerZPosition}");
        isStarted = true;
        _cts = new CancellationTokenSource();

        MeasureDistance(_cts.Token).Forget();
    }

    public void StopMeasure()
    {
        Measure();
        isStarted = false;
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async UniTaskVoid MeasureDistance(CancellationToken ct)
    {
        while (isStarted && !ct.IsCancellationRequested)
        {
            Measure();

            await UniTask.Delay((int)(measureInterval * 1000), cancellationToken: ct);
        }
    }

    private void Measure()
    {
        measuredDistance = _player.transform.position.z - startPlayerZPosition;
        _eventBus.PublishOnDistanceChangedEvent(measuredDistance);
    }

    public float GetDistance()
    {
        return measuredDistance;
    }

    private void OnDestroy()
    {
        StopMeasure();
    }
}
