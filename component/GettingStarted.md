Add `cheesebaron.folding.FoldingLayout` to your AXML layout file:

```
<cheesebaron.folding.FoldingLayout
    android:id="@+id/fold_view"
    android:layout_width="match_parent"
    android:layout_height="0dp"
    android:layout_weight="1">
    <!-- your view to fold -->
</cheesebaron.folding.FoldingLayout>
```

`FoldingLayout` supports one child, which in turn can have many of its own nested layouts and supports
any type of layout inside.

This project requires `Xamarin.Android.Support.v4` referenced to the project. Grab the latest version
in the [component store](http://components.xamarin.com/view/xamandroidsupportv4-18).

FoldingLayout also comes with a custom implementation of the `DrawerLayout` which adds the folding effect.
Instead of using `android.support.v4.widget.DrawerLayout` you can use `cheesebaron.folding.FoldingDrawerLayout`,
which adds the folding effect to the drawer:

```
<cheesebaron.folding.FoldingDrawerLayout xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:sliding="http://schemas.android.com/apk/res-auto"
    android:id="@+id/drawer_layout"
    android:layout_width="match_parent"
    android:layout_height="match_parent">
    <FrameLayout
        android:id="@+id/content_frame"
        android:layout_width="match_parent"
        android:layout_height="match_parent" />
    <ListView
        android:id="@+id/left_drawer"
        android:layout_width="240dp"
        android:layout_height="match_parent"
        android:layout_gravity="start"
        android:choiceMode="singleChoice"
        android:divider="@android:color/transparent"
        android:dividerHeight="0dp"
        android:background="#ddd" />
</cheesebaron.folding.FoldingDrawerLayout>
```

It also comes with a custom implementation of the `SlidingPaneLayout`, which like the `DrawerLayout`
adds the folding effect to the `SlidingPaneLayout`, for this use `cheesebaron.folding.FoldingPaneLayout`:

```
<cheesebaron.folding.FoldingPaneLayout xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:folding="http://schemas.android.com/apk/res-auto"
    android:id="@+id/sliding_pane_layout"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    folding:numberOfFolds="3">
    <ListView
        android:id="@+id/left_pane"
        android:layout_width="280dp"
        android:layout_height="match_parent"
        android:layout_gravity="left" />
    <ScrollView
        android:layout_width="300dp"
        android:layout_height="match_parent"
        android:layout_weight="1"
        android:paddingLeft="16dp"
        android:paddingRight="16dp"
        android:scrollbarStyle="outsideOverlay"
        android:background="#ff999999">
        <TextView
            android:id="@+id/content_text"
            android:layout_width="match_parent"
            android:layout_height="match_parent"
            android:text="@string/sliding_pane_layout_summary"
            android:textAppearance="?android:attr/textAppearanceMedium" />
    </ScrollView>
</cheesebaron.folding.FoldingPaneLayout>
```

Both the `cheesebaron.folding.FoldingDrawerLayout` and `cheesebaron.folding.FoldingPaneLayout` otherwise
work as `DrawerLayout` and `SlidingPaneLayout` otherwise would work.

Attributes
==========

All three layouts allow for using the `numberOfFolds` attribute, which is used as follows:

```
<cheesebaron.folding.FoldingPaneLayout xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:folding="http://schemas.android.com/apk/res-auto"
    folding:numberOfFolds="3"
    ...
```

For specific usage see the provided sample app.