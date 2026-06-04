using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

public class Timeline_EntitySetter : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private string targetTrackName = "CharacterTrack";

    void Start()
    {
        // 1. Instantiate the character into your scene
        GameObject spawnedCharacter = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
        Animator characterAnimator = spawnedCharacter.GetComponent<Animator>();

        if (characterAnimator == null)
        {
            Debug.LogError("The instantiated character is missing an Animator component!");
            return;
        }

        // 2. Get the Timeline Asset
        TimelineAsset timelineAsset = playableDirector.playableAsset as TimelineAsset;

        if (timelineAsset != null)
        {
            // 3. Find the specific Animation Track by its exact name
            TrackAsset targetTrack = timelineAsset.GetOutputTracks()
                .FirstOrDefault(track => track.name == targetTrackName);

            if (targetTrack != null)
            {
                // 4. Bind the instantiated character's Animator to the track
                playableDirector.SetGenericBinding(targetTrack, characterAnimator);
                
                // 5. Play the timeline
                playableDirector.Play();
            }
            else
            {
                Debug.LogError($"Could not find a Timeline track named '{targetTrackName}'");
            }
        }
    }
}
