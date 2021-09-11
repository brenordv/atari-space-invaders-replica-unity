using System;
using System.Collections;
using Project.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts
{
    [Serializable]
    public class EnemyMovement
    {
        public float maximumMovingInterval = .4f;
        public float minimumMovingInterval = .02f;
        public float movingDistance = .1f;
    }

    [Serializable]
    public class ScoreController
    {
        public int basePointsPerEnemy = 2;
        public Text scoreText;
        private int _currentScore;


        public void UpdateScoreboard()
        {
            scoreText.text = _currentScore.ToString();
        }

        public void ScorePoints(float difficultyModifier)
        {
            _currentScore += basePointsPerEnemy + Mathf.RoundToInt(basePointsPerEnemy * difficultyModifier);
            UpdateScoreboard();
        }
    }
    
    public class GameController : MonoBehaviour
    {
        public float horizontalLimit = 2.5f;
        public float shootingInterval = 3f;
        public float shootingSpeed = 1f;
        public EnemyMissile enemyMissilePrefab;
        public GameObject enemyContainer;
        public Player player;
        public EnemyMovement enemyMovement;
        public ScoreController scoreController;
        public Text winnerText;
        public Text loserText;
        
        private float _initialEnemyCount;
        private float _currentEnemyCount;
        private float _currentMovingInterval;
        private float _movingDirection = -1f;
        private float _currentDifficulty;

        private void Awake()
        {
            var enemies = GetComponentsInChildren<Enemy>();
            _initialEnemyCount = enemies.Length;
            _currentEnemyCount = _initialEnemyCount;
            _currentMovingInterval = enemyMovement.maximumMovingInterval;
            RegisterForEnemyEvents(enemies);
            player.OnShipDied += OnPlayerDied;
            scoreController.UpdateScoreboard();
        }

        private void Start()
        {
            StartCoroutine(PrepareToShoot());
            StartCoroutine(PrepareToMove());
        }

        private void RegisterForEnemyEvents(Enemy[] enemies)
        {
            foreach (var enemy in enemies)
            {
                enemy.OnShipDied += OnEnemyDied;
            }
        }
        
        private void OnEnemyDied()
        {
            _currentEnemyCount--;
            scoreController.ScorePoints(_currentDifficulty);
            
            if (_currentEnemyCount > 0) return;
            winnerText.gameObject.SetActive(true);
        }

        private void OnPlayerDied()
        {
            loserText.gameObject.SetActive(true);
        }
        
        private IEnumerator PrepareToMove()
        {
            while (true)
            {
                yield return new WaitForSeconds(_currentMovingInterval);
                MoveEnemies();
                UpdateMovingInterval();
            }
        }

        private void UpdateMovingInterval()
        {
            var remainingEnemiesPercent = _currentEnemyCount / _initialEnemyCount;
            _currentDifficulty = 1f - remainingEnemiesPercent;
            var updatedMovingInterval = enemyMovement.maximumMovingInterval * remainingEnemiesPercent;
            _currentMovingInterval = Mathf.Clamp(updatedMovingInterval, enemyMovement.minimumMovingInterval,
                enemyMovement.maximumMovingInterval);
        }
        
        private void MoveEnemies()
        {
            var position = enemyContainer.transform.position;
            var x = position.x + (enemyMovement.movingDistance * _movingDirection); 
            enemyContainer.transform.position = new Vector3(x, position.y, 0);

            if (_movingDirection > 0)
            {
                var rightmostPosition = 0f;
                foreach (var enemy in GetComponentsInChildren<Enemy>())
                {
                    var enemyX = enemy.transform.position.x;
                    if (enemyX <= rightmostPosition) continue;
                    rightmostPosition = enemyX;
                }

                if (rightmostPosition > horizontalLimit)
                {
                    _movingDirection *= -1;
                    position = enemyContainer.transform.position;
                    var y = position.y - enemyMovement.movingDistance; 
                    enemyContainer.transform.position = new Vector3(position.x, y, 0);
                }
            }
            else
            {
                var leftmostPosition = 0f;
                foreach (var enemy in GetComponentsInChildren<Enemy>())
                {
                    var enemyX = enemy.transform.position.x;
                    if (enemyX >= leftmostPosition) continue;
                    leftmostPosition = enemyX;
                }
                
                if (leftmostPosition < -horizontalLimit)
                {
                    _movingDirection *= -1;
                    position = enemyContainer.transform.position;
                    var y = position.y - enemyMovement.movingDistance; 
                    enemyContainer.transform.position = new Vector3(position.x, y, 0);
                }
            }
            
        }

        private IEnumerator PrepareToShoot()
        {
            while (true)
            {
                yield return new WaitForSeconds(shootingInterval);
                ShootPlayer();
            }
        }

        private void ShootPlayer()
        {
            var enemies = GetComponentsInChildren<Enemy>();
            if (enemies.Length == 0) return;
            var enemy = enemies.Random();

            var missile = Instantiate(enemyMissilePrefab, transform);
            missile.transform.position = enemy.transform.position;
            missile.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -shootingSpeed);
            Destroy(missile, 6f);
        }
    }
}