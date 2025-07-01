using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;

namespace BlendTreeInject
{
    public class BlendTreeManager
    {
        BlendTree root;

        public BlendTreeManager(BlendTree tree)
        {
            root = tree;
        }

        public void AddMotionSmooth(Motion motion, string param)
        {
            BlendTree smoothingTree, target, output;
            
            smoothingTree = GetByTreeName("GeneratedSmoothingTree");

            if (smoothingTree == null)
            {
                smoothingTree = CreateChildTree("Blend", BlendTreeType.Simple1D, "GeneratedSmoothingTree");
                smoothingTree.blendParameter = "Smoothing";
            }

            target = GetByTreeName("TargetOutput", smoothingTree);

            if (target == null)
            {
                target = smoothingTree.CreateBlendTreeChild(0f);
                target.blendType = BlendTreeType.Direct;
                target.name = "TargetOutput";
            }

            output = GetByTreeName("Output", smoothingTree);

            if (output == null)
            {
                output = smoothingTree.CreateBlendTreeChild(1f);
                output.blendType = BlendTreeType.Direct;
                output.name = "Output";
            }

            target.AddChild(motion);
            SetParam(param, target);

            output.AddChild(motion);
            SetParam(param + "_OUT", output);
            
        }

        public void AddMotionDirect(Motion motion, string param)
        {
            root.AddChild(motion);
            SetParam(param);
        }

        public BlendTree GetByTreeName(string name, BlendTree tree = null)
        {
            if (tree == null) tree = root;

            foreach (ChildMotion child in tree.children)
            {
                if (child.motion.name == name && child.motion is BlendTree) return (BlendTree) child.motion;
            }

            return null;
        }

        public BlendTree CreateChildTree(string param, BlendTreeType type, string name, BlendTree tree = null)
        {
            if (tree == null) tree = root;

            BlendTree child = tree.CreateBlendTreeChild(0f);
            child.blendType = type;
            child.name = name;
            SetParam(param, tree);
            return child;
        }


        public void SetParam(string param, BlendTree tree = null)
        {
            if (tree == null) tree = root;

            ChildMotion[] children = tree.children;
            ChildMotion motion = children[children.Length - 1];
            motion.directBlendParameter = param;
            children[children.Length - 1] = motion;
            tree.children = children;
        }
    }
}

