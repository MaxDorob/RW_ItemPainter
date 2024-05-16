---
Project: RWPaintingTool
Dependencies:
  - "[[TeleCore.Loader]]"
  - "[[TeleCore.Events]]"
  - "[[TeleCore.AssetLoader]]"
  - "[[TeleCore.Shared]]"
---
This framework extends the way RimWorld handles masks and coloring of items.
To implement this, multiple new shaders have been created and existing rendering methods have been patched.

## Setup

When making a def for your item, once you set the [[ThingDef#graphicData|graphicData]], all you need to do is add a [[PaintableExtension]] to the [[ThingDef#modExtensions|modExtensions]]. 

This marks the def to be treated differently by the rendering system, by default it now uses the [[CutoutMultiMask]] shader instead of Cutout (which only supports the Red and Green color channels).

This means you don't have to set the [[GraphicData#shaderType|shaderType]] or use a custom [[GraphicData#graphicClass|graphicClass]].


## Function

A [[Thing]] has a [[Graphic]].
A Graphic exposes a [[Material]], essentially being a worker, often selecting from multiple materials.

A Material holds a [[Texture]] `_MainTex` and a [[Shader]] by default, with the shader being responsible for exposing which properties can be set on the Material, and then of course processing the set data on the GPU. These properties can include other textures, like an extra mask texture with a property name like `_MaskTex`, which is the case here, as well as color properties going from `_ColorOne` through `_ColorSix`.

This system is meant to allow for `_MaskTex` to be changed dynamically during gameplay, usually a Graphic that supports masks loads only one mask texture for the relative `_MainTex`.

#### Mask Management

So how is it done?
When a texture folder contains a texture with the suffix `SomeTexture_mask`, it usually gets assigned to the texture named `SomeTexture`, if the shader set in the graphic [[Masking Support|supports masking]].
But that is always only a single mask, now how would multiple masks be handled so we know which masks belong to which thing-state (since a thing can have rotations).

###### Concept:

Define ThingState.
Bind each texture to the corresponding thing-state.
Identify which masks belong to a texture.
Identify Thing-state and which texture is used.

###### ThingState

- Rotation (North, East, South, West)
- BodyType - For Pawns, which BodyType is used

[[Rendering|How are things rendered?]]

