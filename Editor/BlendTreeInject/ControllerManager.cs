using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace BlendTreeInject
{
    public class ControllerManager
    {
        public AnimatorController controller;

        public BlendTree CreateBlendTree(string targetLayer)
        {
            int layerIndex = GetLayerIndex(targetLayer);
            AnimatorState defaultState = controller.layers[layerIndex].stateMachine.defaultState;

            if (defaultState != null)
            {
                if (defaultState.motion is BlendTree) return (BlendTree) defaultState.motion;
                return null;
            }

            controller.CreateBlendTreeInController(targetLayer, out BlendTree tree, layerIndex);
            tree.blendType = BlendTreeType.Direct;
            tree.hideFlags = HideFlags.HideInHierarchy;
            return tree;
        }

        public int GetLayerIndex(string targetLayer)
        {
            AnimatorControllerLayer[] layers = controller.layers;
            for (int i = 0; i < layers.Length; i++) 
            { 
                if (layers[i].name == targetLayer) return i;
            }

            CreateLayer(targetLayer);

            return controller.layers.Length - 1;
        }

        public void CreateLayer(string name)
        {
            controller.AddLayer(name);
            AnimatorControllerLayer[] layers = controller.layers;
            layers[layers.Length - 1].defaultWeight = 1.0f;
            controller.layers = layers;
        }

        public void AddParameter(string name)
        {
            foreach (AnimatorControllerParameter parameter in controller.parameters)
            {
                if (parameter.name == name) return;
            }

            controller.AddParameter(name, AnimatorControllerParameterType.Float);
        }

        public void CleanBlendTrees()
        {
            List<BlendTree> validTrees = new List<BlendTree>();

            foreach (AnimatorControllerLayer layer in controller.layers)
            {
                foreach (ChildAnimatorState childState in layer.stateMachine.states)
                {
                    if (childState.state.motion is BlendTree)
                    {
                        BlendTree blendTree = (BlendTree) childState.state.motion;
                        validTrees.Add(blendTree);
                        FindChildTrees(blendTree, validTrees);
                    }
                }
            }

            string path = AssetDatabase.GetAssetPath(controller);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            int count = 0;
            
            foreach (Object asset in assets)
            {
                if (asset is BlendTree && !validTrees.Contains((BlendTree) asset))
                {
                    AssetDatabase.RemoveObjectFromAsset(asset);
                    count++;
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"{count} Blend Trees were removed");
        }

        private void FindChildTrees(BlendTree tree, List<BlendTree> validTrees)
        {
            foreach (ChildMotion child in tree.children)
            {
                if (child.motion is BlendTree)
                {
                    BlendTree blendTree = (BlendTree)child.motion;
                    validTrees.Add(blendTree);
                    FindChildTrees(blendTree, validTrees);
                }
            }
        }
    }
}

