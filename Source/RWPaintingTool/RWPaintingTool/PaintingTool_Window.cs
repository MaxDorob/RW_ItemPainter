using LudeonTK;
using TeleCore.UI;
using UnityEngine;
using Verse;

namespace RWPaintingTool;

public partial class PaintingTool : Window
{
    [TweakValue("_RWIP", 0f, 700f)]
    private static readonly float colorsWidthPx = 448f;
    
    private ColorPicker _colorPicker;
    
    public override Vector2 InitialSize => new(950f, 750f);
    
    public PaintingTool(Thing thing)
    {
        forcePause = true;
        
        _thing = thing;
        _colorPicker = new ColorPicker();
        _colorPicker.ColorChanged += color => Notify_ColorChanged(color, _curColorIndex);
        
        //TODO Get correct colors and then set it too
        _colorsSet = new SixColorSet
        {
            ColorOne = new Color(Rand.Value, Rand.Value, Rand.Value),
            ColorTwo = new Color(Rand.Value, Rand.Value, Rand.Value),
            ColorThree = new Color(Rand.Value, Rand.Value, Rand.Value),
            ColorFour = new Color(Rand.Value, Rand.Value, Rand.Value),
            ColorFive = new Color(Rand.Value, Rand.Value, Rand.Value),
            ColorSix = new Color(Rand.Value, Rand.Value, Rand.Value),
        };
        
        _colorPicker.SetColor(_colorsSet[0]);
    }
    
    public override void DoWindowContents(Rect inRect)
    {
        Text.Anchor = TextAnchor.UpperLeft;
        Widgets.Label(inRect, _thing.LabelCap);
        Text.Anchor = 0;
        
        var topBar = inRect.TopPartPixels(32);
        var body = inRect.BottomPartPixels(inRect.height - 32);
        var leftPart = body.LeftPartPixels(body.height / 3);
        
        var rightPart = body.RightPartPixels(body.width - leftPart.width).ContractedBy(5,0).Rounded();

        DrawItemPreview(leftPart);
        
        Widgets.DrawMenuSection(rightPart);
        DrawColorTool(rightPart.ContractedBy(5).TopPartPixels(300).LeftPartPixels(300));
    }
    
    private void DrawColorTool(Rect rect)
    {
        rect = rect.Rounded();
        var colorSize = ColorPicker.DefaultSize;
        var colorSelWidth = colorSize.y / 6;
        var subRect = rect.LeftPartPixels(colorSize.x + colorSelWidth);
        var colorPickerRect = subRect.RightPartPixels(colorSize.x);
        var colorSelectRect = subRect.LeftPartPixels(colorSelWidth-5);
        
        //Widgets.DrawHighlight(subRect);
        //Widgets.DrawHighlight(colorPickerRect);
        //Widgets.DrawHighlight(colorSelectRect);
        
        var res = _colorPicker.Draw(colorPickerRect.position);
        
        //TODO: Improve hashcode perf
        var subDiv = new RectDivider(colorSelectRect.ContractedBy(0,2.5f), colorSelectRect.GetHashCode());

        for (int i = 0; i < 6; i++)
        {
            var colorDiv = subDiv.NewRow(colorSelWidth - 5, VerticalJustification.Top, 5);
            var colorRect = colorDiv.Rect.Rounded();
            var color = _colorsSet[i];
            var isSelected = i == _curColorIndex;
            var mouseOver = colorRect.Contains(Event.current.mousePosition);
            var isHighlighted = isSelected || mouseOver;
            Widgets.DrawBoxSolid(colorRect, color);
            if (mouseOver)
            {
                _highLightedIndex = i;   
            }
            if (isHighlighted)
            {
                Widgets.DrawBox(colorRect, 1);
            }
            if (Widgets.ButtonInvisible(colorRect))
            {
                _curColorIndex = i;
                _colorPicker.SetColor(color);
            }
        }
    }
    
}