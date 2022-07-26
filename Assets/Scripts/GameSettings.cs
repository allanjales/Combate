using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
	[Header("Settings Menu")]
	[SerializeField] private GameObject _SettingsMenu;

	[Header("Resolution Control")]
	[SerializeField] private Dropdown _ResolutionDropdown;
	[SerializeField] private Toggle _FullscreenToggle;

	[Header("Volume Control")]
	[SerializeField] private AudioMixer _AudioMixer;
	[SerializeField] private Slider _VolumeSlider;
	[SerializeField] private Text _VolumeText;


	public static GameSettings Instance { get; private set; }
	private Resolution[] _Resolutions;

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
		_Resolutions = Screen.resolutions;
		List<string> DropdownOptions = new();
		string option;
		int currentResolutionIndex = -1;
		for (int i = 0; i < _Resolutions.Length; i++)
		{
			option = $"{_Resolutions[i].width} x {_Resolutions[i].height}";
			DropdownOptions.Add(option);

			if (_Resolutions[i].width == Screen.currentResolution.width &&
				_Resolutions[i].height == Screen.currentResolution.height && currentResolutionIndex == -1)
				currentResolutionIndex = i;

			if (PlayerPrefs.HasKey("Resolution") && PlayerPrefs.GetString("Resolution") == option)
				currentResolutionIndex = i;
		}
		if (currentResolutionIndex < 0) currentResolutionIndex = 0;
		_ResolutionDropdown.ClearOptions();
		_ResolutionDropdown.AddOptions(DropdownOptions);
		_ResolutionDropdown.value = currentResolutionIndex;
		_ResolutionDropdown.RefreshShownValue();

		//FullScreen
		_FullscreenToggle.isOn = Screen.fullScreen;
		if (PlayerPrefs.HasKey("IsFullScreen"))
		{
			_FullscreenToggle.isOn = (PlayerPrefs.GetInt("IsFullScreen") == 1) ? true : false;
		}

		//Volume
		if (PlayerPrefs.HasKey("IsFullScreen"))
		{
			SetVolume(PlayerPrefs.GetFloat("Volume"));
		}
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
		PlayerPrefs.SetFloat("Volume", _VolumeSlider.value);
	}

	public void SetVolume(float volume)
	{
		_AudioMixer.SetFloat("VolumeGeral", GetAttenuationFloat(volume, _VolumeSlider.minValue, _VolumeSlider.maxValue));
		_VolumeSlider.value = volume;
		_VolumeText.text = $"{Mathf.Round(volume)}%";
	}

	private float GetAttenuationFloat(float volume, float volumeMin, float volumeMax)
	{
		return ConvertFromRangeToRange(volume, volumeMin, volumeMax, -80, 0);
	}

	private float ConvertFromRangeToRange(float x, float x0, float xf, float y0, float yf)
	{
		return (x - x0) / (xf - x0) * (yf - y0) + y0;
	}

	public void SetFullScreenMode(bool iSFullScreen)
	{
		Screen.fullScreen = iSFullScreen;
	}

	public void SetResolution(int resolutionIndex)
	{
		Screen.SetResolution(_Resolutions[resolutionIndex].width, _Resolutions[resolutionIndex].height, Screen.fullScreen);
	}
}
