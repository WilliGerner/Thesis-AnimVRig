using UnityEngine;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class AnimationClipSerializer
{
#if UNITY_EDITOR
    public static void SaveAnimationClip(AnimationClip clip, string path)
    {
        // Convert the AnimationClip to JSON and save it to a file
        string json = JsonUtility.ToJson(new AnimationClipData(clip));
        File.WriteAllText(path, json);
        Debug.Log("AnimationClip saved to: " + path);
    }

    public static AnimationClip LoadAnimationClip(string path)
    {
        // Load the JSON from the file and convert it back to an AnimationClip
        string json = File.ReadAllText(path);
        AnimationClipData data = JsonUtility.FromJson<AnimationClipData>(json);
        return data.ToAnimationClip();
    }

    [System.Serializable]
    private class AnimationClipData
    {
        public string name;
        public float frameRate;
        public WrapMode wrapMode;
        public List<CurveData> curves = new List<CurveData>();

        public AnimationClipData(AnimationClip clip)
        {
            name = clip.name;
            frameRate = clip.frameRate;
            wrapMode = clip.wrapMode;

            // Serialize all the curves in the AnimationClip
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                curves.Add(new CurveData(binding, curve));
            }
        }

        public AnimationClip ToAnimationClip()
        {
            AnimationClip clip = new AnimationClip
            {
                name = name,
                frameRate = frameRate,
                wrapMode = wrapMode
            };

            // Deserialize the curves and add them back to the AnimationClip
            foreach (CurveData curveData in curves)
            {
                AnimationUtility.SetEditorCurve(clip, curveData.binding, curveData.ToAnimationCurve());
            }

            return clip;
        }
    }

    [System.Serializable]
    private class CurveData
    {
        public string path;
        public string propertyName;
        public string type;
        public KeyframeData[] keyframes;

        public CurveData(EditorCurveBinding binding, AnimationCurve curve)
        {
            path = binding.path;
            propertyName = binding.propertyName;
            type = binding.type.AssemblyQualifiedName;

            keyframes = new KeyframeData[curve.keys.Length];
            for (int i = 0; i < curve.keys.Length; i++)
            {
                keyframes[i] = new KeyframeData(curve.keys[i]);
            }
        }

        public EditorCurveBinding binding
        {
            get
            {
                return new EditorCurveBinding
                {
                    path = path,
                    propertyName = propertyName,
                    type = System.Type.GetType(type)
                };
            }
        }

        public AnimationCurve ToAnimationCurve()
        {
            Keyframe[] keyframesArray = new Keyframe[keyframes.Length];
            for (int i = 0; i < keyframes.Length; i++)
            {
                keyframesArray[i] = keyframes[i].ToKeyframe();
            }
            return new AnimationCurve(keyframesArray);
        }
    }

    [System.Serializable]
    private class KeyframeData
    {
        public float time;
        public float value;
        public float inTangent;
        public float outTangent;
        public int tangentMode;

        public KeyframeData(Keyframe keyframe)
        {
            time = keyframe.time;
            value = keyframe.value;
            inTangent = keyframe.inTangent;
            outTangent = keyframe.outTangent;
            tangentMode = keyframe.tangentMode;
        }

        public Keyframe ToKeyframe()
        {
            return new Keyframe(time, value, inTangent, outTangent) { tangentMode = tangentMode };
        }
    }
#endif
}
