using UnityEngine;
using Zenject;

public class PlayerHealth : MonoBehaviour
{
    private EventBus _eventBus;
    private SoundManager _soundManager;

    [Inject]
    public void Construct(EventBus bus, SoundManager soundManager)
    {
        _soundManager = soundManager;
        _eventBus = bus;
    }

    private void OnEnable()
    {
        _eventBus.OnPlayerDamagedEvent += TakeDamage;
        _eventBus.OnPlayerDeathEvent += TakeDeath;
    }

    private void OnDisable()
    {
        _eventBus.OnPlayerDamagedEvent -= TakeDamage;
        _eventBus.OnPlayerDeathEvent -= TakeDeath;
    }

    public void TakeDeath()
    {
        _soundManager.PlaySFX(Sounds.death);
    }

    public void TakeDamage(int arg1, int arg2)
    {
        _soundManager.PlaySFX(Sounds.hit);
    }
}
