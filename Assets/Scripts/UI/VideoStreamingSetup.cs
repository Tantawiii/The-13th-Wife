using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoStreamingSetup : MonoBehaviour
{
    [Tooltip("Filename only, relative to Assets/StreamingAssets/ (must be a plain ASCII name - no spaces/emoji, since this becomes part of a URL).")]
    [SerializeField] private string fileName;

    private void Awake()
    {
        VideoPlayer player = GetComponent<VideoPlayer>();
        player.source = VideoSource.Url;
        player.url = Application.streamingAssetsPath + "/" + fileName;
        player.Play();
    }
}
