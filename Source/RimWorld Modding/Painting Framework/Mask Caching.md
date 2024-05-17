
## Situation

We have a graphic for an armor piece "SteelChestplate".

Path
`Textures\Armor\SteelChestPlate`

Path For BodyType "Hulk"
`Textures\Armor\SteelChestPlate_Hulk`

Mask example:
`Textures\Armor\SteelChestPlate_Hulk_Mask0`

### ThingState

A thing chooses graphics based on: 
- Random state ([[Graphic_Random]])
- Thing stack count ([[Graphic_StackCount]])
- Rotation ([[Graphic_Multi]])
- BodyType (resolved through wornGraphic in [[Apparel]])


State from path:

`Path/[Name][BodyType?][Rotation?][Mask]`
