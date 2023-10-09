using OPS.AntiCheat.Field;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<EnemyChance> enemiesChance = new List<EnemyChance>();
    [SerializeField] protected Transform parent;
    [SerializeField] protected ProtectedFloat minCooldown;
    [SerializeField] protected ProtectedFloat maxCooldown;
    [SerializeField] protected ProtectedFloat speedCoeficient = 1f;

    [Header("Difficility Growth")]
    [SerializeField] protected ProtectedFloat difficilityCooldown;
    [SerializeField] protected ProtectedFloat minCooldownValue;
    [SerializeField] protected ProtectedFloat maxCooldownValue;
    [SerializeField] private ProtectedFloat stopGrowthCooldownValue = 0.25f;
    [SerializeField] protected ProtectedFloat speedCoeficientValue;

    protected ProtectedFloat currentMinCooldown;
    protected ProtectedFloat currentMaxCooldown;
    protected ProtectedFloat currentSpeedCoeficient;

    protected Coroutine spawnCoroutine;

    private List<EnemyType> typesList = new List<EnemyType>();

    public virtual void ActivateSpawner()
    {
        currentMinCooldown = minCooldown;
        currentMaxCooldown = maxCooldown;
        currentSpeedCoeficient = speedCoeficient;

        typesList.Clear();
        
        foreach(var i in enemiesChance)
            for(int j = 0; j < i.chance; j++)
                typesList.Add(i.enemyType);

        StartCoroutine(IncreaseDifficility());
        spawnCoroutine = StartCoroutine(SpawnEnemies());
    }

    public virtual void DeactivateSpawner()
    {
        StopAllCoroutines();

        if (parent)
            foreach(var enemy in parent.GetComponentsInChildren<Enemy>())
                EnemiesPool.Instance?.AddEnemy(enemy.gameObject, enemy.enemyType);
    }

    protected virtual void OnDisable() => DeactivateSpawner();

    protected virtual IEnumerator IncreaseDifficility()
    {
        while(true)
        {
            yield return new WaitForSeconds(difficilityCooldown);

            currentMinCooldown -= minCooldownValue;
            currentMaxCooldown -=  maxCooldownValue;
            currentSpeedCoeficient += speedCoeficientValue;

            currentMinCooldown = currentMinCooldown < stopGrowthCooldownValue ? stopGrowthCooldownValue : currentMinCooldown;
            currentMaxCooldown = currentMaxCooldown < stopGrowthCooldownValue ? stopGrowthCooldownValue : currentMaxCooldown;
        }
    }
    
    public void StopSpawning()
    {
        StopCoroutine(spawnCoroutine);
    }

    public void ResumeSpawning(float startDelay = 0f)
    {
        spawnCoroutine = StartCoroutine(SpawnEnemies(startDelay));
    }

    protected virtual IEnumerator SpawnEnemies(float startDelay = 0f)
    {
        yield return new WaitForSeconds(startDelay);

        while(true)
        {
            yield return new WaitForSeconds(Random.Range(currentMinCooldown, currentMaxCooldown));
            SpawnOneEnemy();
        }
    }

    protected void SpawnOneEnemy()
    {
        var camera = Camera.main;
        var spawnPosition = Vector3.zero;

        var random1 = Random.Range(0, 2); // 00 - down, 01 - up, 10 - left, 11 - right
        var random2 = Random.Range(0, 2); 

        if (random1 == 0)
            spawnPosition = camera.ScreenToWorldPoint(new Vector3(Random.Range(0, camera.pixelWidth), random2 == 0 ? 0 : camera.pixelHeight, -camera.transform.position.z));
    
        if (random1 == 1)
            spawnPosition = camera.ScreenToWorldPoint(new Vector3(random2 == 0 ? 0 : camera.pixelWidth, Random.Range(0, camera.pixelHeight), -camera.transform.position.z));
        
        var directionOffset = camera.pixelWidth / 5;
        var direction = (camera.ScreenToWorldPoint(new Vector3(Random.Range(0 + directionOffset, camera.pixelWidth - directionOffset), Random.Range(0 + directionOffset, camera.pixelWidth - directionOffset), -camera.transform.position.z)) - spawnPosition).normalized;

        var enemy = EnemiesPool.Instance.GetEnemy(typesList[Random.Range(0, 100)]);

        var meshFilter = enemy.GetComponent<MeshFilter>();
        var sphereCollider = enemy.GetComponent<SphereCollider>();

        var spawnOffset = meshFilter ? new Vector2(meshFilter.sharedMesh.bounds.size.x * enemy.transform.localScale.x / 2, meshFilter.sharedMesh.bounds.size.y * enemy.transform.localScale.y / 2) : sphereCollider ? new Vector2(sphereCollider.radius, sphereCollider.radius) : Vector2.zero;

        spawnPosition.x += random1 == 1 && random2 == 1 ? spawnOffset.x : random1 == 1 && random2 == 0 ? -spawnOffset.x : 0;
        spawnPosition.y += random1 == 0 && random2 == 1 ? spawnOffset.y : random1 == 0 && random2 == 0 ? -spawnOffset.y : 0;

        enemy.transform.position = spawnPosition;

        var enemyComponent = enemy.GetComponent<Enemy>();

        enemyComponent.SetParent(parent);
        enemyComponent.SetSpeedCoeficient(currentSpeedCoeficient);
        enemyComponent.Move(direction, currentSpeedCoeficient);
    }
}
