using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public AudioClip clickAudio;
        private AudioSource audioSource;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }
        public void PlayGame()
        {
            StartCoroutine(PlayAndLoad());
        }

        private System.Collections.IEnumerator PlayAndLoad()
        {
            // Stoppe la musique du menu
            if (audioSource != null)
                audioSource.Stop();
            
            if (clickAudio != null)
                audioSource.PlayOneShot(clickAudio, 1f);

            
            yield return new WaitForSeconds(clickAudio != null ? clickAudio.length : 0.5f);
            SceneManager.LoadScene("SampleScene");
        }
        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}