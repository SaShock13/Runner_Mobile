using System;
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
            ProcessObstacle(other);
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
            Debug.Log($"ProcessCollectible  {bonus.bonusType}");
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
            default:
                break;
        }

        _soundManager.PlaySFX(Sounds.bonus);
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
