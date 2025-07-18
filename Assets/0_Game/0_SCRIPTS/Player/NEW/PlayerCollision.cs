using UnityEngine;
using Zenject;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private LayerMask _collectibleLayer;
    [SerializeField] private LayerMask _obstacleLayer;

    private PlayerStats _playerStats;
    private EventBus _eventBus;
    private SoundManager _soundManager;


    [Inject]
    public void Construct(PlayerStats playerStats, EventBus eventBus, SoundManager soundManager)
    {
        _playerStats = playerStats;
        _eventBus = eventBus;
        _soundManager = soundManager;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _collectibleLayer) != 0)
        {
            ProcessCollectible(other);
        }
        if (((1 << other.gameObject.layer) & _obstacleLayer) != 0)
        {
            if (!_playerStats.IsInvincible)
            {
                ProcessObstacle(other); 
            }
        }
    }

    private void ProcessObstacle(Collider other)
    {
        if(!_playerStats.IsAlive) return;
        _playerStats.TakeDamage(3);
    }

    private void ProcessCollectible(Collider other)
    {

        var bonus = other.GetComponentInParent<Bonus>();
        bonus.Collect();
            DebugUtils.LogEditor($"ProcessCollectible  {bonus.bonusType}");
        switch (bonus.bonusType)
        {
            case BonusType.Simple:
                ProcessSimpleBonus();
                break;
            case BonusType.Coin:
                ProcessCoinBonus();
                break;
            case BonusType.Diamand:
                ProcessDiamandBonus();
                break; 
            case BonusType.Invincibility:
                ProcessInvincibility(bonus.duration);
                break;
            case BonusType.MultiplyerX2:
                ProcessMultiplyerX2(bonus.duration);
                break;
            case BonusType.SpeedBoost:
                ProcessSpeedBoost(bonus.duration);
                break;
            default:
                break;
        }

        _soundManager.PlaySFX(Sounds.bonus);
    }

    private void ProcessSpeedBoost(float duration)
    {
        _eventBus.PublishOnSpeedBoostCollectedEvent(duration);
    }

    private void ProcessMultiplyerX2(float duration)
    {
        _eventBus.PublishOnMultiplyerX2CollectedEvent(duration);
    }

    private void ProcessInvincibility(float duration)
    {
        _eventBus.PublishOnInvincibilityCollectedEvent(duration);
    }

    private void ProcessDiamandBonus()
    {
        _eventBus.PublishOnDiamondCollectedEventt();
    }

    private void ProcessCoinBonus()
    {
        _eventBus.PublishOnCoinCollectedEvent();
    }

    private void ProcessSimpleBonus()
    {
        _eventBus.PublishOnBonusCollectedEvent();
    }
}
