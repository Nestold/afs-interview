namespace AFSInterview
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class GameplayManager : MonoBehaviour
    {
        #region Singleton
        // I create singleton only for top manager in game.
        public static GameplayManager Instance;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        public List<Enemy> Enemies { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private Enemy enemyPrefab = null;
        [SerializeField] private SimpleTower simpleTowerPrefab = null;
        [SerializeField] private BurstTower burstTowerPrefab = null;

        [Header("Settings")]
        [SerializeField] private Vector2 boundsMin = new Vector2(-12, -8);
        [SerializeField] private Vector2 boundsMax = new Vector2(12, 13);
        [SerializeField] private float enemySpawnRate = 1;

        [Header("UI")] 
        [SerializeField] private TextMeshProUGUI enemiesCountText = null;
        [SerializeField] private TextMeshProUGUI scoreText = null;
        private float enemySpawnTimer;
        private int score;

        private void Start()
        {
            Enemies = new List<Enemy>();
        }

        private void Update()
        {
            enemySpawnTimer -= Time.deltaTime;

            if (enemySpawnTimer <= 0f)
            {
                SpawnEnemy();
                enemySpawnTimer = enemySpawnRate;
            }


            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, LayerMask.GetMask("Ground")))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var spawnPosition = hit.point;
                    spawnPosition.y = simpleTowerPrefab.transform.position.y;
                    SpawnTower(spawnPosition);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    var spawnPosition = hit.point;
                    spawnPosition.y = burstTowerPrefab.transform.position.y;
                    SpawnBurstTower(spawnPosition);
                }
            }

            scoreText.text = "Score: " + score;
            enemiesCountText.text = "Enemies: " + Enemies.Count;

            if(Input.GetKeyDown(KeyCode.P))
            {
                Time.timeScale = Time.timeScale == 1f ? .25f : 1f;
            }
        }

        private void SpawnEnemy()
        {
            var position = new Vector3(Random.Range(boundsMin.x, boundsMax.x), enemyPrefab.transform.position.y, Random.Range(boundsMin.y, boundsMax.y));          
            var enemy = Instantiate(enemyPrefab, position, Quaternion.identity);

            enemy.Initialize(boundsMin, boundsMax);
            Enemies.Add(enemy);
        }

        private void SpawnTower(Vector3 position)
        {
            var tower = Instantiate(simpleTowerPrefab, position, Quaternion.identity);
            tower.Initialize(Enemies);
        }

        private void SpawnBurstTower(Vector3 position)
        {
            var tower = Instantiate(burstTowerPrefab, position, Quaternion.identity);
            tower.Initialize();
        }

        public void AddScore(int amount = 1)
        {
            score += amount;
        }
    }
}