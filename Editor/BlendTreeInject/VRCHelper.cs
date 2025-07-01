using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3A.Editor;

namespace BlendTreeInject
{
    public class VRCHelper
    {
        public Dictionary<string, VRCAvatarDescriptor> validAvatars = new Dictionary<string, VRCAvatarDescriptor>();

        public VRCHelper() 
        {
            GetValidAvatars();
        }

        public void GetValidAvatars()
        {
            validAvatars.Clear();
            VRCAvatarDescriptor[] descriptors = Object.FindObjectsByType<VRCAvatarDescriptor>(FindObjectsSortMode.None);

            foreach (VRCAvatarDescriptor descriptor in descriptors)
            {
                if (isValid(descriptor)) validAvatars.Add(descriptor.gameObject.name, descriptor);
            }

        }

        public bool isValid(VRCAvatarDescriptor descriptor)
        {
            bool isValid = false;

            if (descriptor != null && descriptor.gameObject.activeInHierarchy)
            {
                foreach (VRCAvatarDescriptor.CustomAnimLayer layer in descriptor.baseAnimationLayers)
                {
                    if (layer.type == VRCAvatarDescriptor.AnimLayerType.FX && !layer.isDefault)
                    {
                        isValid = true;
                    }
                }
            }
            return isValid;
        }

        public List<string> GetKeys()
        {
            List<string> keys = new List<string>();

            foreach (string key in validAvatars.Keys) keys.Add(key);

            return keys;
        }

        public AnimatorController GetFXController(string key)
        {
            VRCAvatarDescriptor descriptor = validAvatars[key];
            foreach (VRCAvatarDescriptor.CustomAnimLayer layer in descriptor.baseAnimationLayers)
            {
                if (layer.type == VRCAvatarDescriptor.AnimLayerType.FX)
                {
                    return (AnimatorController) layer.animatorController;
                }
            }

            return null;

        }
    }
}

