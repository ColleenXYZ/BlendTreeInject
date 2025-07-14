# BlendTreeInject

## Usage

This plugin is currently a little half-baked, but all the core functionality is there. As it currently stands there isn't much in the form of error checking. Nothing *too bad* should happen, but there might be some weirdness that'll be ironed out in the future. A fair **WARNING**, when adding a smoothed animation, the passed in animations will be modified to include an AAP called `{param}_OUT`. Its reccommended to create a copy of the animation file before running the plugin. I'd also advise to do the same for the Animation Controller just in case anything goes wrong.

**How To:**

1. Import the Unity package into your project.
2. Open the plugin menu by going to Window->BlendTreeInject
3. Select the target avatar from the drop down. (Only avatars with a custom FX will appear here.)
4. Click Add to add menu item for an animation.
5. Drag over the animation to be added and select the target parameter.
6. Select Smooth or Direct. A "Smoothed" animation will be added to an [exponential smoothing tree](https://vrc.school/docs/Other/Advanced-BlendTrees#bffb17cfe885400fb44b9d2f9ff70e94). A "Direct" animation plays instantly.
7. Click the add animations button and everything *should* be added correctly.

If a layer containing a blend tree is deleted, the clean up blend tree button will delete those loose blend trees from the Animation Controller since Unity doen't clean them up properly. The name of the generated layer can be changed in the drop down menu. The plugin uses the name of the trees to identify them, so I'd advise against changing any of the trees name. Animation menu items can be deleted by clicking on them and then clicking Delete.  
