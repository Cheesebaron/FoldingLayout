:: Grab xpkg from http://components.xamarin.com/submit/xpkg and put contents of the zip into xpkg

xpkg\xamarin-component.exe create-manually foldinglayout-1.0.0.xam ^
	--name="FoldingLayout" ^
	--summary="Add a layout, which folds when you scroll your finger across it!" ^
	--website="https://github.com/Cheesebaron/FoldingLayout" ^
	--details="Details.md" ^
	--license="License.md" ^
	--getting-started="GettingStarted.md" ^
	--icon="icons/foldinglayout_128x128.png" ^
	--icon="icons/foldinglayout_512x512.png" ^
	--library="android":"../bin/Debug/Cheesebaron.Folding.dll" ^
	--publisher "Cheesebaron" ^
	--sample="Android Sample. Demonstrates FoldingLayout":"../src/Folding.sln"