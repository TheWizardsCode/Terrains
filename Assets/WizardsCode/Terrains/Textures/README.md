# Open Source Textures for Unity 3D

This is a library of open source assets that you can use in early prototypes of your games and assets. They are all under a license that means you can use them in any environment and without restriction. This includes redistribution as individual assets. This means you can use them in demo scenes for your assets - something that is normally prohibitted. All you are required to do is provide a LICENSE and a NOTICE file in your project source.

The goal of this project is not to provide AAA quality assets but instead provide a library of assets that are "good enough" for game prototypes and demo scenes for your own published assets. Though you will be surprised at the quality of some of the models here.

# Use

  1. Import this package into your project.
  2. Open the `OpenSourceModels/Scenes/TexturesCatalog` scene
  3. Use the UI to filter the available models and select one for display (when viewing materials you can view the matieral in more detail by flipping to an inspector view with the 'F' key)
  4. When you find a model you like it will have been pinged in the Project view - go ahead and use it in your project as you would use any other model - but without license restrictions (see `NOTICE File` below)
  
## Installing Via Package Manager

Modify your manifest.json file found at /PROJECTNAME/Packages/manifest.json by including the following line

```json
{
	"dependencies": {
		...
		"org.3dtbd.textures": "https://github.com/3dtbd/Textures.git#release/stable",
		...
  }
}
```

## NOTICE file

When you use assets from this project you must copy the NOTICE and LICENSE files that can be found in the root of this project into your project. The NOTICE file must be placed in the root of your project while the LICENSE file can be placed anywhere convenient. 

If you already have NOTICE file simply append the contents of our NOTICE to yours.

In order to avoid confusion over which assets are covered by our LICENSE we recommend keeping all the assets you use in a directory called `3dtbd/OpenSource` or similar. You can then place the LICENSE file in this folder.

# Contribution

We welcome contributions of new models and integrations of these models into other assets.

FIXME: Contribution guide
FIXME: Reference to Discord


