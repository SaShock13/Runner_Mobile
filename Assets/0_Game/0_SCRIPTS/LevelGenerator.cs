using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;




public class LevelGenerator : MonoBehaviour
{
    [Header("Настройки генерации")]
    [SerializeField] private float _generationDistance = 20f; // Дистанция до генерируемой стены
    [SerializeField] private float _deleteDistance = 30f;     // Дистанция удаления пройденных стен
    [SerializeField] private int _maxWalls = 5;               // Максимальное количество стен в памяти
    [SerializeField] private int _maxFloors = 5;               // Максимальное количество плит пола в памяти
    [SerializeField] private float wallsStep = 10f;             // шаг между стенами
    [SerializeField][Range(0, 1)] private float _betweenBonusChance = 0.9f; // Шанс бонуса между стенами

    [Header("Префабы")]
    [SerializeField] private GameObject _floorPrefab;
    [SerializeField] private GameObject[] _obstaclePrefabs;   // 2 - непроходимое

    [Header("Список лута с весами") ]
    public WeightedLoot[] weightedLootList; 

    private float _nextBonusZ = 5f; // Z-позиция для следующего бонуса
    private Transform _player;
    private List<GameObject> _activeWalls = new List<GameObject>();
    private List<GameObject> _activeFloors = new List<GameObject>();
    private float _nextWallZPos = 0f;
    private float _nextFloorZPos = 0f;
    private float floorStep = 10f; // шаг между плитами пола
    private float bonusToWallMinDistance = 1f;
    private Color mainColor;
    private FirebaseRemoteConfigManager _remoteConfig;
    private bool isStarted = false;
    private bool isNeedToGenerate = false;
    private EventBus _bus;

    [Inject]
    public void Construct(Player player, FirebaseRemoteConfigManager remoteConfig , EventBus bus)
    {
        _player = player.transform;
        _remoteConfig = remoteConfig;
        _bus = bus;
        _bus.OnTutorialFinishedEvent += () => isNeedToGenerate = true;
    }   

    public void StartGeneration()
    {
        for (int i = 0; i < 3; i++)
        {
            GenerateFloor(_nextFloorZPos);
            _nextFloorZPos += floorStep;
        }


        Debug.Log($"mainColor {_remoteConfig.GetColorValue("MainColor")}");
        mainColor = _remoteConfig.GetColorValue("MainColor");
        //var colorString = _remoteConfig.GetStringValue("MainColor");
        //if (colorString == "White") { mainColor = Color.white; }
        //if (colorString == "Green") { mainColor = Color.green; }
        

        isStarted = true;
    }

    public void IncreaseWallsStep()
    {
        wallsStep += 5;
    }

    private void Update()
    {
        if (isStarted)
        {
            // Генерация стен при приближении игрока
            if (_player.position.z + _generationDistance > _nextWallZPos)
            {
                if (isNeedToGenerate)
                {
                    GenerateWall(); 
                }
                _nextWallZPos += wallsStep;
            }

            if (_player.position.z + _generationDistance > _nextFloorZPos)
            {
                GenerateFloor(_nextFloorZPos);
                _nextFloorZPos += floorStep;
            }
            _maxFloors = (int)(_generationDistance / floorStep) + 1;

            // Удаление пройденных стен
            if (_activeWalls.Count > _maxWalls)
            {
                Destroy(_activeWalls[0]);
                _activeWalls.RemoveAt(0);
            }

            // Удаление пройденных плит пола
            if (_activeFloors.Count > _maxFloors)
            {
                Destroy(_activeFloors[0]);
                _activeFloors.RemoveAt(0);
            }        
        }
    }   

    private GameObject GetRandomObstacle(bool forcePassable = false)
    {
        if (forcePassable)
        {
            return _obstaclePrefabs[Random.Range(0, 2)];
        }
        else
        {
            float chance = Random.value;
            if (chance < 0.5f) return _obstaclePrefabs[Random.Range(0, 2)];
            else return _obstaclePrefabs[2]; // Непроходимое
        }
    }

    private void GenerateWall()
    {
        GameObject wall = new GameObject("Wall_" + _nextWallZPos);
        wall.transform.position = new Vector3(0, 0, _nextWallZPos);
        _activeWalls.Add(wall);
        int obstacleCount = Random.Range(3, 6); // количество препятствий в стене
        bool hasPassableObstacle = false;

        hasPassableObstacle = false;
        for (int lane = -2; lane <= 2; lane++)
        {
            if (obstacleCount > 0 && (Random.value > 0.5f || !hasPassableObstacle))
            {
                // Если это последний шанс добавить проходимое препятствие, forcePassable = true
                bool isLastChance = (lane == 2 && !hasPassableObstacle);
                GameObject obstaclePrefab = GetRandomObstacle(isLastChance);

                obstaclePrefab.GetComponentInChildren<Renderer>().sharedMaterial.color = mainColor;
                InstantiateObstacle(obstaclePrefab, wall.transform, lane);
                obstacleCount--;

                if (obstaclePrefab != _obstaclePrefabs[2]) // Если препятствие проходимое
                    hasPassableObstacle = true;
            }
        }

        if ( _nextWallZPos - _nextBonusZ >= wallsStep / 2f)
        {
            SpawnBetweenBonuses();
        }
    }

    private void InstantiateObstacle(GameObject prefab, Transform parent, int lane)
    {
        Vector3 pos = new Vector3(lane * 2f, 0, parent.position.z);
        Instantiate(prefab, pos, Quaternion.identity, parent);
    }

    private void GenerateFloor(float zPos)
    {
        var floorTile = Instantiate(_floorPrefab, new Vector3(0, 0, zPos), Quaternion.identity);
        _activeFloors.Add(floorTile);
    }

    private void SpawnBetweenBonuses()
    {
        var rnd = Random.Range(0, 4);
        List<int> busyLanes  = new List<int>();
        for (int i = 0; i < rnd; i++)
        {
            float bonusZ = Random.Range(
                _nextWallZPos - wallsStep + bonusToWallMinDistance,
                _nextWallZPos - bonusToWallMinDistance
            );

            int randomLane;
            do
            {
                randomLane = Random.Range(-2, 3);
            }
            while (busyLanes.Contains(randomLane));

            busyLanes.Add(randomLane);
            Vector3 bonusPos = new Vector3(randomLane * 2f, 0, bonusZ);
            InstantiateRandomBonus(bonusPos); 
        }
        _nextBonusZ = _nextWallZPos;
    }

    private void InstantiateRandomBonus(Vector3 position)
    {
        var items = weightedLootList.Select(x => (x.bonusPrefab, x.weight)).ToArray();
        GameObject randomBonus = MyUtilities.RandomUtilities.WeightedChoice(items);        
        GameObject bonus = Instantiate(randomBonus,
            position,
            Quaternion.identity
        );
    }
}
