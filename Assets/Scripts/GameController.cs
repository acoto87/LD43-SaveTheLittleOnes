using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR || UNITY_WIN
using XInputDotNetPure;
#endif

public class GameController : MonoBehaviour
{
    public int initialTime = 180; // in seconds
    public int initialLives = 8;

    public PlayerController2D player;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI antidotesText;
    public TextMeshProUGUI infoText;

    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    public Image[] hearts;

    public bool won;

    private int _time;
    private float _nextSecond;

    private int _antidotes;

    private string _initialText = @"There is a problem in the village, 14 of your children are sick and need the antidote. Unfortunately, you have little time and the antidote hurts adults. Are you willing to sacrifice yourself for your little ones?
(typical platfomer controls)";
    private string _timeOutText = "You ran out of time, and now all the little ones will be dead";
    private string _deadText = @"You're dead, and now your little ones can not survive.
Press Jump to restart.";
    private string _winText = "Well done, your little ones will survive and so will you";
    private string _winNoText = "You will live but all your little ones will be dead because you didn't bring any antidote";
    private string _winNotAllText = "You will live, but some of your little ones will not because you didn't bring enough antidotes";
    private string _winSacrificeText = "You're dead, but your little ones will survive";
    private string _winNotAllSacrificeText = "You're dead, and some of your little ones will survive, but others not because you did not bring enough antidotes";

    void Start()
    {
        _time = initialTime;
        timeText.text = $"TIME: {_time}";
        _nextSecond = Time.time + 1.0f;

        _antidotes = 0;
        player.lives = initialLives;
        player._dead = false;
        player.invulnerable = false;

        infoText.text = _initialText;
        Invoke(nameof(HideInitialText), 30);

        ResetToSpawn(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }

        if (!won && player.lives > 0 && _time > 0 && Time.time >= _nextSecond)
        {
            _time--;
            timeText.text = $"TIME: {_time}";
            _nextSecond = Time.time + 1.0f;
        }

        antidotesText.text = $": {_antidotes}";

        for (int i = 0; i < hearts.Length; i++)
        {
            if (player.lives >= i + 1)
                hearts[i].sprite = fullHeart;
            else if (player.lives > i)
                hearts[i].sprite = halfHeart;
            else
                hearts[i].sprite = emptyHeart;
        }

        if (won)
        {
            if (player.lives == 0)
            {
                if (_antidotes == 14)
                    infoText.text = _winSacrificeText;
                else
                    infoText.text = _winNotAllSacrificeText;
            }
            else
            {
                if (_antidotes == 14)
                    infoText.text = _winText;
                else if (_antidotes == 0)
                    infoText.text = _winNoText;
                else
                    infoText.text = _winNotAllText;
            }
        }
        else if (player.lives == 0)
        {
            if (Input.GetButton("Jump"))
            {
                CancelInvoke(nameof(ResetGame));
                ResetGame();
            }

            infoText.text = _deadText;
            Invoke(nameof(ResetGame), 30);
        }
        else if (_time == 0)
        {
            player._dead = true;

            if (Input.GetButton("Jump"))
            {
                ResetGame();
                return;
            }

            infoText.text = _timeOutText;

            if (!IsInvoking(nameof(ResetGame)))
                Invoke(nameof(ResetGame), 30);
        }
    }

    private void ResetGame()
    {
        CancelInvoke(nameof(ResetGame));
        SceneManager.LoadScene("Game");
    }

    public void PickupAntidote()
    {
        _antidotes++;

        AudioManager.Play(GameAudioClip.Antidote);
        Invoke(nameof(PlayHurtAudio), 1.0f);
    }

    private void PlayHurtAudio()
    {
        player.lives -= 0.5f;
        if (player.lives < 0)
            player.lives = 0;
        AudioManager.Play(GameAudioClip.Hurt);
    }

    public void PickupPowerUp(float increaseSpeed, float increaseMaxJumpHeight, float increaseMinJumpHeight)
    {
        player.maxJumpHeight += increaseMaxJumpHeight;
        player.minJumpHeight += increaseMinJumpHeight;
        player.speed += increaseSpeed;
        player.CalculateGravity();
    }

    public void Hurt(float hurtAmount)
    {
        if (!player.invulnerable)
        {
            player.lives -= hurtAmount;
            if (player.lives < 0)
                player.lives = 0;
            player.invulnerable = true;
            Invoke(nameof(ResetPlayerInvulnerability), 1.5f);

            AudioManager.Play(GameAudioClip.Hurt);
        }
    }

    private void ResetPlayerInvulnerability()
    {
        player.invulnerable = false;
    }

    public void ResetToSpawn(bool vibrate = true)
    {
        if (vibrate)
        {
#if UNITY_EDITOR || UNITY_WIN
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
            Invoke("ClearGamePadVibration", 0.2f);
#endif
        }

        var playerSpawns = GameObject.FindObjectsOfType<PlayerSpawnController>();
        for (int i = 0; i < playerSpawns.Length; i++)
        {
            if (playerSpawns[i].active)
            {
                player.transform.position = playerSpawns[i].transform.position;
                break;
            }
        }
    }

    void ClearGamePadVibration()
    {
#if UNITY_EDITOR || UNITY_WIN
        GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
#endif
    }

    private void HideInitialText()
    {
        infoText.text = string.Empty;
    }
}

public static class GameTags
{
    public const string Player = "Player";
    public const string Enemy = "Enemy";
    public const string Through = "Through";
    public const string Boundary = "Boundary";
    public const string BoundaryContainer = "BoundaryContainer";
    public const string Cannon = "Cannon";
}