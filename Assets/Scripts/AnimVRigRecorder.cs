using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Threading;

public class AnimVRigRecorder : MonoBehaviour
{
    [Tooltip("The imported Model Rig to be recorded.")]
    public GameObject originalRig;

    private GameObjectRecorder _recorder;
    private AnimationClip _currentClip;
    private Thread _recordingThread;

    [SerializeField]
    private string _saveFolderLocation = "Assets/Animations/";
    [SerializeField]
    private string _clipName;
    [SerializeField]
    private float _frameRate = 30f;

    private bool _isRecording = false;

    public void StartRecording()
    {
        if (_isRecording)
        {
            Debug.LogWarning("Recording is already in progress.");
            return;
        }

        if (_recorder == null)
        {
            _recorder = new GameObjectRecorder(gameObject);
            _recorder.BindComponentsOfType<Transform>(originalRig, true);
        }

        if (_currentClip == null)
        {
            _currentClip = new AnimationClip() { frameRate = _frameRate };
            if (!string.IsNullOrEmpty(_clipName))
            {
                _currentClip.name = _clipName;
            }
            else
            {
                _currentClip.name = gameObject.name + "_Animation";
            }
        }

        _isRecording = true;
        _recordingThread = new Thread(RecordAnimation);
        _recordingThread.Start();
    }

    private void RecordAnimation()
    {
        while (_isRecording)
        {
            _recorder.TakeSnapshot(Time.deltaTime);
            Thread.Sleep((int)(1000 / _frameRate)); // Match the frame rate delay
        }
    }

    public void StopRecording()
    {
        if (!_isRecording)
        {
            Debug.LogWarning("Recording has not been started.");
            return;
        }

        _isRecording = false;
        _recordingThread.Join(); // Wait for the recording thread to finish

        SaveClip();
        Debug.Log("Recording stopped and file saved.");
    }

    private void SaveClip()
    {
        _recorder.SaveToClip(_currentClip);
        string path = _saveFolderLocation + _currentClip.name + ".anim";
        AssetDatabase.CreateAsset(_currentClip, path);
        AssetDatabase.SaveAssets();
        Debug.Log("Animation saved to: " + path);
    }

    private void OnDisable()
    {
        if (_isRecording)
        {
            StopRecording();
        }
    }
}
