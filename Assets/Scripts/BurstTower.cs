namespace AFSInterview
{
    using System.Collections.Generic;
    using UnityEngine;

    public class BurstTower : MonoBehaviour
    {
        [SerializeField] private BurstBullet bulletPrefab = null;
        [SerializeField] private Transform bulletSpawnPoint = null;
        
        [SerializeField] private float firingRange = 4f;
        [SerializeField] private float burstFiringTime = .25f;
        [SerializeField] private float firingCooldown = 5f;
        [SerializeField] private int burstBulletsCapacity = 3;

        private Enemy targetEnemy;

        private int bulletsCount = 0;
        private float cooldownTimer = 0;
        private float burstTimer = 0;
        private bool canFire;
        private bool burstMode;
        private bool waitToFire;

        public void Initialize()
        {
            canFire = true;
        }

        private void Update()
        {
            targetEnemy = FindClosestEnemy();
            if (targetEnemy != null)
            {
                var enemyDistance = (transform.position - targetEnemy.transform.position).magnitude;
                var lookRotation = Quaternion.LookRotation(targetEnemy.NextPosition - transform.position);
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lookRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

                Debug.DrawLine(bulletSpawnPoint.position, targetEnemy.NextPosition);

                if(canFire)
                {
                    burstMode = true;
                    canFire = false;
                    waitToFire = false;
                    bulletsCount = 0;
                }

                if (burstMode)
                {
                    if (!waitToFire)
                    {
                        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
                        bullet.Initialize(firingRange * (enemyDistance / firingRange), transform.forward);
                        bulletsCount++;
                        waitToFire = true;
                        burstTimer = 0;
                    }
                    else
                    {
                        burstTimer += Time.deltaTime;
                        if (burstTimer >= burstFiringTime)
                        {
                            waitToFire = false;
                        }
                    }

                    if (bulletsCount == burstBulletsCapacity)
                    {
                        burstMode = false;
                    }
                }
            }
            else
            {
                if(burstMode)
                {
                    burstMode = false;
                    canFire = false;
                    waitToFire = false;
                    bulletsCount = 0;
                }
            }

            if (!burstMode && !canFire)
            {
                cooldownTimer += Time.deltaTime;
                if (cooldownTimer >= firingCooldown)
                {
                    cooldownTimer = 0;
                    canFire = true;
                }
            }
        }

        private Enemy FindClosestEnemy()
        {
            Enemy closestEnemy = null;
            var closestDistance = float.MaxValue;

            foreach (var enemy in GameplayManager.Instance.Enemies)
            {
                var distance = (enemy.transform.position - transform.position).magnitude;
                if (distance <= firingRange && distance <= closestDistance)
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }

            return closestEnemy;
        }
    }
}