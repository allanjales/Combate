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
		int currentResolutionIndex = -1;
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			option = $"{Screen.resolutions[i].width} x {Screen.resolutions[i].height}";
			if (DropdownOptions.Contains(option))
				continue;
			_Resolutions.Add(Screen.resolutions[i]);
			DropdownOptions.Add(option);

			// If this is current resolution
			if (Screen.resolutions[i].width == Screen.currentResolution.width &&
				Screen.resolutions[i].height == Screen.currentResolution.height && currentResolutionIndex == -1)
				currentResolutionIndex = i;

			//If can load resolution
			if (PlayerPrefs.HasKey("Resolution") && PlayerPrefs.GetString("Resolution") == option)
				currentResolutionIndex = i;
		}
		_Resolutions.Insert(0, new Resolution());
		DropdownOptions.Insert(0, "Custom");
		currentResolutionIndex++;
		_ResolutionDropdown.ClearOptions();
		_ResolutionDropdown.AddOptions(DropdownOptions);
		_ResolutionDropdown.value = currentResolutionIndex;
		_ResolutionDropdown.RefreshShownValue();
		SetResolution(currentResolutionIndex);

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
		_AudioMixer.SetFloat("VolumeMaster", GetAttenuationFloat(ConvertFromRangeToRange(volume, _VolumeSlider.minValue, _VolumeSlider.maxValue, 0, 1)));
		_VolumeSlider.value = volume;
		_VolumeText.text = $"{Mathf.Round(volume)}%";
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

	public void SetFullScreenMode(bool iSFullScreen)
	{
		Screen.fullScreen = iSFullScreen;
	}

	public void SetResolution(int resolutionIndex)
	{
		if (_Resolutions[resolutionIndex].width == 0)
			return;
		Screen.SetResolution(_Resolutions[resolutionIndex].width, _Resolutions[resolutionIndex].height, Screen.fullScreen);
	}
}
