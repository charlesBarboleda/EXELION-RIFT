using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    [SerializeField] List<Level> _levels = new List<Level>();
    [SerializeField] int _currentLevelIndex = 1;
    SpawnerManager _spawnerManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        _spawnerManager = GetComponent<SpawnerManager>();
        _currentLevelIndex = 1;

    }
    void Start()
    {
        _levels.Add(GenerateNextLevel());
        Debug.Log($"Levels added in Start. Levels count: {_levels.Count}");

    }


    Level GenerateNextLevel()
    {
        LevelType levelType = GetRandomLevelType();
        switch (levelType)
        {
            case LevelType.Invasion: return CreateSoloInvasionLevel();
            case LevelType.Boss: return Random.value < 0.5f ? CreateSoloSpawnerBossLevel(_spawnerManager.GetSpawnerBossName()) : CreateSoloShooterBossLevel(_spawnerManager.GetShooterBossName());
            case LevelType.MultiPhaseBoss: return CreateMultiPhaseBossLevel();
            case LevelType.Comet: return CreateCometLevel();
            default: return CreateHordeLevel();
        }
    }

    LevelType GetRandomLevelType()
    {
        float randomValue = Random.value;

        if (_currentLevelIndex > 10)
        {

            // There's a 20% chance for Invasion level after level 10
            if (randomValue < 0.2)
            {
                return LevelType.Invasion;
            }

            // There's a 20% chance for a Boss level after level 10
            else if (randomValue < 0.4f)  // 0.2f to 0.4f range
            {
                return LevelType.Boss;
            }

            // There's a 15% chance for a MultiPhase Boss level after level 10
            else if (randomValue < 0.55f)  // 0.4f to 0.55f range
            {
                return LevelType.MultiPhaseBoss;
            }

            // There's a 2.5% chance for a Comet level
            else if (randomValue < 0.575f)  // 0.55f to 0.575f range
            {
                return LevelType.Comet;
            }
        }

        // Otherwise, it's a Horde level (default)
        return LevelType.Horde;

    }



    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.LevelIn)
        {
            _levels[_currentLevelIndex - 1].UpdateLevel();
        }
    }


    public void StartLevel()
    {
        Debug.Log("Starting Level from LevelManager");
        _levels[_currentLevelIndex - 1].StartLevel();
    }


    public void CompleteLevel()
    {
        _currentLevelIndex++;
        SpawnerManager.Instance.EnemiesToSpawnLeft = 0;
        _levels.Add(GenerateNextLevel());
        GameManager.Instance.ChangeState(GameManager.GameState.LevelEnd);

    }

    public Level CreateCometLevel()
    {
        int cometCount = 300;
        float cometSpawnRate = 0.5f;
        return new CometLevel(cometCount, cometSpawnRate);
    }
    Level CreateMultiPhaseBossLevel()
    {
        if (InvasionManager.Instance.DefendingFaction == FactionType.Syndicates)
        {
            return CreateSyndicatesMultiPhaseBossLevel();
        }
        else if (InvasionManager.Instance.DefendingFaction == FactionType.ThraxArmada)
        {
            return CreateThraxMultiPhaseBossLevel("Thrax", "ThraxPhase2");
        }
        else
        {
            return CreateSyndicatesMultiPhaseBossLevel();
        }
    }

    public Level CreateSyndicatesMultiPhaseBossLevel()
    {
        float health = CurrentLevelIndex * 3000f;
        int bulletAmount = Mathf.Max(CurrentLevelIndex, 10);
        float bulletDamage = Mathf.Max(CurrentLevelIndex * 2.5f, 50f);
        float bulletSpeed = Mathf.Min(Mathf.Max(CurrentLevelIndex * 1, 10), 60);
        float fireRate = 3f;
        float speed = Mathf.Min(Mathf.Max(CurrentLevelIndex * 1, 10), 60);
        float stopDistance = Mathf.Min(Mathf.Max(CurrentLevelIndex * 2.5f, 50f), 80f);
        float attackRange = Mathf.Min(Mathf.Max(CurrentLevelIndex * 2.4f, 70f), 100f);
        float fireAngle = bulletAmount * 4;
        float currencyDrop = health / 4;

        return new SyndicatesMultiPhaseBossLevel(
            health,
            bulletAmount,
            bulletDamage,
            bulletSpeed,
            fireRate,
            speed,
            stopDistance,
            attackRange,
            fireAngle,
            currencyDrop,
            SpawnerManager.Instance.SoloBossSpawnPoints,
            "SyndicatesFinalBoss",
            this,
            _spawnerManager,
            FormationType.Circle,
            10,
            30f,
            new List<string> { "MediumShip", "MediumShip2", "SmallShip", "MeleeShip" }
        );
    }

    public Level CreateSoloInvasionLevel()
    {
        float spawnRateDefending = 0.5f;
        List<Ship> shipsToSpawnInvading = _spawnerManager.DetermineSoloInvadingShips();
        List<Ship> shipsToSpawnDefending = _spawnerManager.DetermineDefendingShips();
        int spawnAmountRatio = 1 / 2;
        int amountOfEnemiesLosing = _currentLevelIndex * 3;
        FactionType factionType = InvasionManager.Instance.InvadingFactions[0];

        return new SoloInvasionLevel(
            factionType,
            spawnRateDefending,
            shipsToSpawnInvading,
            shipsToSpawnDefending,
            spawnAmountRatio,
            amountOfEnemiesLosing,
            this,
            _spawnerManager
            );
    }


    public Level CreateHordeLevel()
    {
        int amountOfEnemies = _currentLevelIndex * 3;
        List<Ship> shipsToSpawn = _spawnerManager.DetermineDefendingShips();
        float spawnRate = Mathf.Min(0.5f, _currentLevelIndex * 0.01f);
        FactionType factionType = InvasionManager.Instance.DefendingFaction;
        return new HordeLevel(
            amountOfEnemies,
            this,
            shipsToSpawn,
            spawnRate,
            _spawnerManager,
            factionType
        );
    }

    public Level CreateThraxMultiPhaseBossLevel(string bossName, string bossNamePhase2)
    {
        float health = Mathf.Max(_currentLevelIndex * 15000f, 500000f);
        int bulletAmount = Mathf.Min(Mathf.Max(_currentLevelIndex * 2, 30), 60);
        float bulletDamage = Mathf.Max(_currentLevelIndex * 5f, 50f);
        float bulletSpeed = Mathf.Min(Mathf.Max(_currentLevelIndex * 1f, 30f), 40f);
        float firerate = Random.Range(1, 3);
        float speed = Mathf.Max(_currentLevelIndex * 0.5f, 20f);
        float stopDistance = Mathf.Min(Mathf.Max(_currentLevelIndex * 2f, 90f), 120f);
        float attackRange = Mathf.Min(Mathf.Max(_currentLevelIndex * 4, 150f), 180f);
        float fireAngle = Random.Range(15, 21);
        float currencyDrop = health / 2;
        List<Vector3> spawnPoints = SpawnerManager.Instance.SoloBossSpawnPoints;
        // Choose a random formation type
        FormationType formationType = (FormationType)Random.Range(0, 7);
        Debug.Log($"Formation type: {formationType}");
        // Number of ships in formation is based on the current level index
        int numberOfShipsInFormation = Mathf.Min(_currentLevelIndex * 5, 50);
        float formationRadius = Mathf.Min(Mathf.Max(_currentLevelIndex * 2, 25f), 50f);
        List<string> formationShipName = new List<string>(_spawnerManager.GetFormationShipNames());

        return new ThraxMultiPhaseBossLevel(
            health,
            bulletAmount,
            bulletDamage,
            bulletSpeed,
            firerate,
            speed,
            stopDistance,
            attackRange,
            fireAngle,
            currencyDrop,
            spawnPoints,
            bossName,
            this,
            _spawnerManager,
            formationType,
            numberOfShipsInFormation,
            formationRadius,
            formationShipName,
            bossNamePhase2
        );
    }

    public Level CreateSoloSpawnerBossLevel(string bossName)
    {
        float health = _currentLevelIndex * 1000f;
        // Every 5 levels, add 1 extra ship to spawn
        int shipsPerSpawn = Mathf.RoundToInt(_currentLevelIndex / 3) + 1;
        float speed = Mathf.Max(_currentLevelIndex * 2f, 20f);
        float spawnRate = Mathf.Max(3f - (_currentLevelIndex * 0.1f), 0.1f);
        float stopDistance = Mathf.Min(Mathf.Max(_currentLevelIndex * 2.5f, 80f), 120f);
        float attackRange = Mathf.Min(Mathf.Max(_currentLevelIndex * 2.5f, 100f), 120f);
        float currencyDrop = health / 4;
        List<Vector3> spawnPoints = SpawnerManager.Instance.SoloBossSpawnPoints;
        return new SoloSpawnerBossLevel(
            health,
            speed,
            spawnRate,
            stopDistance,
            shipsPerSpawn,
            attackRange,
            currencyDrop,
            spawnPoints,
            bossName,
            this,
            _spawnerManager
        );
    }

    public Level CreateSoloShooterBossLevel(string bossName)
    {
        float health = _currentLevelIndex * 1000f;
        int bulletAmount = (int)Mathf.Round(_currentLevelIndex / 3);
        float bulletDamage = _currentLevelIndex * 2f;
        float bulletSpeed = Mathf.Min(Mathf.Max(_currentLevelIndex * 1f, 10), 30);
        float firerate = Random.Range(1, 5);
        float speed = Mathf.Max(_currentLevelIndex * 0.5f, 20f);
        float stopDistance = Mathf.Min(Mathf.Max(_currentLevelIndex * 2.5f, 60f), 100f);
        float attackRange = Mathf.Min(Mathf.Max(_currentLevelIndex * 2.4f, 80f), 120f);
        float fireAngle = bulletAmount * 4;
        float currencyDrop = health / 4;
        List<Vector3> spawnPoints = SpawnerManager.Instance.SoloBossSpawnPoints;
        // Choose a random formation type
        FormationType formationType = (FormationType)Random.Range(0, 7);
        Debug.Log($"Formation type: {formationType}");
        // Number of ships in formation is based on the current level index
        int numberOfShipsInFormation = Mathf.Min(_currentLevelIndex * 5, 25);
        float formationRadius = Mathf.Min(Mathf.Max(_currentLevelIndex * 2, 25f), 50f);
        List<string> formationShipName = new List<string>(_spawnerManager.GetFormationShipNames());

        return new SoloShooterBossLevel(
            health,
            bulletAmount,
            bulletDamage,
            bulletSpeed,
            firerate,
            speed,
            stopDistance,
            attackRange,
            fireAngle,
            currencyDrop,
            spawnPoints,
            bossName,
            this,
            _spawnerManager,
            formationType,
            numberOfShipsInFormation,
            formationRadius,
            formationShipName
        );
    }



    public List<Level> Levels { get => _levels; }
    public int CurrentLevelIndex { get => _currentLevelIndex; set => _currentLevelIndex = value; }

}
