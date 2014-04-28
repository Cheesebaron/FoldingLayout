using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;

namespace Cheesebaron.Folding
{
    public class FoldingDrawerLayout : DrawerLayout
    {
        private new const string Tag = "FoldingDrawerLayout";
        private int _numberOfFolds = 2;

        protected FoldingDrawerLayout(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer) { }

        public FoldingDrawerLayout(Context context) 
            : base(context) { }

        public FoldingDrawerLayout(Context context, IAttributeSet attrs) 
            : base(context, attrs) { }

        public FoldingDrawerLayout(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            InitView(context, attrs);
        }

        private void InitView(Context context, IAttributeSet attrs)
        {
            var ta = context.ObtainStyledAttributes(attrs, Resource.Styleable.FoldingLayout);
            var folds = ta.GetInt(Resource.Styleable.FoldingLayout_numberOfFolds,
                    _numberOfFolds);
            if (folds > 0 && folds < 8)
                _numberOfFolds = folds;
            else
                _numberOfFolds = 2;
            ta.Recycle();
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            for (var i = 0; i < ChildCount; i++)
            {
                var child = GetChildAt(i);
                if (!IsViewInDrawer(child)) continue;

                Log.Debug(Tag, "at {0}", i);
                var foldingNavigationLayout = new BaseFoldingLayout(Context)
                {
                    AnchorFactor = 1,
                    NumberOfFolds = _numberOfFolds
                };
                RemoveView(child);
                foldingNavigationLayout.AddView(child);
                var layoutParams = child.LayoutParameters;
                AddView(foldingNavigationLayout, i, layoutParams);
            }

            DrawerSlide += (s, e) =>
            {
                var foldingLayout = e.DrawerView as BaseFoldingLayout;
                if (foldingLayout == null) return;

                foldingLayout.FoldFactor = 1 - e.SlideOffset;
            };
        }

        public override void CloseDrawer(View drawerView)
        {
            base.CloseDrawer(GetRealDrawer(drawerView));
        }

        private static bool IsViewInDrawer(View child)
        {
            var gravity = ((LayoutParams) child.LayoutParameters).Gravity;
            var absGravity = GravityCompat.GetAbsoluteGravity(gravity,
                ViewCompat.GetLayoutDirection(child));
            return (absGravity & ((int) GravityFlags.Left | (int) GravityFlags.Right)) != 0;
        }

        private static View GetRealDrawer(View drawerView)
        {
            var drawerview2 = drawerView.Parent as BaseFoldingLayout;

            return drawerview2 ?? drawerView;
        }

        public BaseFoldingLayout GetFoldingLayout(View drawerView)
        {
            if (!IsViewInDrawer(GetRealDrawer(drawerView)))
                throw new ArgumentException("View is not a DrawerLayout", "drawerView");

            var drawer = GetRealDrawer(drawerView) as BaseFoldingLayout;

            return drawer;
        }

        /// <summary>
        /// Apparently this needs to be overriden otherwise children of DrawerLayout
        /// will have MarginLayoutParams instead of DrawerLayout.LayoutParams, which
        /// makes it impossible to get the gravity of a child in the DrawerLayout.
        /// </summary>
        /// <param name="attrs"></param>
        /// <returns></returns>
        public override ViewGroup.LayoutParams GenerateLayoutParams(IAttributeSet attrs)
        {
            return new LayoutParams(Context, attrs);
        }
    }
}