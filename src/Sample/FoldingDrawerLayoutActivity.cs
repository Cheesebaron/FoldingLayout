using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Cheesebaron.Folding;

namespace Sample
{
    [Activity(Label = "FoldingDrawerLayout Sample", MainLauncher = false, Theme = "@style/Theme.AppCompat.Light")]
    public class FoldingDrawerLayoutActivity : ActionBarActivity
    {
        private FoldingDrawerLayout _drawer;
        private MyActionBarDrawerToggle _drawerToggle;
        private ListView _drawerList;

        private string _drawerTitle;
        private string _title;
        private string[] _planetTitles;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_drawer);

            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _title = _drawerTitle = Title;
            _planetTitles = Resources.GetStringArray(Resource.Array.PlanetsArray);
            _drawer = FindViewById<FoldingDrawerLayout>(Resource.Id.drawer_layout);
            _drawerList = FindViewById<ListView>(Resource.Id.left_drawer);

            _drawerList.Adapter = new ArrayAdapter<string>(this,
                Resource.Layout.drawer_listitem, _planetTitles);
            _drawerList.ItemClick += (sender, args) => SelectItem(args.Position);

            _drawerToggle = new MyActionBarDrawerToggle(this, _drawer,
                                                      Resource.Drawable.ic_drawer_light,
                                                      Resource.String.drawer_open,
                                                      Resource.String.drawer_close);

            _drawerToggle.DrawerClosed += delegate
            {
                ActionBar.Title = _title;
                InvalidateOptionsMenu();
            };

            _drawerToggle.DrawerOpened += delegate
            {
                ActionBar.Title = _drawerTitle;
                InvalidateOptionsMenu();
            };

            _drawer.SetDrawerListener(_drawerToggle);

            if (null == savedInstanceState)
                SelectItem(0);
        }

        private void SelectItem(int position)
        {
            var fragment = new PlanetFragment();
            var arguments = new Bundle();
            arguments.PutInt(PlanetFragment.ArgPlanetNumber, position);
            fragment.Arguments = arguments;

            FragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();

            _drawerList.SetItemChecked(position, true);
            ActionBar.Title = _title = _planetTitles[position];
            _drawer.CloseDrawer(_drawerList);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            _drawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            _drawerToggle.OnConfigurationChanged(newConfig);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.spinner, menu);

            var spinnerItem = menu.FindItem(Resource.Id.num_of_folds);
            var spinner = (Spinner)MenuItemCompat.GetActionView(spinnerItem);

            spinner.ItemSelected += (s, e) =>
            {
                var numberOfFolds = int.Parse(e.Parent.GetItemAtPosition(e.Position).ToString());

                var fold = _drawer.GetFoldingLayout(_drawerList);
                if (fold != null)
                    fold.NumberOfFolds = numberOfFolds;
            };

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return _drawerToggle.OnOptionsItemSelected(item) || base.OnOptionsItemSelected(item);
        }
    }
}