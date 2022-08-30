using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesPool : SingletonComponent<EnemiesPool>
{
    [SerializeField] private List<GameObject> _smallMeteorsPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> _mediumMeteorsPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> _largeMeteorsPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> _friendsPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> _specialEnemiesPrefabs = new List<GameObject>();

    private Dictionary<EnemyType, List<GameObject>> _enemiesPool = new Dictionary<EnemyType, List<GameObject>>();

    public void EnemiesPoolInit()
    {
        _enemiesPool.Clear();
        
        _enemiesPool.Add(EnemyType.SmallMeteorite, new List<GameObject>());
        _enemiesPool.Add(EnemyType.MediumMeteorite, new List<GameObject>());
        _enemiesPool.Add(EnemyType.LargeMeteorite, new List<GameObject>());
        _enemiesPool.Add(EnemyType.Friendly, new List<GameObject>());
        _enemiesPool.Add(EnemyType.Special, new List<GameObject>());

        for(int i = 0; i < 3; i++)
        {
            SpawnEnemy(EnemyType.SmallMeteorite);
            SpawnEnemy(EnemyType.MediumMeteorite);
            SpawnEnemy(EnemyType.LargeMeteorite);
            SpawnEnemy(EnemyType.Friendly);
            SpawnEnemy(EnemyType.Special);
        }
    }

    private void SpawnEnemy(EnemyType enemyType)
    {
        GameObject prefab = _smallMeteorsPrefabs[Random.Range(0, _smallMeteorsPrefabs.Count)];

        if (enemyType == EnemyType.MediumMeteorite)
            prefab = _mediumMeteorsPrefabs[Random.Range(0, _mediumMeteorsPrefabs.Count)];

        else if (enemyType == EnemyType.LargeMeteorite)
            prefab = _largeMeteorsPrefabs[Random.Range(0, _largeMeteorsPrefabs.Count)];

        else if (enemyType == EnemyType.Friendly)
            prefab = _friendsPrefabs[Random.Range(0, _friendsPrefabs.Count)];

        else if (enemyType == EnemyType.Special)
            prefab = _specialEnemiesPrefabs[Random.Range(0, _specialEnemiesPrefabs.Count)];
        
        var enemy = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        enemy.SetActive(false);

        _enemiesPool[enemyType].Add(enemy);
    }

    public void AddEnemy(GameObject enemy, EnemyType enemyType)
    {
        enemy.SetActive(false);

        _enemiesPool[enemyType].Add(enemy);
    }

    public GameObject GetEnemy(EnemyType enemyType)
    {
        if(_enemiesPool[enemyType].Count == 0)
            SpawnEnemy(enemyType);

        var random = Random.Range(0, _enemiesPool[enemyType].Count);
        var enemy = _enemiesPool[enemyType][random];

        enemy.SetActive(true);

        _enemiesPool[enemyType].RemoveAt(random);

        return enemy;
    }
}
