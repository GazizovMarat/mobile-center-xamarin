﻿/*
 * BottomNavigationBar for Xamarin Forms
 * Copyright (c) 2016 Thrive GmbH and others (http://github.com/thrive-now).
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using BottomBar.XamarinForms;
using BottomNavigationBar;
using BottomNavigationBar.Listeners;

using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using BottomBar.Droid.Renderers;
using BottomBar.Droid.Utils;
using MobileCenterDemoApp.Helpers;

[assembly: ExportRenderer(typeof(BottomTabbedPage), typeof(BottomBarMyPageRenderer))]

namespace BottomBar.Droid.Renderers
{
    public class BottomBarMyPageRenderer : VisualElementRenderer<BottomTabbedPage>, IOnTabClickListener
    {
        bool _disposed;
        BottomNavigationBar.BottomBar _bottomBar;
        FrameLayout _frameLayout;
        IPageController _pageController;

        public BottomBarMyPageRenderer()
        {
            AutoPackage = false;
        }

        #region IOnTabClickListener
        public void OnTabSelected(int position)
        {
            //Do we need this call? It's also done in OnElementPropertyChanged
            SwitchContent(Element.Children[position]);
            var bottomBarPage = Element as BottomTabbedPage;
            bottomBarPage.CurrentPage = Element.Children[position];
            //bottomBarPage.RaiseCurrentPageChanged();
        }

        public void OnTabReSelected(int position)
        {
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                RemoveAllViews();

                foreach (Page pageToRemove in Element.Children)
                {
                    IVisualElementRenderer pageRenderer = Platform.GetRenderer(pageToRemove);

                    if (pageRenderer != null)
                    {
                        pageRenderer.ViewGroup.RemoveFromParent();
                        pageRenderer.Dispose();
                    }

                    // pageToRemove.ClearValue (Platform.RendererProperty);
                }

                if (_bottomBar != null)
                {
                    _bottomBar.SetOnTabClickListener(null);
                    _bottomBar.Dispose();
                    _bottomBar = null;
                }

                if (_frameLayout != null)
                {
                    _frameLayout.Dispose();
                    _frameLayout = null;
                }

                /*if (Element != null) {
					PageController.InternalChildren.CollectionChanged -= OnChildrenCollectionChanged;
				}*/
            }

            base.Dispose(disposing);
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            _pageController.SendAppearing();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            _pageController.SendDisappearing();
        }


        protected override void OnElementChanged(ElementChangedEventArgs<BottomTabbedPage> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {

                BottomTabbedPage bottomBarPage = e.NewElement;

                if (_bottomBar == null)
                {
                    _pageController = PageController.Create(bottomBarPage);

                    // create a view which will act as container for Page's
                    _frameLayout = new FrameLayout(Forms.Context);
                    _frameLayout.LayoutParameters = new FrameLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent, GravityFlags.Fill);
                    AddView(_frameLayout, 0);

                    // create bottomBar control
                    _bottomBar = BottomNavigationBar.BottomBar.Attach(_frameLayout, null);
                    _bottomBar.NoTabletGoodness();
                    if (bottomBarPage.FixedMode)
                    {
                        _bottomBar.UseFixedMode();
                    }

                    switch (bottomBarPage.BarTheme)
                    {
                        case BottomTabbedPage.BarThemeTypes.Light:
                            break;
                        case BottomTabbedPage.BarThemeTypes.DarkWithAlpha:
                            _bottomBar.UseDarkThemeWithAlpha(true);
                            break;
                        case BottomTabbedPage.BarThemeTypes.DarkWithoutAlpha:
                            _bottomBar.UseDarkThemeWithAlpha(false);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    _bottomBar.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    _bottomBar.SetOnTabClickListener(this);

                    UpdateTabs();
                    UpdateBarBackgroundColor();
                    UpdateBarTextColor();
                }

                if (bottomBarPage.CurrentPage != null)
                {
                    SwitchContent(bottomBarPage.CurrentPage);
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(TabbedPage.CurrentPage))
            {
                SwitchContent(Element.CurrentPage);
                UpdateSelectedTabIndex(Element.CurrentPage);
            }
            else if (e.PropertyName == NavigationPage.BarBackgroundColorProperty.PropertyName)
            {
                UpdateBarBackgroundColor();
            }
            else if (e.PropertyName == NavigationPage.BarTextColorProperty.PropertyName)
            {
                UpdateBarTextColor();
            }
        }

        protected virtual void SwitchContent(Page view)
        {
            Context.HideKeyboard(this);

            _frameLayout.RemoveAllViews();

            if (view == null)
            {
                return;
            }

            if (Platform.GetRenderer(view) == null)
            {
                Platform.SetRenderer(view, Platform.CreateRenderer(view));
            }

            _frameLayout.AddView(Platform.GetRenderer(view).ViewGroup);
        }

        private int _iconTopPadding;

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            //for (int i = 0; i < _bottomBar.ItemContainer.ChildCount; i++)
            //{
            //    var bottomBarTab = _bottomBar.ItemContainer.GetChildAt(i);
            //    var tabIcon = bottomBarTab.FindViewById<Android.Support.V7.Widget.AppCompatImageView>(BottomNavigationBar.Resource.Id.bb_bottom_bar_icon);
            //    tabIcon.SetScaleType(ImageView.ScaleType.FitCenter);
            //    tabIcon.ScaleX = 1.3f;
            //    tabIcon.ScaleY = 1.3f;
            //    tabIcon.ImageAlpha = i == _bottomBar.CurrentTabPosition ? 255 : 155;
            //    if (_iconTopPadding == 0)
            //        _iconTopPadding = _bottomBar.ItemContainer.MeasuredHeight / 2 - tabIcon.MeasuredHeight / 2;
            //    tabIcon.SetPadding(0, _iconTopPadding, 0, 0);
            //}

            //base.OnLayout(changed, l, t, r, b);

            int width = r - l;
            int height = b - t;

            var context = Context;

            _bottomBar.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly), MeasureSpecFactory.MakeMeasureSpec(height, MeasureSpecMode.AtMost));
            int tabsHeight = Math.Min(height, Math.Max(_bottomBar.MeasuredHeight, _bottomBar.MinimumHeight));

            if (width > 0 && height > 0)
            {
                _pageController.ContainerArea = new Rectangle(0, 0, context.FromPixels(width), context.FromPixels(_frameLayout.MeasuredHeight));
                ObservableCollection<Element> internalChildren = _pageController.InternalChildren;

                for (var i = 0; i < internalChildren.Count; i++)
                {
                    var child = internalChildren[i] as VisualElement;

                    if (child == null)
                    {
                        continue;
                    }

                    IVisualElementRenderer renderer = Platform.GetRenderer(child);
                    var navigationRenderer = renderer as NavigationPageRenderer;
                    if (navigationRenderer != null)
                    {
                        // navigationRenderer.ContainerPadding = tabsHeight;
                    }
                }

                _bottomBar.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly), MeasureSpecFactory.MakeMeasureSpec(tabsHeight, MeasureSpecMode.Exactly));
                _bottomBar.Layout(0, 0, width, tabsHeight);
            }

            base.OnLayout(changed, l, t, r, b);

        }

        void UpdateSelectedTabIndex(Page page)
        {
            var index = Element.Children.IndexOf(page);
            _bottomBar.SelectTabAtPosition(index, true);
        }

        void UpdateBarBackgroundColor()
        {
            if (_disposed || _bottomBar == null)
            {
                return;
            }

            _bottomBar.SetBackgroundColor(Element.BarBackgroundColor.ToAndroid());
        }

        void UpdateBarTextColor()
        {
            if (_disposed || _bottomBar == null)
            {
                return;
            }

            _bottomBar.SetActiveTabColor(Element.BarTextColor.ToAndroid());
            // The problem SetActiveTabColor does only work in fiexed mode // haven't found yet how to set text color for tab items on_bottomBar, doesn't seem to have a direct way
        }

        void UpdateTabs()
        {
            // create tab items
            SetTabItems();

            // set tab colors
            SetTabColors();
        }

        void SetTabItems()
        {
            BottomBarTab[] tabs = Element.Children.Select(page => {
                var tabIconId = ResourceManagerEx.IdFromTitle(page.Icon, ResourceManager.DrawableClass);
                return new BottomBarTab(tabIconId, page.Title);
            }).ToArray();

            if (tabs.Length > 0)
            {
                _bottomBar.SetItems(tabs);
            }
        }

        void SetTabColors()
        {
            for (int i = 0; i < Element.Children.Count; ++i)
            {
                Page page = Element.Children[i];

                Color tabColor = BottomBarPageExtensions.GetTabColor(page);

                if (tabColor != Color.Transparent)
                {
                    _bottomBar.MapColorForTab(i, tabColor.ToAndroid());
                }
            }
        }
    }
}