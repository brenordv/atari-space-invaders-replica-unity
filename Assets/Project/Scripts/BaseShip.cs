using Project.Scripts.UI;
using UnityEngine;

namespace Project.Scripts
{
    public class BaseShip: MonoBehaviour
    {
        public SoundPlayer soundPlayer;
        public AudioClip deathSound;
        public AudioClip shotSound;
        public ParticleSystem explosionPrefab;
        
        [TagSelector]
        public string enemyMissileTag;

        [TagSelector]
        public string enemyShipTag;
        
        public delegate void ShipHandler();

        public event ShipHandler OnShipDied;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(enemyMissileTag) && !other.CompareTag(enemyShipTag)) return;
            soundPlayer.PlayClip(deathSound);
            var explosion = Instantiate(explosionPrefab, transform.parent);
            explosion.transform.position = transform.position;
            
            Destroy(explosion, 1.5f);
            
            if (other != null)
                Destroy(other.gameObject);
            
            
            OnShipDied?.Invoke();
            Destroy(gameObject);
        }
    }
}