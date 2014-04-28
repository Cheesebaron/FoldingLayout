using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;

namespace Cheesebaron.Folding
{
    public class FoldingPaneLayout : SlidingPaneLayout
    {
        private new const string Tag = "FoldingPaneLayout";
        private int _numberOfFolds = 2;
        private BaseFoldingLayout _baseFoldingLayout;

        protected FoldingPaneLayout(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer) { }

        public FoldingPaneLayout(Context context) 
            : this(context, null) { }

        public FoldingPaneLayout(Context context, IAttributeSet attrs)
            : this(context, attrs, 0) { }

        public FoldingPaneLayout(Context context, IAttributeSet attrs, int defStyle)
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

            _baseFoldingLayout = new BaseFoldingLayout(context)
            {
                NumberOfFolds = _numberOfFolds,
                AnchorFactor = 0
            };
        }

        public BaseFoldingLayout GetFoldingLayout()
        {
            return _baseFoldingLayout;
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            // Pane content is always the first view?
            var child = GetChildAt(0);

            if (child != null)
            {
                RemoveView(child);

                _baseFoldingLayout.AddView(child);
                AddView(_baseFoldingLayout, 0, child.LayoutParameters);
            }

            PanelSlide += (s, e) =>
            {
                if (_baseFoldingLayout != null)
                    _baseFoldingLayout.FoldFactor = 1 - e.SlideOffset;
            };
        }
    }
}