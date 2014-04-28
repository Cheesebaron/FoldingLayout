using Android.App;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cheesebaron.Folding;
using Orientation = Cheesebaron.Folding.BaseFoldingLayout.FoldingLayoutOrientation;

namespace Sample
{
    [Activity(Label = "FoldingLayout Sample", MainLauncher = false, Theme = "@style/Theme.AppCompat.Light")]
    public class FoldingLayoutActivity : ActionBarActivity
    {
        private const string Tag = "FoldingLayoutActivity";
        
        private ImageView _imageView;
        private SeekBar _anchorSeekBar;
        private FoldingLayout _foldingLayout;

        private float _anchorFactor;
        private int _numberOfFolds;
        private bool _didLoadSpinner;
        private Orientation _orientation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_fold);

            _imageView = FindViewById<ImageView>(Resource.Id.image_view);
            _imageView.SetImageResource(Resource.Drawable.image);

            _anchorSeekBar = FindViewById<SeekBar>(Resource.Id.anchor_seek_bar);
            _foldingLayout = FindViewById<FoldingLayout>(Resource.Id.fold_view);

            _foldingLayout.FoldEnded += (s, e) => Log.Info(Tag, "Fold Ended");
            _foldingLayout.FoldStarted += (s, e) => Log.Info(Tag, "Fold Started");

            _anchorSeekBar.StopTrackingTouch += (s, e) =>
            {
                _anchorFactor = _anchorSeekBar.Progress / 100.0f;
                _foldingLayout.AnchorFactor = _anchorFactor;
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.fold, menu);

            var spinnerItem = menu.FindItem(Resource.Id.num_of_folds);
            var spinner = (Spinner) MenuItemCompat.GetActionView(spinnerItem);

            spinner.ItemSelected += (s, e) =>
            {
                _numberOfFolds = int.Parse(e.Parent.GetItemAtPosition(e.Position).ToString());

                if (!_didLoadSpinner)
                    _didLoadSpinner = true;
                else
                    _foldingLayout.NumberOfFolds = _numberOfFolds;
            };

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.toggle_orientation:
                    _orientation = (_orientation == Orientation.Horizontal)
                        ? Orientation.Vertical
                        : Orientation.Horizontal;
                    item.SetTitle((_orientation == Orientation.Horizontal)
                        ? Resource.String.vertical
                        : Resource.String.horizontal);
			        _foldingLayout.Orientation = _orientation;
			    break;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}