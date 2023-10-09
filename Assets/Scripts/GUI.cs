using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using OPS.AntiCheat.Field;
using OPS.AntiCheat.Speed;

public class GUI : SingletonComponent<GUI>
{
    [SerializeField] private GameObject _menuUI;
    [SerializeField] private GameObject _HUD;
    [SerializeField] private GameObject _menuWindow;
    [SerializeField] private GameObject _noticeWindow;
    [SerializeField] private GameObject _userInfoWindow;
    [SerializeField] private GameObject _gameOverWindow;
    [SerializeField] private GameObject _loadingMessage;
    [SerializeField] private GameObject _warning;
    [SerializeField] private TextMeshProUGUI _scoreCounter;

    [SerializeField] private SpaceshipController _spaceship;
    [SerializeField] private StaticSpawner _staticSpawner;
    [SerializeField] private WaveSpawner _waveSpawner;

    [SerializeField] private Animation _cameraAnimation;

    [SerializeField] private GameObject _leadersContainer;

    [SerializeField] private Animator _hintAnimator;

    private ProtectedInt32 _score = 0;
    private const int NAME_INDEX = 2;
    private const int VALUE_INDEX = 3;

    private void Start()
    {
        StartCoroutine(SwitchByPressingEnter());
    }

    private IEnumerator SwitchByPressingEnter()
    {
        yield return new WaitForSeconds(0.5f);

        while(true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (_menuWindow.activeInHierarchy)
                {
                    _menuWindow.SetActive(false);
                    _noticeWindow.SetActive(true);
                    SoundManager.Instance.PlayMenuButtonSound();
                }

                else if (_noticeWindow.activeInHierarchy)
                {
                    _noticeWindow.SetActive(false);

                    SoundManager.Instance.PlayMenuButtonSound();

                    StartGame();
                }

                else if(_gameOverWindow.activeInHierarchy)
                {
                    _gameOverWindow.SetActive(false);

                    SoundManager.Instance.PlayMenuButtonSound();

                    StartGame();
                }

                else if (_userInfoWindow.activeInHierarchy)
                {
                    PlayfabManager.Instance.OnLoginButtonPressed();
                }
            }

            yield return null;
        }
    }

    public void UpdateScoreCounter(int value = 0)
    {
        _score += value;
        _scoreCounter.text = _score + "";
    }

    public void StartGame()
    {
        _spaceship.ActivateSpaceship();
        _staticSpawner.ActivateSpawner();
        _waveSpawner.ActivateSpawner();
        EnemiesPool.Instance?.EnemiesPoolInit();

        _score = 0;
        UpdateScoreCounter();
        _menuUI.SetActive(false);
        _HUD.SetActive(true);

        _hintAnimator.SetTrigger("ShowHint");
    }

    public void HideHint()
    {
        _hintAnimator.SetTrigger("HideHint");
    }

    public void EndGame()
    {
        StartCoroutine(EndGameWithDelay());
    }

    private IEnumerator EndGameWithDelay()
    {
        ProtectedTime.timeScale = 0;
        _spaceship.PlayDestroyEffect();

        yield return new WaitForSecondsRealtime(1.5f);

        ProtectedTime.timeScale = 1;

        _spaceship.DeactivateSpaceship();
        _staticSpawner.DeactivateSpawner();
        _waveSpawner.DeactivateSpawner();

        _menuUI.SetActive(true);
        _userInfoWindow.SetActive(true);
        _HUD.SetActive(false);
    }

    public void PlayCameraAnimation()
    {
        //if (_cameraAnimation.isPlaying)
        //    _cameraAnimation.Stop();

        //_cameraAnimation.Play();
    }

    public void UpdateLeaderBoard()
    {
        var leaderboard = PlayfabManager.Instance.GetLeaderboard();

        _gameOverWindow.SetActive(true);

        int size = leaderboard.Count;
        for (int i = 0; i < size; ++i)
        {
            var leader = _leadersContainer.transform.GetChild(i).transform;
            leader.GetChild(NAME_INDEX).GetComponent<TextMeshProUGUI>().text = leaderboard[i].DisplayName;
            leader.GetChild(VALUE_INDEX).GetComponent<TextMeshProUGUI>().text = leaderboard[i].StatValue.ToString();
        }
    }

    public int GetScore => _score;

    public void ToggleLoadingMessage()
    {
        _loadingMessage.SetActive(!_loadingMessage.activeInHierarchy);
    }

    public void OnLogginButtonPressed()
    {
        _userInfoWindow.SetActive(false);
        _menuUI.SetActive(false);
        SoundManager.Instance.PlayMenuButtonSound();
    }

    public void ShowWarning()
    {
        _warning.SetActive(true);
    }
    public void HideWarning()
    {
        _warning.SetActive(false);
    }
}
