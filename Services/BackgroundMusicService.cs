using Plugin.Maui.Audio;
using Microsoft.Maui.Storage;

namespace MathMaster.Services;

public class BackgroundMusicService
{
    private static BackgroundMusicService? _instance;
    public static BackgroundMusicService Instance => _instance ??= new BackgroundMusicService();

    private IAudioPlayer? _player;
    private bool _isPlaying = false;
    private string? _currentFile = null;
    private double _volume = Preferences.Get("MusicVolume", 1.0);

    private BackgroundMusicService() { }

    public async Task PlayAsync(string fileName)
    {
        if (_isPlaying && _currentFile == fileName) return;
        Stop();
        var audioManager = AudioManager.Current;
        var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
        _player = audioManager.CreatePlayer(stream);
        _player.Loop = true;
        _player.Volume = _volume;
        _player.Play();
        _isPlaying = true;
        _currentFile = fileName;
    }

    public void Stop()
    {
        if (_player != null)
        {
            _player.Stop();
            _player.Dispose();
            _player = null;
            _isPlaying = false;
        }
    }

    public bool IsPlaying => _isPlaying;

    public async Task ResumeAsync()
    {
        if (!_isPlaying && _currentFile != null)
        {
            var audioManager = AudioManager.Current;
            var stream = await FileSystem.OpenAppPackageFileAsync(_currentFile);
            _player = audioManager.CreatePlayer(stream);
            _player.Loop = true;
            _player.Volume = _volume;
            _player.Play();
            _isPlaying = true;
        }
    }

    public async Task ResumeLastMusicIfAny()
    {
        await ResumeAsync();
    }

    public double Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            Preferences.Set("MusicVolume", value);
            if (_player != null)
                _player.Volume = value;
        }
    }
}