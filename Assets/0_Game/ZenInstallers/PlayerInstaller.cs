using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Transform _spawnTransform;


    public override void InstallBindings()
    {
        Container.Bind<PlayerAnimatorManager>().AsSingle().NonLazy();
        Container.Bind<PlayerStats>().FromNew().AsSingle().NonLazy();

        var playerInstance = Container.InstantiatePrefabForComponent<Player>(_playerPrefab, _spawnTransform.position, Quaternion.identity, null);
        Container.Bind<Player>().FromInstance(playerInstance).AsSingle().NonLazy();
        Container.QueueForInject(playerInstance);
    }
}