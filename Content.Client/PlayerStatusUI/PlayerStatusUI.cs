using System.Numerics;
using Content.Client.Administration.UI.CustomControls;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.PlayerStatus.UI;

public sealed partial class PlayerStatusUI : UIWidget
{
    public RichTextLabel EntNameText;
    public BoxContainer ControlContainer;

    public PlayerStatusUI()
    {
        Name = "StatusUI";
        HorizontalAlignment = HAlignment.Center;
        VerticalAlignment = VAlignment.Top;
        MinSize = new Vector2(256, 256);
        Orientation = LayoutOrientation.Vertical;
        Margin = new Thickness(10);

        var panel = new PanelContainer
        {
            HorizontalAlignment = HAlignment.Stretch,
            VerticalAlignment = VAlignment.Stretch,
            StyleClasses = { "tooltipBox" }
        };

        var box = new BoxContainer
        {
            Margin = new Thickness(2),
            Orientation = LayoutOrientation.Vertical
        };

        var stripeBack = new StripeBack
        {
            MaxSize = new Vector2(275,1000),
            HorizontalExpand = true,
            VerticalExpand = true
        };

        EntNameText = new RichTextLabel
        {
            HorizontalAlignment = HAlignment.Center
        };

        ControlContainer = new BoxContainer
        {
            SeparationOverride = 2,
            Orientation = LayoutOrientation.Vertical,
            Margin = new Thickness(2)
        };

        var footer = new VSeparator
        {
            MinHeight = SetHeight = 2,
            Visible = false,
            ReservesSpace = true
        };

        stripeBack.AddChild(EntNameText);

        box.AddChild(stripeBack);
        box.AddChild(ControlContainer);
        box.AddChild(footer);
        box.AddChild(box);

        AddChild(panel);
    }

    public void ResetUI()
    {
        ControlContainer.DisposeAllChildren();
    }

}
