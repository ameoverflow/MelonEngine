using Chroma.Audio;
using Chroma.Audio.Sources;
using Chroma.MemoryManagement;

namespace MelonEngine;

public class InstancedSound : DisposableResource
{
    private static Queue<Sound> _expiredInstances = new();
    private static List<Sound> _liveInstances = new();
    private Stream _template = new MemoryStream();

    public static int LiveInstanceCount => _liveInstances.Count;

    public InstancedSound(string filePath)
        : this(new FileStream(filePath, FileMode.Open))
    {
    }

    public InstancedSound(Stream stream)
    {
        EngineCore.AudioOutput.AudioSourceFinished += OnAudioSourceFinished;
        stream.CopyTo(_template);
        _template.Seek(0, SeekOrigin.Begin);
    }

    public static void Update()
    {
        while (_expiredInstances.Any())
        {
            var instance = _expiredInstances.Dequeue();

            if (_liveInstances.Remove(instance))
                instance.Dispose();
        }
    }

    public void PlayInstance()
    {
        if (_template == null)
            return;

        using (var ms = new MemoryStream())
        {
            _template.CopyTo(ms);
            _template.Seek(0, SeekOrigin.Begin);

            var snd = new Sound(ms);
            _liveInstances.Add(snd);

            snd.Play();
        }
    }

    protected override void FreeManagedResources()
    {
        EngineCore.AudioOutput.AudioSourceFinished -= OnAudioSourceFinished;
        _template.Dispose();
    }

    private void OnAudioSourceFinished(object sender, AudioSourceEventArgs e)
    {
        if (e.IsLooping)
            return;

        if (e.Source is Sound snd)
        {
            _expiredInstances.Enqueue(snd);
        }
    }
}