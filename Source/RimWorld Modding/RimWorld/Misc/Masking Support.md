
RimWorld uses a hardcoded check to return whether a shader supports the `_MaskTex` property.

The list includes:
- CutoutComplex
- CutoutSkinOverlay
- Wound
- FirefoamOverlay
- CutoutWithOverlay
- CutoutComplexBlend
- BioferriteHarvester

```cs
public static bool SupportsMaskTex(this Shader shader)        
{            
	return shader == ShaderDatabase.CutoutComplex 
	|| shader == ShaderDatabase.CutoutSkinOverlay 
	|| shader == ShaderDatabase.Wound 
	|| shader == ShaderDatabase.FirefoamOverlay 
	|| shader == ShaderDatabase.CutoutWithOverlay 
	|| shader == ShaderDatabase.CutoutComplexBlend 
	|| shader == ShaderDatabase.BioferriteHarvester;        
}
```

This has been extended within [[TeleCore.AssetLoader]], since it provides managed shader access, it also implements a [[CustomShaderDef]] with a `supportMask` field. 
This can be used to tell the method above that a custom shader also supports the mask property.