#nullable enable
using UnityEngine;

namespace VHDPV2.UI
{
    public sealed class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject? panel;
        private bool _isPaused;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
            if (panel != null)
            {
                panel.SetActive(_isPaused);
            }
        }

        public void Resume()
        {
            if (_isPaused)
            {
                TogglePause();
            }
        }

        public void QuitToMenu()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
