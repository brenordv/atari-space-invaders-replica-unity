using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Project.Scripts
{
    public class Player : BaseShip
    {
        public float speed = 1.5f;
        public float horizontalLimit = 2.5f;
        public PlayerMissile playerMissilePrefab;
        public float firingSpeed = 3f;
        public float fireCooldown = 1f;
        
        private Rigidbody2D _rigidbody2D;
        private bool _waitingCooldown;
        
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            MovePlayer();
            KeepPlayerWithinBounds();
            Fire();
        }

        private bool WeaponCoolingDown()
        {
            return _waitingCooldown;
        }

        private IEnumerator ResetCooldownTimer()
        {
            _waitingCooldown = true;
            yield return new WaitForSeconds(fireCooldown);
            _waitingCooldown = false;
        }

        private void Fire()
        {
            if (!Input.GetButtonDown("Fire1") || WeaponCoolingDown()) return;
            StartCoroutine(ResetCooldownTimer());

            var missile = Instantiate(playerMissilePrefab, transform.parent);
            missile.transform.position = transform.position;
            missile.GetComponent<Rigidbody2D>().velocity = new Vector2(0, firingSpeed);
            soundPlayer.PlayClip(shotSound);
            Destroy(missile, 2f);
        }

        private void KeepPlayerWithinBounds()
        {
            var position = transform.position;

            if (position.x > horizontalLimit)
            {
                transform.position = new Vector3(horizontalLimit, position.y, position.z);
                _rigidbody2D.velocity = Vector2.zero;
            }
            else if (position.x < -horizontalLimit)
            {
                transform.position = new Vector3(-horizontalLimit, position.y, position.z);
                _rigidbody2D.velocity = Vector2.zero;
            }
        }

        private void MovePlayer()
        {
            var x = Input.GetAxis("Horizontal") * speed;
            _rigidbody2D.velocity = new Vector2(x, 0);
        }
    }
}