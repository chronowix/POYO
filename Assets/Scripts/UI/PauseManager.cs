using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


namespace Platformer.UI
{
    public class PauseManager : MonoBehaviour
    {
        public GameObject pauseMenu;
        public GameObject controlsMenu;
        public Slider volumeSlider;

        private bool isPaused = false;

        private bool isInitialized = false;

        void Start()
        {
            pauseMenu.SetActive(false);
            controlsMenu.SetActive(false);
            volumeSlider.value = 1f;
            AudioListener.volume = 1f;
            isInitialized = true;
        }

        public void SetVolume(float volume)
        {
            if (!isInitialized) return;
            AudioListener.volume = volume;
        }

        private float pauseCooldown = 0f;

        void Update()
        {
            pauseCooldown -= Time.unscaledDeltaTime;
    
            if (Keyboard.current.escapeKey.wasPressedThisFrame && pauseCooldown <= 0f)
            {
                pauseCooldown = 0.2f;
                if (isPaused)
                    Resume();
                else
                    Pause();
            }
        }

        public void Pause()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }

        public void Resume()
        {
            pauseMenu.SetActive(false);
            controlsMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }

        public void ShowControls()
        {
            pauseMenu.SetActive(false);
            controlsMenu.SetActive(true);
        }

        public void HideControls()
        {
            controlsMenu.SetActive(false);
            pauseMenu.SetActive(true);
        }


        public void QuitGame()
        {
            Time.timeScale = 1f;
            Application.Quit();
        }
    }
}