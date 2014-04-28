FoldingLayout
=============

A port of https://github.com/tibi1712/Folding-Android


Add a layout, which folds up when you scroll your finger across it!

![screenshot](https://raw.githubusercontent.com/Cheesebaron/FoldingLayout/master/component/screenshots/foldinglayout_folded_anchored_thumb.png)&nbsp;
![screenshot](https://raw.githubusercontent.com/Cheesebaron/FoldingLayout/master/component/screenshots/foldinglayout_horizontal_thumb.png)&nbsp;
![screenshot](https://raw.githubusercontent.com/Cheesebaron/FoldingLayout/master/component/screenshots/foldingdrawerlayout_thumb.png)

## Key features

- Customizable number of folds
- Customizable anchor point
- DrawerLayout implementation, which adds the folding effect to the drawer
- SlidingPaneLayout implementation, which as with DrawerLayout adds the effect to the Sliding Pane.

## Requirements

This library uses Android Support v4, and it is tested on Android 2.2 and above.

## Usage

To use it, add the component and in your layout simply wrap your layout with `cheesebaron.folding.FoldingLayout`. It supports one child, but that
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

Se sample app for more examples.

License
=======

```
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```
