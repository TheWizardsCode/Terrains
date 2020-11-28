# Open Source Models for Unity 3D

This is a library of open source assets that you can use in early prototypes of your games and assets. They are all under a license that means you can use them in any environment and without restriction. This includes redistribution as individual assets. This means you can use them in demo scenes for your assets - something that is normally prohibitted. All you are required to do is provide a LICENSE and a NOTICE file in your project source.

The goal of this project is not to provide AAA quality assets but instead provide a library of assets that are "good enough" for game prototypes and demo scenes for your own published assets. Though you will be surprised at the quality of some of the models here.

# Use

  1. Import this package into your project (see below for Package Manager details).
  2. Open the `OpenSourceModels/Scenes/ModelsCatalog` scene
  3. Use the UI to filter the available models and select one for display
  4. When you find a model you like it will have been pinged in the Project view - go ahead and use it in your project as you would use any other model - but without license restrictions (see `NOTICE File` below)

## Installing Via Package Manager

Modify your manifest.json file found at /PROJECTNAME/Packages/manifest.json by including the following line - be sure to replace '[VERSiON NUMBER]' with the released version you want to use. You can see the list of [release branches](https://github.com/3dtbd/Common/branches/all?utf8=%E2%9C%93&query=release%2F) on GitHub. 

```json
{
	"dependencies": {
		...
		"org.3dtbd.models": "https://github.com/3dtbd/Models.git#release/v[VERSION NUMBER]",
		...
  }
}
```

## Categorization System

Models are categorized by defining a meta data file for each prefab. This meta-data file captures information such as the category (as in building, tree, flower etc.) and the time period (as in Medieval, modern etc.). The database can then be filtered based on this data. This data is stored alongside the model in a Scriptable Object (`ModelMetaData`).

When the application is run in the editor all prefabs found in the `OpenSourceModels` folder are discovered and their meta-data files are loaded. If this file is missing a default one is created and saved. These files should then be edited to ensure the prefab is correctly categorized in the system.

You can create meta data objects for any assets you import using the `Create -> 3DTBD -> Model Meta Data`

At the time of writing these files need to be edited in the Unity Editor. 

Categories and time periods can be created using the `Create -> 3DTBD -> Category` or `Create -> 3DTBD -> Time Period` menu items. They should be created in the `OpenSourceModels/Resources/Categories` and `OpenSourceModels/Resources/Time Periods` folders.

# NOTICE file

When you use assets from this project you must copy the NOTICE and LICENSE files that can be found in the root of this project into your project. The NOTICE file must be placed in the root of your project while the LICENSE file can be placed anywhere convenient. 

If you already have NOTICE file simply append the contents of our NOTICE to yours.

In order to avoid confusion over which assets are covered by our LICENSE we recommend keeping all the assets you use in a directory called `3dtbd/OpenSource` or similar. You can then place the LICENSE file in this folder.

# Contribution

We welcome contributions of new models and integrations of these models into other assets.

FIXME: Contribution guide
FIXME: Reference to Discord
