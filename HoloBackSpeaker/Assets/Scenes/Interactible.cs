using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

/// <summary>
/// The Interactible class flags a Game Object as being "Interactible".
/// Determines what happens when an Interactible is being gazed at.
/// </summary>
public class Interactible : MonoBehaviour {
    [Tooltip("Audio clip to play when interacting with this hologram.")]
    public AudioClip TargetFeedbackSound;
    private AudioSource audioSource;

    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    private Material[] defaultMaterials;

    void Start() {
        defaultMaterials = GetComponent<Renderer>().materials;

        // Add a BoxCollider if the interactible does not contain one.
        Collider collider = GetComponentInChildren<Collider>();
        if ( collider == null ) {
            gameObject.AddComponent<BoxCollider>();
        }

        EnableAudioHapticFeedback();

        keywords.Add("Play Music", () =>
        {
            // Call the OnReset method on every descendant object.
            gameObject.SendMessageUpwards("makeAPIRequest", "play");
        });

        keywords.Add("Volume Up", () =>
        {
            // Call the OnReset method on every descendant object.
            gameObject.SendMessageUpwards("makeAPIRequest", "volumeup");
        });

        keywords.Add("Volume Down", () =>
        {
            // Call the OnReset method on every descendant object.
            gameObject.SendMessageUpwards("makeAPIRequest", "volumedown");
        });

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

    }

    //Added Voice Keyword Recognizing and starts the recognizing function.
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    private void EnableAudioHapticFeedback() {
        // If this hologram has an audio clip, add an AudioSource with this clip.
        if ( TargetFeedbackSound != null ) {
            audioSource = GetComponent<AudioSource>();
            if ( audioSource == null ) {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = TargetFeedbackSound;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;
            audioSource.dopplerLevel = 0;
        }
    }

    /* TODO: DEVELOPER CODING EXERCISE 2.d */

    void GazeEntered() {
        Debug.Log("Gaze Entered");
        for ( int i = 0; i < defaultMaterials.Length; i++ ) {
            // 2.d: Uncomment the below line to highlight the material when gaze enters.
            defaultMaterials[i].SetFloat("_Highlight", .25f);
        }
    }

    void GazeExited() {
        Debug.Log("Gaze Exited");
        for ( int i = 0; i < defaultMaterials.Length; i++ ) {
            // 2.d: Uncomment the below line to remove highlight on material when gaze exits.
            defaultMaterials[i].SetFloat("_Highlight", 0f);
        }
    }

    void OnSelect() {
        for ( int i = 0; i < defaultMaterials.Length; i++ ) {
            defaultMaterials[i].SetFloat("_Highlight", .5f);
        }

        // Play the audioSource feedback when we gaze and select a hologram.
        if ( audioSource != null && !audioSource.isPlaying ) {
            audioSource.Play();
        }

        /* TODO: DEVELOPER CODING EXERCISE 6.a */
        // 6.a: Handle the OnSelect by sending a PerformTagAlong message.
        Debug.Log("I am: " + gameObject.name);
        Debug.Log("And I Pooted");

        // EXAMPLE: NavButton btn = gameObject.GetComponentInParent<NavButton>();
        gameObject.SendMessageUpwards("makeAPIRequest", gameObject.name);

        
    
    }
}