using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;

namespace BlendTreeInject
{
    public class ClipEditor
    {
        string rootPath = "Assets";
        public void MakeDefaultClip(GameObject root, AnimationClip clip)
        {
            AnimationClip defaultClip = new AnimationClip();
            AssetDatabase.CreateAsset(defaultClip, $"{rootPath}/{clip.name}_defaults.anim");

            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip)) 
            { 
                AnimationUtility.GetFloatValue(root, binding, out float value);
                AnimationCurve curve = new AnimationCurve(new Keyframe(0f, value));
                AnimationUtility.SetEditorCurve(defaultClip, binding, curve);
            }
        }

        public void AddAAP(string param, AnimationClip clip)
        {
            EditorCurveBinding binding = EditorCurveBinding.FloatCurve("", typeof(Animator), param);
            AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 1f));
            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }
    }

}
