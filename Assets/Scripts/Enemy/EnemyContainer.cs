using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : SingletonComponent<EnemyContainer>
{
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

    public bool IsEmpty { get => enemies.Count == 0; }

    public void AddEnemy(GameObject enemy) 
    {
        enemies.Add(enemy);
    } 

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }
}