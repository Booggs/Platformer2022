namespace GSGD2.Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MusicSwapper : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _zoneMusic = null;

        private void OnTriggerEnter(Collider other)
        {
            AudioSource audioSource = other.GetComponent<AudioSource>();
            if (audioSource != null && audioSource.clip != _zoneMusic)
            {
                audioSource.clip = _zoneMusic;
                audioSource.Play();
            }
        }
    }
}