using OPS.AntiCheat.Field;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Characteristic")]
    public EnemyType enemyType;
    [SerializeField] private ProtectedFloat _minSpeed;
    [SerializeField] private ProtectedFloat _maxSpeed;

    [Header("Spawn On Destroy")]
    [SerializeField] private List<EnemyType> _child = new List<EnemyType>();
    [SerializeField] private float _childSpeedCoeficient = 0.5f;

    [Header("Physics")]
    [SerializeField] private Rigidbody _rigidBody;

    [Header("Effects")]
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField] private ParticleSystem _destroyEffect;

    private Transform _parent;
    private float _speedCoeficient;
    protected float _speed;

    protected ProtectedInt16 _specialReward = 5;
    protected ProtectedInt16 _regularReward = 1;

    private void OnDisable()
    {
        StopAllCoroutines();
    } 

    public void Move(Vector2 direction, float speedCoeficient)
    {
        _speed = Random.Range(_minSpeed, _maxSpeed) * speedCoeficient;
        _rigidBody.velocity = direction * _speed;

        StartCoroutine(PositionControl());
    }

    public void SetParent(Transform parent) => _parent = parent;

    public void SetSpeedCoeficient(float speedCoeficient) => _speedCoeficient = speedCoeficient;

    private IEnumerator PositionControl()
    {
        var camera = Camera.main;

        var meshFilter = GetComponent<MeshFilter>();
        var sphereCollider = GetComponent<SphereCollider>();

        var offset = meshFilter ? new Vector3(meshFilter.sharedMesh.bounds.size.x * transform.localScale.x / 2, meshFilter.sharedMesh.bounds.size.y * transform.localScale.y / 2, 0) : sphereCollider ? new Vector3(sphereCollider.radius, sphereCollider.radius, 0) : Vector3.zero;

        var minPosition = camera.ScreenToWorldPoint(new Vector3(0, 0, -camera.transform.position.z)) - offset;
        var maxPosition = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, -camera.transform.position.z)) + offset;

        var delay = 0.5f / _speed;
        bool activateDelay = true;

        while(true)
        {
            if (activateDelay)
            {
                activateDelay = false;
                yield return new WaitForSeconds(delay);
            }

            if (transform.position.x >= maxPosition.x || transform.position.x <= minPosition.x)
            {
                if (enemyType == EnemyType.Friendly)
                {
                    EnemiesPool.Instance?.AddEnemy(gameObject, enemyType);
                    break;
                }

                activateDelay = true;

                transform.position += new Vector3(-2 * transform.position.x, 0, 0);
            }
                
            if (transform.position.y >= maxPosition.y || transform.position.y <= minPosition.y)
            {
                if (enemyType == EnemyType.Friendly)
                {
                    EnemiesPool.Instance?.AddEnemy(gameObject, enemyType);
                    break;
                }

                activateDelay = true;

                transform.position += new Vector3(0, -2 * transform.position.y, 0);
            }
                
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spaceship") && enemyType != EnemyType.Friendly)
        {
            _rigidBody.velocity = Vector3.zero;
            GetComponent<EnemyRotator>()?.StopRotation();
            GUI.Instance?.EndGame();
        }
    }

    public void Destroy()
    {
        StopAllCoroutines();

        if (_child.Count > 0)
        {
            var degree = Random.Range(0, 360);
            var step = 360 / _child.Count;

            for(int i = 0; i < _child.Count; i++)
            {
                var enemy = EnemiesPool.Instance?.GetEnemy(_child[i]);
                enemy.transform.position = transform.position;

                var enemyComponent = enemy.GetComponent<Enemy>();

                enemyComponent.SetParent(_parent);
                enemyComponent.SetSpeedCoeficient(_speedCoeficient);
                enemyComponent.Move(new Vector2(Mathf.Sin(degree * Mathf.Deg2Rad) * -1, Mathf.Cos(degree * Mathf.Deg2Rad)).normalized, _speedCoeficient * _childSpeedCoeficient);

                degree += step;
            }
        }

        GUI.Instance?.UpdateScoreCounter(enemyType == EnemyType.Special ? _specialReward : enemyType != EnemyType.Friendly ? _regularReward : 0);

        PlayDestroySound(); 
        StartCoroutine(DestroyEffect());
    }

    private IEnumerator DestroyEffect()
    {
        if (enemyType == EnemyType.SmallMeteorite || enemyType == EnemyType.MediumMeteorite || enemyType == EnemyType.LargeMeteorite)
        {
            _meshCollider.enabled = false;
            _meshRenderer.enabled = false;
        }

        else if (enemyType == EnemyType.Special || enemyType == EnemyType.Friendly)
        {
            _sphereCollider.enabled = false;
            _spriteRenderer.enabled = false;
        }

        _rigidBody.velocity = Vector3.zero;

        if (_destroyEffect)
        {
            if (enemyType == EnemyType.SmallMeteorite || enemyType == EnemyType.MediumMeteorite || enemyType == EnemyType.LargeMeteorite)
                GUI.Instance.PlayCameraAnimation();
                
            _destroyEffect.Play();

            while(_destroyEffect.isPlaying)
                yield return null;
        }

        EnemiesPool.Instance?.AddEnemy(gameObject, enemyType);

        if (enemyType == EnemyType.SmallMeteorite || enemyType == EnemyType.MediumMeteorite || enemyType == EnemyType.LargeMeteorite)
        {
            _meshCollider.enabled = true;
            _meshRenderer.enabled = true;
        }

        else if (enemyType == EnemyType.Special || enemyType == EnemyType.Friendly)
        {
            _sphereCollider.enabled = true;
            _spriteRenderer.enabled = true;
        }
    }

    private void PlayDestroySound()
    {
        if (enemyType == EnemyType.Friendly)
            SoundManager.Instance.PlayFriendDestroySound();

        else if (enemyType == EnemyType.SmallMeteorite || enemyType == EnemyType.MediumMeteorite || enemyType == EnemyType.LargeMeteorite)
            SoundManager.Instance.PlayAsteroidDestroySound();

        else if (enemyType == EnemyType.Special)
            SoundManager.Instance.PlayEnemyDestroySound();
    }
}
