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
        _playerStats.TakeDamage(1);
        _soundManager.PlayObstacleCollision();
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

        
    }

    private void ProcessSpeedBoost(float duration)
    {
        _eventBus.PublishOnSpeedBoostCollectedEvent(duration);
        _soundManager.PlaySFX(Sounds.speedBoost);
    }

    private void ProcessMultiplyerX2(float duration)
    {
        _eventBus.PublishOnMultiplyerX2CollectedEvent(duration);
        _soundManager.PlaySFX(Sounds.x2);
    }

    private void ProcessInvincibility(float duration)
    {
        _eventBus.PublishOnInvincibilityCollectedEvent(duration);
        _soundManager.PlaySFX(Sounds.invincibility);
    }

    private void ProcessDiamandBonus()
    {
        _eventBus.PublishOnDiamondCollectedEventt();
        _soundManager.PlaySFX(Sounds.diamond);
    }

    private void ProcessCoinBonus()
    {
        _eventBus.PublishOnCoinCollectedEvent();
        _soundManager.PlaySFX(Sounds.coin);
    }

    private void ProcessSimpleBonus()
    {
        _eventBus.PublishOnBonusCollectedEvent();
        _soundManager.PlaySFX(Sounds.bonus);
    }
}
