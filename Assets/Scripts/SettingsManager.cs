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

	[Header("Ambient")]
	[SerializeField] private Slider _AmbientVolumeSlider;
	[SerializeField] private Text _AmbientVolumeText;

	[Header("Effects")]
	[SerializeField] private Slider _EffectsVolumeSlider;
	[SerializeField] private Text _EffectsVolumeText;


	public static SettingsManager Instance { get; private set; }
	private List<Resolution> _Resolutions = new();

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

		//Resolution Dropdown
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
		int currentResolutionIndex = DropdownOptions.FindIndex(x => x == GetResolutionString(Screen.currentResolution));
		int playerResolutionPrefs = -1;
		if (PlayerPrefs.HasKey("Resolution"))
			playerResolutionPrefs = DropdownOptions.FindIndex(x => x == PlayerPrefs.GetString("Resolution"));
		if (playerResolutionPrefs >= 0) currentResolutionIndex = playerResolutionPrefs;
		if (currentResolutionIndex < 0) currentResolutionIndex = 0;
		_ResolutionDropdown.ClearOptions();
		_ResolutionDropdown.AddOptions(DropdownOptions);
		_ResolutionDropdown.RefreshShownValue();
		SetResolution(currentResolutionIndex);

		//FullScreen
		_FullscreenToggle.isOn = Screen.fullScreen;
		if (PlayerPrefs.HasKey("IsFullScreen"))
			SetFullScreen((PlayerPrefs.GetInt("IsFullScreen") == 1) ? true : false);
	}

	private string GetResolutionString(Resolution res)
	{
		return $"{res.width} x {res.height}";
	}

	private void Start()
	{
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
		if (PlayerPrefs.HasKey("AmbientVolume"))
			SetAmbientVolume(PlayerPrefs.GetFloat("AmbientVolume"));
		else
			SetAmbientVolume(20);
	}

	public void OpenSettingsMenu()
	{
		_SettingsMenu.SetActive(true);
	}

	public void CloseSettingsMenu()
	{
		_SettingsMenu.SetActive(false);
		PlayerPrefs.SetString("Resolution", _ResolutionDropdown.options[_ResolutionDropdown.value].text);
		PlayerPrefs.SetInt("IsFullScreen", _FullscreenToggle.isOn ? 1 : 0);
		PlayerPrefs.SetFloat("MasterVolume", _MasterVolumeSlider.value);
		PlayerPrefs.SetFloat("MusicVolume", _MusicVolumeSlider.value);
		PlayerPrefs.SetFloat("AmbientVolume", _AmbientVolumeSlider.value);
		PlayerPrefs.SetFloat("EffectsVolume", _EffectsVolumeSlider.value);
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

	public void SetAmbientVolume(float volume)
	{
		SetVolume(volume, "AmbientVolume", _AmbientVolumeSlider, _AmbientVolumeText);
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
		Screen.SetResolution(_Resolutions[resolutionIndex].width, _Resolutions[resolutionIndex].height, Screen.fullScreen);
	}
}
