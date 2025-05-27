using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform _redTeamSpawnPoint;
    [SerializeField] private Transform _blueTeamSpawnPoint;

    [SerializeField] private Transform _redTeamMinionSpawnPoint;
    [SerializeField] private Transform _blueTeamMinionSpawnPoint;

    [SerializeField] private Transform _redTeamTowerSpawnPoint;
    [SerializeField] private Transform _blueTeamTowerSpawnPoint;

    [SerializeField] private GameObject _redMeleeMinionPrefab;
    [SerializeField] private GameObject _redRangedMinionPrefab;

    [SerializeField] private GameObject _blueMeleeMinionPrefab;
    [SerializeField] private GameObject _blueRangedMinionPrefab;

    [SerializeField] private GameObject _redTowerPrefab;
    [SerializeField] private GameObject _blueTowerPrefab;

    [SerializeField] private GameObject _meleePlayerPrefab;
    [SerializeField] private GameObject _rangedPlayerPrefab;

    [SerializeField] private GameObject[] _itemPrefabList;

    const float MINION_SPAWN_TIME = 20.0f;

    public UnitTeamType LocalPlayerTeamType => _localPlayerTeamType;
    private UnitTeamType _localPlayerTeamType;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        Logger.Info("OnNetworkSpawn : " + NetworkManager.Singleton.LocalClientId);
        if (IsHost)
        {
            Logger.Info("Server started");
            _localPlayerTeamType = UnitTeamType.RedTeam;
        }
        else if (IsClient)
        {
            Logger.Info("Client started");
            _localPlayerTeamType = UnitTeamType.BlueTeam;
        }

        Logger.Info("LocalPlayerTeamType : " + _localPlayerTeamType);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count == 2)
        {
            Logger.Info("Game start");
            PlayerSpawn(clientId);
            StartCoroutine(SpawnMinionWaveCoroutine());
            TowerSpawn();
        }
    }

    private void PlayerSpawn(ulong clientId)
    {
        if (IsHost)
        {
            GameObject redTeamPlayer = Instantiate(_meleePlayerPrefab, _redTeamSpawnPoint.position, Quaternion.identity);
            redTeamPlayer.GetComponent<PlayerController>().Init(UnitTeamType.RedTeam, NetworkManager.Singleton.LocalClientId);

            GameObject blueTeamPlayer = Instantiate(_rangedPlayerPrefab, _blueTeamSpawnPoint.position, Quaternion.identity);
            blueTeamPlayer.GetComponent<PlayerController>().Init(UnitTeamType.BlueTeam, clientId);
        }
    }

    private void TowerSpawn()
    {
        if (IsHost)
        {
            GameObject redTeamTower = Instantiate(_redTowerPrefab, _redTeamTowerSpawnPoint.position, Quaternion.identity);
            redTeamTower.GetComponent<TowerController>().Init(UnitTeamType.RedTeam, OwnerClientId);
            GameObject blueTeamTower = Instantiate(_blueTowerPrefab, _blueTeamTowerSpawnPoint.position, Quaternion.identity);
            blueTeamTower.GetComponent<TowerController>().Init(UnitTeamType.BlueTeam, OwnerClientId);
        }
    }

    private IEnumerator SpawnMinionWaveCoroutine()
    {
        SpawnMinion(_redMeleeMinionPrefab, _redTeamMinionSpawnPoint.position, UnitTeamType.RedTeam, _blueTeamTowerSpawnPoint.position);
        SpawnMinion(_blueMeleeMinionPrefab, _blueTeamMinionSpawnPoint.position, UnitTeamType.BlueTeam, _redTeamTowerSpawnPoint.position);

        yield return new WaitForSeconds(0.5f);

        SpawnMinion(_redMeleeMinionPrefab, _redTeamMinionSpawnPoint.position, UnitTeamType.RedTeam, _blueTeamTowerSpawnPoint.position);
        SpawnMinion(_blueMeleeMinionPrefab, _blueTeamMinionSpawnPoint.position, UnitTeamType.BlueTeam, _redTeamTowerSpawnPoint.position);

        yield return new WaitForSeconds(0.5f);

        SpawnMinion(_redRangedMinionPrefab, _redTeamMinionSpawnPoint.position, UnitTeamType.RedTeam, _blueTeamTowerSpawnPoint.position);
        SpawnMinion(_blueRangedMinionPrefab, _blueTeamMinionSpawnPoint.position, UnitTeamType.BlueTeam, _redTeamTowerSpawnPoint.position);

        yield return new WaitForSeconds(MINION_SPAWN_TIME);

        StartCoroutine(SpawnMinionWaveCoroutine());
    }

    void SpawnMinion(GameObject prefab, Vector3 spawnPos, UnitTeamType team, Vector3 targetPos)
    {
        var minion = Instantiate(prefab, spawnPos, Quaternion.identity).GetComponent<MinionController>();
        minion.Init(team, OwnerClientId);
        minion.SetDestination(targetPos);
    }

    [Rpc(SendTo.Server)]
    public void SpawnItemRpc(Vector3 position)
    {
        GameObject itemObject = Instantiate(_itemPrefabList[UnityEngine.Random.Range(0,
            _itemPrefabList.Length)], position, Quaternion.identity);
        itemObject.GetComponent<ItemObject>().Init();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void GameOverRpc(UnitTeamType destroyedTeam)
    {
        bool isWin = destroyedTeam != _localPlayerTeamType;
        UIManager.Instance.SetGameOverUI(isWin);
    }

    public Vector3 GetRespawnPoint(UnitTeamType teamType)
    {
        if (teamType == UnitTeamType.RedTeam)
        {
            return _redTeamSpawnPoint.position;
        }
        else if (teamType == UnitTeamType.BlueTeam)
        {
            return _blueTeamSpawnPoint.position;
        }

        return Vector3.zero;
    }

    public void PlayAfterCoroutine(Action action, float time) => StartCoroutine(PlayCoroutine(action, time));
    private IEnumerator PlayCoroutine(Action action, float time)
    {
        yield return new WaitForSeconds(time);

        action();
    }
}