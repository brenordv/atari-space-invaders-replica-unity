using System;
using UnityEngine;

namespace Project.Scripts
{
    public class Enemy : BaseShip
    {
        private void Awake()
        {
            if (soundPlayer != null) return;
            //I'm not happy with this approach. Feels a bit hacky, but works well for a project this size.
            soundPlayer = GameObject
                .Find("GameController")
                .GetComponentInChildren<SoundPlayer>();
        }
    }
}