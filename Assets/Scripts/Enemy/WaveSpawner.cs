using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WaveSpawner : StaticSpawner
{
    [Header("Wave Settings")]
    [SerializeField] private StaticSpawner _staticSpawner;
    [SerializeField] private float _resumeSpawnDelay = 5f;
    [SerializeField] private float _delayBeforeSpawn = 0.8f;
    [SerializeField] private int _minCount = 3;
    [SerializeField] private int _maxCount = 5;

    [Header("Wave Difficility Growth")]
    [SerializeField] private float _minCountValue;
    [SerializeField] private float _maxCountValue;

    private float _currentMinCount;
    private float _currentMaxCount;

    public override void ActivateSpawner()
    {
        _currentMinCount = _minCount;
        _currentMaxCount = _maxCount;

        base.ActivateSpawner();
    }

    protected override IEnumerator SpawnEnemies(float startDelay = 0f)
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(currentMinCooldown, currentMaxCooldown));

            //var enemyContainer = EnemyContainer.Instance;

            //while(!enemyContainer.IsEmpty)
            //    yield return null;

            //yield return new WaitForSeconds(_delayBeforeSpawn);

            SpawnWave();
         
            _currentMinCount += _minCountValue;
            _currentMaxCount += _maxCountValue;

            _staticSpawner.StopSpawning();

            yield return new WaitForSeconds(_resumeSpawnDelay);

            _staticSpawner.ResumeSpawning(_resumeSpawnDelay);
        }
    }

    private void SpawnWave()
    {
        for(int i = 0; i < (int)(Random.Range(_currentMinCount, _currentMaxCount)); i++)
            SpawnOneEnemy();
    }
}
