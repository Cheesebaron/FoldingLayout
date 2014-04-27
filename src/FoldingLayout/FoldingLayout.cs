using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace Folding
{
    public class FoldingLayout : BaseFoldingLayout
    {
        private GestureDetector _scrollGestureDetector;
        private FoldingLayout _that;

        public FoldingLayout(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer) { }

        public FoldingLayout(Context context) 
            : this(context, null) { }

        public FoldingLayout(Context context, IAttributeSet attrs)
            : this(context, attrs, 0) { }

        public FoldingLayout(Context context, IAttributeSet attrs, int defStyle) 
            : base(context, attrs, defStyle)
        {
            _that = this;
            Init(context, attrs);
        }

        private new void Init(Context context, IAttributeSet attrs)
        {
            _scrollGestureDetector = new GestureDetector(context,
                new ScrollGestureDetector(_that, ViewConfiguration.Get(context).ScaledTouchSlop));
            AnchorFactor = 0;

            base.Init(context, attrs);
        }

        protected override bool AddViewInLayout(View child, int index, LayoutParams @params, bool preventRequestLayout)
        {
            if (ChildCount > 1)
                throw new InvalidOperationException("FoldingLayout can only have 1 child at most");

            return base.AddViewInLayout(child, index, @params, preventRequestLayout);
        }

        private class ScrollGestureDetector : GestureDetector.SimpleOnGestureListener
        {
            private readonly FoldingLayout _layout;
            private bool _didNotStartScroll = true;
            private int _translation;
            private int _parentPositionY = -1;
            private readonly int _touchSlop = -1;

            public ScrollGestureDetector(FoldingLayout layout, int touchSlop)
            {
                _layout = layout;
                _touchSlop = touchSlop;
            }

            public override bool OnDown(MotionEvent e)
            {
                _didNotStartScroll = true;
                return true;
            }

            public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                var touchSlop = 0;
                float factor;
                if (_layout.Orientation == FoldingLayoutOrientation.Vertical)
                {
                    factor = Math.Abs((float) _translation / _layout.Height);

                    if (e2.RawY - _parentPositionY <= _layout.Height
                        && e2.RawY - _parentPositionY >= 0)
                    {
                        if ((e2.RawY - _parentPositionY) > _layout.Height * _layout.AnchorFactor)
                        {
                            _translation -= (int)distanceY;
                            touchSlop = distanceY < 0 ? -_touchSlop : _touchSlop;
                        }
                        else
                        {
                            _translation += (int)distanceY;
                            touchSlop = distanceY < 0 ? _touchSlop : -_touchSlop;
                        }

                        _translation = _didNotStartScroll ? _translation
                            + touchSlop : _translation;

                        if (_translation < -_layout.Height)
                        {
                            _translation = -_layout.Height;
                        }
                    }
                }
                else
                {
                    factor = Math.Abs(_translation / (float)_layout.Width);

                    if (e2.RawX > _layout.Width * _layout.AnchorFactor)
                    {
                        _translation -= (int)distanceX;
                        touchSlop = distanceX < 0 ? -touchSlop : touchSlop;
                    }
                    else
                    {
                        _translation += (int)distanceX;
                        touchSlop = distanceX < 0 ? touchSlop : -touchSlop;
                    }
                    _translation = _didNotStartScroll ? _translation + touchSlop
                            : _translation;

                    if (_translation < -_layout.Width)
                    {
                        _translation = -_layout.Width;
                    }
                }

                _didNotStartScroll = false;

                if (_translation > 0)
                    _translation = 0;

                _layout.FoldFactor = factor;

                return true;
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            return _scrollGestureDetector.OnTouchEvent(e);
        }
    }
}