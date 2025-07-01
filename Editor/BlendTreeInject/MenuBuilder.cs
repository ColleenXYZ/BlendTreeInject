using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Animations;
using System.Linq;



namespace BlendTreeInject
{
    public enum AnimationType
    {
        Direct,
        Smooth
    }
    public struct AnimationItem
    {
        public string parameter;
        public AnimationClip clip;
        public AnimationType type;
    }
    public struct Settings
    {
        public string targetLayer;
    }
    public class MenuBuilder
    {
        VisualElement root;
        VisualElement selected;
        VisualElement animationMenu;
        Foldout settingsMenu;
        AnimatorController animatorController;
        List<string> validParams;

        private static readonly string[] hiddenParams =
        {
            "Smoothing",
            "Blend"
        };

        public MenuBuilder(VisualElement root)
        {
            this.root = root;
            this.validParams = new List<string>();
        }

        public void CreateAnimationMenu()
        {
            if (animationMenu != null) return;

            animationMenu = new VisualElement 
            { 
                name = "animationMenu",
            };

            ScrollView animations = new ScrollView
            {
                name = "animations",         
            };
            animationMenu.Add(animations);

            VisualElement buttonConatiner = new VisualElement();
            buttonConatiner.style.flexDirection = FlexDirection.Row;

            Button add = new Button
            {
                name = "add",
                text = "Add",
            };
            add.clicked += () => animations.Add(CreateAnimationMenuItem());
            buttonConatiner.Add(add);

            Button remove = new Button
            {
                name = "remove",
                text = "Delete",
            };
            remove.clicked += () => animations.Remove(selected);
            buttonConatiner.Add(remove);

            animationMenu.Add(buttonConatiner);

            root.Add(animationMenu);

        }

        public VisualElement CreateAnimationMenuItem()
        {
            VisualElement menuItem = new VisualElement();

            DropdownField animParams = new DropdownField
            {
                name = "parameter",
                label = "Parameter",
                choices = validParams,
            };
            menuItem.Add(animParams);


            ObjectField animation = new ObjectField
            {
                name =  "animation",
                label = "Animation",
                objectType = typeof(AnimationClip),
                allowSceneObjects = false,
            };
            menuItem.Add(animation);

            List<string> options = new List<string>();
            options.Add("Direct");
            options.Add("Smooth");
            RadioButtonGroup type = new RadioButtonGroup
            {
                name = "type",
                label = "Type",
                choices = options,
                value = 0,
            };
            menuItem.Add(type);
            menuItem.RegisterCallback<ClickEvent>((evt) => {
                if (selected != null) SetSelectedColor(selected, false);
                selected = menuItem; 
                SetSelectedColor(selected, true);
            });

            return menuItem;
        }

        public void RegisterController(AnimatorController controller)
        {
            if (animatorController != null) UnregisterController();
            animatorController = controller;
            SerializedObject obj = new SerializedObject(animatorController);
            SerializedProperty property = obj.FindProperty("m_AnimatorParameters");

            animationMenu.TrackPropertyValue(property, UpdateParameters);
            UpdateParameters(property);
        }

        public void UnregisterController()
        {
            animatorController = null;
            animationMenu.Unbind();
        }

        public void UpdateParameters(SerializedProperty prop)
        {
            validParams.Clear();
            foreach (AnimatorControllerParameter parameter in animatorController.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Float && !parameter.name.Contains("_OUT") && !hiddenParams.Contains(parameter.name)) validParams.Add(parameter.name);   
            }
        }

        public List<AnimationItem> GetAnimationItems()
        {
            List<AnimationItem> items = new List<AnimationItem>();

            ScrollView animations = animationMenu.Q<ScrollView>("animations");

            foreach (VisualElement menuItem in animations.Children())
            {
                AnimationItem item = new AnimationItem();
                item.parameter = menuItem.Q<DropdownField>("parameter").value;
                item.clip = (AnimationClip) menuItem.Q<ObjectField>("animation").value;
                item.type = (AnimationType) menuItem.Q<RadioButtonGroup>("type").value;
                
                items.Add(item);
            }

            return items;
        }
    
        public void CreateSettingMenu()
        {
            if (settingsMenu != null) return;

            settingsMenu = new Foldout
            {
                name = "settings",
                text = "Settings",
                value = false,
            };

            TextField targetLayer = new TextField
            {
                name = "layer",
                label = "Target Layer Name",
                value = "Generated Layer",
            };
            settingsMenu.Add(targetLayer);

            root.Add(settingsMenu);
        }

        public Settings GetSettings()
        {
            Settings settings = new Settings();
            settings.targetLayer = settingsMenu.Q<TextField>("layer").value;

            return settings;
        }

        public void SetSelectedColor(VisualElement element, bool isSelected)
        {
            float colorValue = 0.15f;
            if (!isSelected) colorValue = 0;
            Color color = Color.black;
            color.a = colorValue;
            element.style.backgroundColor = color;
        }
    }
}
