using UnityEngine;
using System.Collections;

public class MixLevels : MonoBehaviour {

	// AudioMixer replaced with AudioSource for Unity 2018 compatibility
	public AudioSource sfxSource;
	public AudioSource musicSource;

	public void SetSfxLvl(float sfxLvl)
	{
		if (sfxSource != null)
			sfxSource.volume = sfxLvl;
	}

	public void SetMusicLvl(float musicLvl)
	{
		if (musicSource != null)
			musicSource.volume = musicLvl;
	}
}
