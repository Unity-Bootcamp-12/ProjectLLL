using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform _redTeamSpawnPoint;
    [SerializeField] private Transform _blueTeamSpawnPoint;

    [SerializeField] private Transform _redTeamMinionSpawnPoint;
    [SerializeField] private Transform _blueTeamMinionSpawnPoint;

    [SerializeField] private Transform _redTeamTowerSpawnPoint;
    [SerializeField] private Transform _blueTeamTowerSpawnPoint;

    [SerializeField] private GameObject _minionPrefab;
    [SerializeField] private GameObject _towerPrefab;
    [SerializeField] private GameObject _playerPrefab;

    const float MINION_SPAWN_TIME = 10.0f;

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
        }
        else if (IsClient)
        {
            Logger.Info("Client started");
        }

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
            MinionSpawnWave();
            TowerSpawn();
        }
    }

    private void MinionSpawnWave()
    {
        if (IsHost)
        {
            SpawnMinion(UnitTeamType.RedTeam);
            SpawnMinion(UnitTeamType.BlueTeam);

            PlayAfterCoroutine(() =>
            {
                MinionSpawnWave();
            }, MINION_SPAWN_TIME);
        }
    }

    private void PlayerSpawn(ulong clientId)
    { 
        if (IsHost)
        {
            GameObject redTeamPlayer = Instantiate(_playerPrefab, _redTeamSpawnPoint.position, Quaternion.identity);
            redTeamPlayer.GetComponent<PlayerController>().Init(UnitTeamType.RedTeam, NetworkManager.Singleton.LocalClientId);

            GameObject blueTeamPlayer = Instantiate(_playerPrefab, _blueTeamSpawnPoint.position, Quaternion.identity);
            blueTeamPlayer.GetComponent<PlayerController>().Init(UnitTeamType.BlueTeam, clientId);
        }
    }

    private void TowerSpawn()
    {
        if (IsHost)
        {
            GameObject redTeamTower = Instantiate(_towerPrefab, _redTeamTowerSpawnPoint.position, Quaternion.identity);
            redTeamTower.GetComponent<TowerController>().Init(UnitTeamType.RedTeam);
            GameObject blueTeamTower = Instantiate(_towerPrefab, _blueTeamTowerSpawnPoint.position, Quaternion.identity);
            blueTeamTower.GetComponent<TowerController>().Init(UnitTeamType.BlueTeam);
        }
    }

    private void SpawnMinion(UnitTeamType team)
    {
        if (team == UnitTeamType.RedTeam)
        {
            GameObject minionObject = Instantiate(_minionPrefab, _redTeamMinionSpawnPoint.position, Quaternion.identity);
            minionObject.GetComponent<MinionController>().Init(UnitTeamType.RedTeam, _blueTeamTowerSpawnPoint.position);
        }
        else if (team == UnitTeamType.BlueTeam)
        {
            GameObject minionObject = Instantiate(_minionPrefab, _blueTeamMinionSpawnPoint.position, Quaternion.identity);
            minionObject.GetComponent<MinionController>().Init(UnitTeamType.BlueTeam, _redTeamTowerSpawnPoint.position);
        }
    }

    public void PlayAfterCoroutine(Action action, float time) => StartCoroutine(PlayCoroutine(action, time));
    private IEnumerator PlayCoroutine(Action action, float time)
    {
        yield return new WaitForSeconds(time);

        action();
    }
}