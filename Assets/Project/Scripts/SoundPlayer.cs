using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts
{
    public class SoundPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        public void PlayClip(AudioClip clip)
        {
            _audioSource.pitch = Random.Range(0.8f, 1.3f);
            _audioSource.PlayOneShot(clip);
        }
    }
}
