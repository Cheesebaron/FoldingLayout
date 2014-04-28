using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace Sample
{
    [Activity(Label = "FoldingLayout Samples", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light")]
    public class MainActivity : ActionBarActivity 
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.main);

            var values = new[]
            {
                "FoldingLayout",
                "FoldingDrawerLayout", 
                "FoldingPaneLayout"
            };

            var adapter = new ArrayAdapter<string>(this,
                Android.Resource.Layout.SimpleListItem1, values);
            var listView = FindViewById<ListView>(Resource.Id.listView1);
            listView.Adapter = adapter;

            listView.ItemClick += (s, e) =>
            {
                Intent intent = null;
                switch (values[e.Position])
                {
                    case "FoldingLayout":
                        intent = new Intent(this, typeof(FoldingLayoutActivity));
                        break;
                    case "FoldingDrawerLayout":
                        intent = new Intent(this, typeof(FoldingDrawerLayoutActivity));
                        break;
                    case "FoldingPaneLayout":
                        intent = new Intent(this, typeof(FoldingPaneLayoutActivity));
                        break;
                }
                StartActivity(intent);
            };
        }
    }
}