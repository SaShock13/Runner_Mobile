using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] Joystick joystick;
    [SerializeField] SoundManager soundManager;
    [SerializeField] LevelGenerator levelGenerator;
    [SerializeField] Tutorial tutorial;
    [SerializeField] FirebaseRemoteConfigManager remoteConfig;
    [SerializeField] DistanceMeasurer distanceMeasurer;

    


    public override void InstallBindings()
    {
        Container.Bind<FirebaseRemoteConfigManager>().FromInstance(remoteConfig).AsSingle().NonLazy();
        Container.Bind<Tutorial>().FromInstance(tutorial).AsSingle();
        Container.Bind<Joystick>().FromInstance(joystick).AsSingle().NonLazy();
        Container.Bind<SoundManager>().FromInstance(soundManager).AsSingle().NonLazy();
        Container.Bind<LevelGenerator>().FromInstance(levelGenerator).AsSingle().NonLazy();
        Container.Bind<DistanceMeasurer>().FromInstance(distanceMeasurer).AsSingle();
        Container.Bind<IAsssetProvider>().To<AddressablesAssetProvider>().AsSingle();
        Container.Bind<IDataService>().To<DataProvider>().AsSingle();
        Container.Bind<IDataStorage>().To<PlayerPrefsStorage>().AsSingle();
        Container.Bind<PlayerProgress>().AsSingle().NonLazy();
        Container.Bind<PlayerWallet>().AsSingle().NonLazy();
        Container.Bind<EventBus>().AsSingle();
    }
}