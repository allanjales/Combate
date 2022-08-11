using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
	[Header("Settings Menu")]
	[SerializeField] private GameObject _SettingsMenu;

	[Header("Resolution Control")]
	[SerializeField] private Dropdown _ResolutionDropdown;
	[SerializeField] private Toggle _FullscreenToggle;

	[Header("Volume Control")]
	[SerializeField] private AudioMixer _AudioMixer;

	[Header("Master")]
	[SerializeField] private Slider _MasterVolumeSlider;
	[SerializeField] private Text _MasterVolumeText;

	[Header("Music")]
	[SerializeField] private Slider _MusicVolumeSlider;
	[SerializeField] private Text _MusicVolumeText;

	[Header("Effects")]
	[SerializeField] private Slider _EffectsVolumeSlider;
	[SerializeField] private Text _EffectsVolumeText;

	public static SettingsManager Instance { get; private set; }
	private List<Resolution> _Resolutions = new();
	private Vector2 _Resolution = new Vector2(Screen.width, Screen.height);
	private static bool isGameLaunch = true;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			gameObject.SetActive(false);
			return;
		}
		Instance = this;

		_SettingsMenu.SetActive(false);
		Application.targetFrameRate = Screen.currentResolution.refreshRate;
		DropdownResolutionLoadValues();

		if (Application.platform == RuntimePlatform.Android)
			_ResolutionDropdown.interactable = false;
	}

	private void Update()
	{
		if (!_SettingsMenu.activeSelf)
			return;

		if (_Resolution.x == Screen.width && _Resolution.y == Screen.height)
			return;

		_Resolution.x = Screen.width;
		_Resolution.y = Screen.height;
		DropdownResolutionLoadValues();
	}

	private void DropdownResolutionLoadValues()
	{
		_Resolutions.Clear();
		List<string> DropdownOptions = new();
		string option;
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			option = GetResolutionString(Screen.resolutions[i]);
			if (DropdownOptions.Contains(option))
				continue;
			_Resolutions.Add(Screen.resolutions[i]);
			DropdownOptions.Add(option);
		}
		_Resolutions.Insert(0, new Resolution());
		DropdownOptions.Insert(0, "Personalizado");
		int currentResolutionIndex = DropdownOptions.FindIndex(x => x == GetResolutionString(Screen.width, Screen.height)); ;
		if (currentResolutionIndex < 0) currentResolutionIndex = 0;
		_ResolutionDropdown.ClearOptions();
		_ResolutionDropdown.AddOptions(DropdownOptions);
		_ResolutionDropdown.RefreshShownValue();
		_ResolutionDropdown.value = currentResolutionIndex;
	}

	private string GetResolutionString(Resolution res)
	{
		return $"{res.width} x {res.height}";
	}
	private string GetResolutionString(int width, int height)
	{
		return $"{width} x {height}";
	}

	private void Start()
	{
		LoadSettings(true);
	}

	private void LoadSettings(bool isStartLoad = false)
	{
		//FullScreen
		_FullscreenToggle.isOn = Screen.fullScreen;
		if (PlayerPrefs.HasKey("IsFullScreen"))
			SetFullScreen((PlayerPrefs.GetInt("IsFullScreen") == 1) ? true : false);

		//Volumes
		if (PlayerPrefs.HasKey("MasterVolume"))
			SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
		else
			SetMasterVolume(100);
		if (PlayerPrefs.HasKey("MusicVolume"))
			SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
		else
			SetMusicVolume(20);
		if (PlayerPrefs.HasKey("EffectsVolume"))
			SetEffectsVolume(PlayerPrefs.GetFloat("EffectsVolume"));
		else
			SetEffectsVolume(100);

		if (!(isGameLaunch && isStartLoad))
			return;
		isGameLaunch = false;

		//Resolution
		int loadedResolution = -1;
		if (PlayerPrefs.HasKey("Resolution"))
			loadedResolution = _ResolutionDropdown.options.FindIndex(x => x.text == PlayerPrefs.GetString("Resolution"));
		if (loadedResolution >= 0)
			SetResolution(loadedResolution);
	}

	private void SaveSettings()
	{
		PlayerPrefs.SetString("Resolution", _ResolutionDropdown.options[_ResolutionDropdown.value].text);
		PlayerPrefs.SetInt("IsFullScreen", _FullscreenToggle.isOn ? 1 : 0);
		PlayerPrefs.SetFloat("MasterVolume", _MasterVolumeSlider.value);
		PlayerPrefs.SetFloat("MusicVolume", _MusicVolumeSlider.value);
		PlayerPrefs.SetFloat("EffectsVolume", _EffectsVolumeSlider.value);
	}

	public void OpenSettingsMenu()
	{
		DropdownResolutionLoadValues();
		_SettingsMenu.SetActive(true);
	}

	public void CloseSettingsMenu()
	{
		_SettingsMenu.SetActive(false);
		SaveSettings();
	}

	private void SetVolume(float volume, string audioMixerParam, Slider VolumeSlider, Text ShowText)
	{
		_AudioMixer.SetFloat(audioMixerParam, GetAttenuationFloat(ConvertFromRangeToRange(volume, VolumeSlider.minValue, VolumeSlider.maxValue, 0, 1)));
		VolumeSlider.value = volume;
		ShowText.text = $"{Mathf.Round(volume)}%";

	}

	public void SetMasterVolume(float volume)
	{
		SetVolume(volume, "MasterVolume", _MasterVolumeSlider, _MasterVolumeText);
	}

	public void SetMusicVolume(float volume)
	{
		SetVolume(volume, "MusicVolume", _MusicVolumeSlider, _MusicVolumeText);
	}

	public void SetEffectsVolume(float volume)
	{
		SetVolume(volume, "EffectsVolume", _EffectsVolumeSlider, _EffectsVolumeText);
	}

	private float ConvertFromRangeToRange(float x, float x0, float xf, float y0, float yf)
	{
		return (x - x0) / (xf - x0) * (yf - y0) + y0;
	}

	private float GetAttenuationFloat(float volume)
	{
		if (volume < 0.0001)
			return -80;
		if (volume > 1)
			return 1;
		return Mathf.Log10(volume) * 20;
	}

	public void SetFullScreen(bool isFullScreen)
	{
		_FullscreenToggle.isOn = isFullScreen;
		Screen.fullScreen = isFullScreen;
	}

	public void SetResolution(int resolutionIndex)
	{
		if (_Resolutions[resolutionIndex].width == 0)
			return;
		_ResolutionDropdown.value = resolutionIndex;
		if (Application.platform == RuntimePlatform.Android)
			return;
		Screen.SetResolution(_Resolutions[resolutionIndex].width, _Resolutions[resolutionIndex].height, Screen.fullScreen);
	}
}
