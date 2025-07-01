using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Animations;
using UnityEngine.Assertions.Must;

namespace BlendTreeInject
{
    public class Window : EditorWindow
    {
        MenuBuilder menu;
        ClipEditor clipEditor;
        VRCHelper vrc;
        ControllerManager controllerManager;

        [MenuItem("Window/BlendTreeInject")]
        public static void ShowWindow()
        {
            Window wnd = GetWindow<Window>();
            wnd.titleContent = new GUIContent("BlendTreeInject");
        }
        public void CreateGUI()
        {
            menu = new MenuBuilder(rootVisualElement);
            clipEditor = new ClipEditor();
            vrc = new VRCHelper();
            controllerManager = new ControllerManager();

            menu.CreateAnimationMenu();

            DropdownField avatars = new DropdownField
            {
                name =  "avatars",
                label = "Selected Avatar",
                choices = vrc.GetKeys(),
            };
            if (avatars.choices.Count > 0)
            {
                avatars.index = 0;
                UpdateController(avatars.value);
            }
            avatars.RegisterValueChangedCallback(avatar => UpdateController(avatar.newValue));
            rootVisualElement.Add(avatars);

            Button refresh = new Button
            {
                name = "refresh",
                text = "Refresh Avatar List",
            };
            refresh.clicked += Refresh;
            rootVisualElement.Add(refresh);

            Button cleanTrees = new Button
            {
                name = "cleanTrees",
                text = "Clean Up Loose Trees",
            };
            cleanTrees.clicked += controllerManager.CleanBlendTrees;
            rootVisualElement.Add(cleanTrees);

            Button addToTree = new Button
            {
                name = "addToTree",
                text = "Add Animations To Blend Tree",
            };
            addToTree.clicked += BuildTree;
            rootVisualElement.Add(addToTree);

            menu.CreateSettingMenu();
        }

        public void Refresh()
        {
            vrc.GetValidAvatars();
            rootVisualElement.Q<DropdownField>("avatars").choices = vrc.GetKeys();
        }

        public void UpdateController(string value)
        {
            AnimatorController controller = vrc.GetFXController(value);
            menu.RegisterController(controller);
            controllerManager.controller = controller;
        }

        public void BuildTree()
        {
            Settings settings = menu.GetSettings();

            BlendTree tree = controllerManager.CreateBlendTree(settings.targetLayer);
            BlendTreeManager treeManager = new BlendTreeManager(tree);
            
            foreach (AnimationItem item in menu.GetAnimationItems())
            {
                if (item.type == AnimationType.Direct) treeManager.AddMotionDirect(item.clip, item.parameter);
                if (item.type == AnimationType.Smooth)
                {
                    controllerManager.AddParameter(item.parameter + "_OUT");
                    controllerManager.AddParameter("Smoothing");
                    clipEditor.AddAAP(item.parameter + "_OUT", item.clip);
                    treeManager.AddMotionSmooth(item.clip, item.parameter);
                }
                AssetDatabase.SaveAssets();
            }
        }
    }
}
