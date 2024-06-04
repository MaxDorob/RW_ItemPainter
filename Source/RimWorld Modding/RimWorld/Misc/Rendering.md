
###### Thing

Uses the [[Thing]]'s direct [[Graphic]] to draw or print the Thing.
```cs
protected virtual void DrawAt(Vector3 drawLoc, bool flip = false)        
{
	if (this.def.drawerType == DrawerType.RealtimeOnly || !this.Spawned)
	{
		this.Graphic.Draw(drawLoc, flip ?
		this.Rotation.Opposite : this.Rotation, this, 0f);
	}
	SilhouetteUtility.DrawGraphicSilhouette(this, drawLoc);
}
```

```cs
public virtual void Print(SectionLayer layer)        
{
	this.Graphic.Print(layer, this, 0f);        
}
```

###### Pawn

Uses [[Pawn_DrawTracker|Drawer]].[[RimWorld/Codebase/PawnRenderer|renderer]] to do the actual drawing of the [[Pawn]].
```cs
public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, 
									   bool flip = false)        
{            
	base.DynamicDrawPhaseAt(phase, drawLoc, flip);            
	this.Drawer.renderer.DynamicDrawPhaseAt(phase, drawLoc, null, false);        
}
```

```cs
//Doesnt actually do any rendering
protected override void DrawAt(Vector3 drawLoc, bool flip = false)        
{            
	base.Comps_PostDraw();            
	Pawn_MechanitorTracker pawn_MechanitorTracker = this.mechanitor;            
	if (pawn_MechanitorTracker == null)            
	{                
		return;            
	}            
	pawn_MechanitorTracker.DrawCommandRadius();        
}
```
