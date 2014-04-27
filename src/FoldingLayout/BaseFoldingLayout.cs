using System;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Math = System.Math;

namespace Folding
{
    public class BaseFoldingLayout : ViewGroup
    {
        public enum FoldingLayoutOrientation
        {
            Vertical,
            Horizontal
        }

        private const float ShadingAlpha = 0.8f;
        private const float ShadingFactor = 0.5f;
        private const int DepthConstant = 1500;
        private const int NumOfPolyPoints = 8;

        private int _numberOfFolds = 2;
        private Rect[] _foldRectArray;
        private Matrix[] _matrix;
        protected float _anchorFactor = 0;
        private float _foldFactor;
        private bool _isHorizontal = true;
        private int _originalWidth;
        private int _originalHeight;
        private float _foldMaxWidth;
        private float _foldMaxHeight;
        private float _foldDrawWidth;
        private float _foldDrawHeight;

        private bool _isFoldPrepared;
        private bool _shouldDraw = true;

        private Paint _solidShadow;
        private Paint _gradientShadow;
        private LinearGradient _shadowLinearGradient;
        private Matrix _shadowGradientMatrix;

        private float[] _src;
        private float[] _dst;
        private float _previousFoldFactor;

        private Bitmap _fullBitmap;
        private Rect _dstRect;

        protected FoldingLayoutOrientation _orientation = FoldingLayoutOrientation.Horizontal;

        public event EventHandler FoldStarted;
        public event EventHandler FoldEnded;

        public float FoldFactor
        {
            get { return _foldFactor; }
            set
            {
                if (FloatNearlyEqual(value, _foldFactor)) return;

                _foldFactor = value;
                CalculateMatrices();
                Invalidate();
            }
        }

        public FoldingLayoutOrientation Orientation
        {
            get { return _orientation; }
            set
            {
                if (_orientation == value) return;

                _orientation = value;
                UpdateFold();
            }
        }

        public float AnchorFactor
        {
            get { return _anchorFactor; }
            set
            {
                if (FloatNearlyEqual(_anchorFactor, value)) return;

                _anchorFactor = value;
                UpdateFold();
            }
        }

        public int NumberOfFolds
        {
            get { return _numberOfFolds; }
            set
            {
                if (_numberOfFolds == value) return;

                _numberOfFolds = value;
                UpdateFold();
            }
        }

        public BaseFoldingLayout(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer) { }

        public BaseFoldingLayout(Context context)
            : this(context, null) { }
        

        public BaseFoldingLayout(Context context, IAttributeSet attrs) 
            : this(context, attrs, 0) { }

        public BaseFoldingLayout(Context context, IAttributeSet attrs, int defStyle) 
            : base(context, attrs, defStyle)
        {
            Init(context, attrs);
        }

        protected void Init(Context context, IAttributeSet attrs)
        {
            var ta = context.ObtainStyledAttributes(attrs, Resource.Styleable.FoldingLayout);
            var numberOfFolds = ta.GetInt(Resource.Styleable.FoldingLayout_numberOfFolds, _numberOfFolds);
            if (numberOfFolds > 0 && _numberOfFolds < 7)
                _numberOfFolds = numberOfFolds;
            else
                _numberOfFolds = 2;
            ta.Recycle();
        }

        protected override bool AddViewInLayout(View child, int index, LayoutParams @params, bool preventRequestLayout)
        {
            if (ChildCount > 1)
                throw new InvalidOperationException("BaseFoldingLayout can only have 1 child at most");

            return base.AddViewInLayout(child, index, @params, preventRequestLayout);
        }

        public override void AddView(View child, int index, LayoutParams @params)
        {
            if (ChildCount > 1)
                throw new InvalidOperationException("BaseFoldingLayout can only have 1 child at most");

            base.AddView(child, index, @params);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var child = GetChildAt(0);
            MeasureChild(child, widthMeasureSpec, heightMeasureSpec);
            SetMeasuredDimension(widthMeasureSpec, heightMeasureSpec);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var child = GetChildAt(0);
            child.Layout(0, 0, child.MeasuredWidth, child.MeasuredHeight);
            UpdateFold();
        }

        private void UpdateFold()
        {
            PrepareFold(Orientation, AnchorFactor, _numberOfFolds);
            CalculateMatrices();
            Invalidate();
        }

        private void PrepareFold(FoldingLayoutOrientation orientation, float anchorFactor, int numberOfFolds)
        {
            _src = new float[NumOfPolyPoints];
            _dst = new float[NumOfPolyPoints];

            _dstRect = new Rect();

            _foldFactor = 0;
            _previousFoldFactor = 0;

            _isFoldPrepared = false;

            _solidShadow = new Paint();
            _gradientShadow = new Paint();

            Orientation = orientation;
            _isHorizontal = Orientation == FoldingLayoutOrientation.Horizontal;

            if (_isHorizontal)
                _shadowLinearGradient = new LinearGradient(0, 0, ShadingFactor, 0, Color.Black, Color.Transparent,
                    Shader.TileMode.Clamp);
            else
                _shadowLinearGradient = new LinearGradient(0, 0, 0, ShadingFactor, Color.Black, Color.Transparent,
                    Shader.TileMode.Clamp);

            _gradientShadow.SetStyle(Paint.Style.Fill);
            _gradientShadow.SetShader(_shadowLinearGradient);
            _shadowGradientMatrix = new Matrix();

            _anchorFactor = anchorFactor;
            _numberOfFolds = numberOfFolds;

            _originalWidth = MeasuredWidth;
            _originalHeight = MeasuredHeight;

            _foldRectArray = new Rect[_numberOfFolds];
            _matrix = new Matrix[_numberOfFolds];

            for (var x = 0; x < _numberOfFolds; x++)
                _matrix[x] = new Matrix();

            var h = _originalHeight;
            var w = _originalWidth;

            if (Build.VERSION.SdkInt == BuildVersionCodes.JellyBeanMr2 && h != 0 && w != 0)
            {
                _fullBitmap = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);
                var canvas = new Canvas(_fullBitmap);
                GetChildAt(0).Draw(canvas);
            }

            var delta = (int) Math.Round(_isHorizontal
                ? ((float) w) / _numberOfFolds
                : ((float) h) / _numberOfFolds);

            for (var i = 0; i < _numberOfFolds; i++)
            {
                if (_isHorizontal)
                {
                    var deltap = (i + 1) * delta > w ? w - i * delta : delta;
                    _foldRectArray[i] = new Rect(i * delta, 0, i * delta + deltap, h);
                }
                else
                {
                    var deltap = (i + 1) * delta > h ? h - i * delta : delta;
                    _foldRectArray[i] = new Rect(0, i * delta, w, i * delta + deltap);
                }
            }

            if (_isHorizontal)
            {
                _foldMaxHeight = h;
                _foldMaxWidth = delta;
            }
            else
            {
                _foldMaxHeight = delta;
                _foldMaxWidth = w;
            }

            _isFoldPrepared = true;
        }

        private void CalculateMatrices()
        {
            _shouldDraw = true;

            if (!_isFoldPrepared)
                return;

            if (FloatNearlyEqual(_foldFactor, 1))
            {
                _shouldDraw = false;
                return;
            }

            if (FloatNearlyEqual(_foldFactor, 0) && _previousFoldFactor > 0 && FoldEnded != null)
                FoldEnded(this, EventArgs.Empty);

            if (FloatNearlyEqual(_previousFoldFactor, 0) && _foldFactor > 0 && FoldStarted != null)
                FoldStarted(this, EventArgs.Empty);

            _previousFoldFactor = _foldFactor;

            for (var i = 0; i < _numberOfFolds; i++)
                _matrix[i].Reset();

            var translationFactor = 1 - _foldFactor;

            var translatedDistance = _isHorizontal
                ? _originalWidth * translationFactor
                : _originalHeight * translationFactor;

            var translatedDistancePerFold = Math.Round(translatedDistance / _numberOfFolds);

            _foldDrawWidth =
                (float) (_foldMaxWidth < translatedDistancePerFold ? translatedDistancePerFold : _foldMaxWidth);
            _foldDrawHeight =
                (float) (_foldMaxHeight < translatedDistancePerFold ? translatedDistancePerFold : _foldMaxHeight);

            var translatedDistanceFoldSquared = translatedDistancePerFold * translatedDistancePerFold;

            var depth = _isHorizontal
                ? (float) Math.Sqrt(_foldDrawWidth * _foldDrawWidth - translatedDistanceFoldSquared)
                : (float) Math.Sqrt(_foldDrawHeight * _foldDrawHeight - translatedDistanceFoldSquared);

            var scaleFactor = DepthConstant / (DepthConstant + depth);

            float scaledWidth, scaledHeight;

            if (_isHorizontal)
            {
                scaledWidth = _foldDrawWidth * translationFactor;
                scaledHeight = _foldDrawHeight * scaleFactor;
            }
            else
            {
                scaledWidth = _foldDrawWidth * scaleFactor;
                scaledHeight = _foldDrawHeight * translationFactor;
            }

            var topScaledPoint = (_foldDrawHeight - scaledHeight) / 2.0f;
            var bottomScaledPoint = topScaledPoint + scaledHeight;

            var leftScaledPoint = (_foldDrawWidth - scaledWidth) / 2.0f;
            var rightScaledPoint = leftScaledPoint + scaledWidth;

            var anchorPoint = _isHorizontal ? AnchorFactor * _originalWidth : AnchorFactor * _originalHeight;

            var midFold = _isHorizontal ? (anchorPoint / _foldDrawWidth) : anchorPoint / _foldDrawHeight;

            _src[0] = 0;
            _src[1] = 0;
            _src[2] = 0;
            _src[3] = _foldDrawHeight;
            _src[4] = _foldDrawWidth;
            _src[5] = 0;
            _src[6] = _foldDrawWidth;
            _src[7] = _foldDrawHeight;

            for (var i = 0; i < _numberOfFolds; i++)
            {
                var isEven = (i % 2 == 0);

                if (_isHorizontal)
                {
                    _dst[0] = (anchorPoint > i * _foldDrawWidth) ? anchorPoint
                        + (i - midFold) * scaledWidth : anchorPoint
                        - (midFold - i) * scaledWidth;
                    _dst[1] = isEven ? 0 : topScaledPoint;
                    _dst[2] = _dst[0];
                    _dst[3] = isEven ? _foldDrawHeight : bottomScaledPoint;
                    _dst[4] = (anchorPoint > (i + 1) * _foldDrawWidth) ? anchorPoint
                            + (i + 1 - midFold) * scaledWidth
                            : anchorPoint - (midFold - i - 1) * scaledWidth;
                    _dst[5] = isEven ? topScaledPoint : 0;
                    _dst[6] = _dst[4];
                    _dst[7] = isEven ? bottomScaledPoint : _foldDrawHeight;
                }
                else
                {
                    _dst[0] = isEven ? 0 : leftScaledPoint;
                    _dst[1] = (anchorPoint > i * _foldDrawHeight) ? anchorPoint
                            + (i - midFold) * scaledHeight : anchorPoint
                            - (midFold - i) * scaledHeight;
                    _dst[2] = isEven ? leftScaledPoint : 0;
                    _dst[3] = (anchorPoint > (i + 1) * _foldDrawHeight) ? anchorPoint
                            + (i + 1 - midFold) * scaledHeight
                            : anchorPoint - (midFold - i - 1) * scaledHeight;
                    _dst[4] = isEven ? _foldDrawWidth : rightScaledPoint;
                    _dst[5] = _dst[1];
                    _dst[6] = isEven ? rightScaledPoint : _foldDrawWidth;
                    _dst[7] = _dst[3];
                }

                for (var j = 0; j < NumOfPolyPoints; j++)
                    _dst[j] = (float) Math.Round(_dst[j]);

                if (_isHorizontal)
                {
                    if (_dst[4] <= _dst[0] || _dst[6] <= _dst[2])
                    {
                        _shouldDraw = false;
                        return;
                    }
                }
                else
                {
                    if (_dst[3] <= _dst[1] || _dst[7] <= _dst[5])
                    {
                        _shouldDraw = false;
                        return;
                    }
                }

                _matrix[i].SetPolyToPoly(_src, 0, _dst, 0, NumOfPolyPoints / 2);
            }

            var alpha = (int) (_foldFactor * 255 * ShadingAlpha);

            _solidShadow.Color = Color.Argb(alpha, 0, 0, 0);

            if (_isHorizontal)
            {
                _shadowGradientMatrix.SetScale(_foldDrawWidth, 1);
                _shadowLinearGradient.SetLocalMatrix(_shadowGradientMatrix);
            }
            else
            {
                _shadowGradientMatrix.SetScale(1, _foldDrawHeight);
                _shadowLinearGradient.SetLocalMatrix(_shadowGradientMatrix);
            }

            _gradientShadow.Alpha = alpha;
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            if (!_isFoldPrepared || FloatNearlyEqual(_foldFactor, 0))
            {
                base.DispatchDraw(canvas);
                return;
            }

            if (!_shouldDraw)
                return;

            for (var x = 0; x < _numberOfFolds; x++)
            {
                var src = _foldRectArray[x];

                canvas.Save();

                canvas.Concat(_matrix[x]);

                if (Build.VERSION.SdkInt == BuildVersionCodes.JellyBeanMr2)
                {
                    _dstRect.Set(0, 0, src.Width(), src.Height());
                    canvas.DrawBitmap(_fullBitmap, src, _dstRect, null);
                }
                else
                {
                    canvas.ClipRect(0, 0, src.Right - src.Left, src.Bottom - src.Top);

                    if (_isHorizontal)
                        canvas.Translate(-src.Left, 0);
                    else
                        canvas.Translate(0, -src.Top);

                    base.DispatchDraw(canvas);

                    if (_isHorizontal)
                        canvas.Translate(src.Left, 0);
                    else
                        canvas.Translate(0, src.Top);
                }

                canvas.DrawRect(0, 0, _foldDrawWidth, _foldDrawHeight, x % 2 == 0 ? _solidShadow : _gradientShadow);

                canvas.Restore();
            }
        }

        public static bool FloatNearlyEqual(float a, float b, float epsilon)
        {
            var absA = Math.Abs(a);
            var absB = Math.Abs(b);
            var diff = Math.Abs(a - b);

            if (a == b) // shortcut, handles infinities
                return true;
            if (a == 0 || b == 0 || diff < float.MinValue)
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * float.MinValue);

            // use relative error
            return diff / (absA + absB) < epsilon;
        }

        public static bool FloatNearlyEqual(float a, float b)
        {
            return FloatNearlyEqual(a, b, float.Epsilon);
        }
    }
}