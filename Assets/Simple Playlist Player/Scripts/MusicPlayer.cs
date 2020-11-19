using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))] //an AudioSource Component will be added autoatically to
//the game object that has this script attached to it
[System.Serializable] //making the class serializable
public class MusicPlayer : MonoBehaviour {

	[Header("Text")]
	public Text trackText; //this text object will be updated so as to show the player which track is playing	

	[Header("Options")]
	public bool playRandomClips; //if selected the clips in the playlist will play in random order
	public bool messageOnPlaying; //if selected when a clip begins playing the trackText object will 
	//show the name of that clip

	private AudioSource audioSource; //the AudioSource that will play the clips
	private bool isPlaying; //variable indicating if any clip is playing
	private int playlistIndex = -1;	//index indicating the position of the track currently playing

	[System.Serializable] //we make this struct Serializable to be able to edit its values in the inspector
	public struct MusicTracks // custom datatype that will represent a MusicTrack
	{
		public bool enabled; //if false it will be ignored by the playlist player (it will be disabled)
		public AudioClip track; //the audio clip of the track
	}

	[Header("Your Playlist")]
	public List<MusicTracks> Playlist = new List<MusicTracks>(); //playlist that we can edit in the inspector 
	//adding our own tracks
	private List<MusicTracks> enabledTracks = new List<MusicTracks>(); //this list will contain the tracks we 
	//have not disabled

	void Start () {
		DontDestroyOnLoad(gameObject); //the object won't be destroyed when a new level loads
		trackText.gameObject.transform.parent.gameObject.SetActive(false); //deactivating the the text label
		GetActiveTracks(); //get all the active tracks
		audioSource = GetComponent<AudioSource>(); //initialize the audio source variable
		StartCoroutine(playTrack(enabledTracks)); //begin playing tracks
	}

	void Update () {
		if (!isPlaying) //if no track is playing our will begin playing one
		{
			StartCoroutine(playTrack(enabledTracks));
		}
	}

	void GetActiveTracks() //this function reads through our playlist and adds any enabled tracks to a new list 
	{
		enabledTracks.Clear();
		for (int i = 0; i < Playlist.Count; i++)
		{
			if (Playlist[i].enabled)
			{
				enabledTracks.Add(Playlist[i]);
			}
		}
	} //Note: if you want to give the player the ability to change active tracks through the game you should call
	//this function to update the selected by the player tracks

	void UpdateIndex() //this function will determine the index of the track to play next
	{
		if (playRandomClips) //if tracks are playing at a random order the index will just contain a random value
		{
			playlistIndex = Random.Range(0, enabledTracks.Count);
		}
		else //otherwise we increase the index by one and if it is out of the list bounds we reset it to 0
		{
			playlistIndex++;
			if (playlistIndex >= enabledTracks.Count)
			{
				playlistIndex = 0;
			}
		}
	}

	public IEnumerator playTrack (List<MusicTracks> tracks) //this IEnumerator will play a track from a given list of tracks
	{
		isPlaying = true; //we send a message showing that a track is playing
		UpdateIndex(); //get the index of the track to play next
		audioSource.clip = tracks[playlistIndex].track; //set the AudioSource clip that will play
		audioSource.Play(); //begin playing the track
		if (messageOnPlaying) //this defines how the trackText will be updated if this option is enabled
		{
			trackText.gameObject.transform.parent.gameObject.SetActive(true); //activate the text
			trackText.text = audioSource.clip.name; //set the test value to the current track's name
			yield return new WaitForSeconds(2.5f); //wait for a specoific amount of time
			trackText.gameObject.transform.parent.gameObject.SetActive(false); //deactivate the text
		}
		yield return new WaitForSeconds(audioSource.clip.length); //wait for the clip to finish
		isPlaying = false; //we send a message showing that the current track is finished and we need a new track to begin playing
	}
}