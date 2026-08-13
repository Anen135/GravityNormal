# Lucy Audio System

## Installation

Copy the `Assets` folder into the root of the Unity project.

After scripts compile, run:

`Tools -> Lucy -> Audio -> Create Audio System`

The installer creates:

- `Assets/Lucy/Features/Audio/Mixer/MainAudioMixer.mixer`
- Mixer groups: `Master`, `Music`, `SFX`, `UI`, `Ambience`
- Exposed parameters:
  - `MasterVolume`
  - `MusicVolume`
  - `SFXVolume`
  - `UIVolume`
  - `AmbienceVolume`
- `[AudioSystem]` in the active scene
- `Assets/Lucy/Features/Audio/Prefabs/AudioSystem.prefab`

## Usage

```csharp
AudioSystem.Instance.PlayMusic(musicClip);
AudioSystem.Instance.PlayAmbience(spaceHum);
AudioSystem.Instance.PlaySFX(laserClip);
AudioSystem.Instance.PlaySFXAt(explosionClip, transform.position);
AudioSystem.Instance.PlayUI(clickClip);

AudioSystem.Instance.SetMasterVolume(0.8f);
AudioSystem.Instance.SetMusicVolume(0.5f);
AudioSystem.Instance.SetSFXVolume(1f);
AudioSystem.Instance.SetUIVolume(0.75f);
AudioSystem.Instance.SetAmbienceVolume(0.6f);
```

Volume values are linear from `0` to `1`; AudioSystem converts them to dB for the mixer and optionally persists them through PlayerPrefs.

## Notes

The editor installer uses reflection only to create AudioMixer groups and exposed parameters because Unity keeps the mixer authoring controller API internal. Runtime code uses public `UnityEngine.Audio` API only.
