Add a layout, which folds up when you scroll your finger across it!

## Key features

- Customizable number of folds
- Customizable anchor point
- DrawerLayout implementation, which adds the folding effect to the drawer
- SlidingPaneLayout implementation, which as with DrawerLayout adds the effect to the Sliding Pane.

## Requirements

This library uses Android Support v4, and it is tested on Android 2.2 and above.

## Usage

To use it, add the component and in your layout simply wrap your layouts with `cheesebaron.folding.FoldingLayout`. It supports one child, but that
child can have as many nested children as you want.

```
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:tools="http://schemas.android.com/tools"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:orientation="vertical">
    <cheesebaron.folding.FoldingLayout
        android:id="@+id/fold_view"
        android:layout_width="match_parent"
        android:layout_height="0dp"
        android:layout_weight="1">
        <ImageView
            android:id="@+id/image_view"
            android:layout_width="match_parent"
            android:layout_height="match_parent"
            android:scaleType="fitXY" />
    </cheesebaron.folding.FoldingLayout>
    
    <!-- other layouts here -->
</LinearLayout>
```

The project is Open Source and can be [forked on GitHub](https://github.com/Cheesebaron/FoldingLayout).
