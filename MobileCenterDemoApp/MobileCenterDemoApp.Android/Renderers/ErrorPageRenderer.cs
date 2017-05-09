using Android.Graphics;
using Android.Views;
using BottomBar.Droid.Renderers;
using View = Android.Views.View;


namespace MobileCenterDemoApp.Droid.Renderers
{
    public class ErrorPageRenderer : BottomBarPageRenderer
    {
        public ErrorPageRenderer()
        {
            ElementChanged += (sender, args) =>
            {
                return;
            };

            Hover += (sender, args) =>
            {
                return;
            };

            Click += (sender, args) =>
            {
                return;
            };

            Touch += (sender, args) =>
            {
                return;
            };
        }

        public override void RequestLayout()
        {
            base.RequestLayout();
        }

        public override bool RequestFocus(FocusSearchDirection direction, Rect previouslyFocusedRect)
        {
            return base.RequestFocus(direction, previouslyFocusedRect);
        }

        public override void RequestChildFocus(View child, View focused)
        {
            base.RequestChildFocus(child, focused);
        }
    }
}