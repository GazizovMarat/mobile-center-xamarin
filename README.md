# Mobile center xamarin demo app
Demo mobile application for mobile center

## Setting up enviroment
* Install Visual Studio 2017
  * Select toolset **"Mobile development with .Net"**  
* Install Android SDK 
  * Android 7.1.1 (API 25) - SDK Platform
  * Google Play services
  * Google Repository

* Setting up android-sdk for Xamarin
  * Move your android-sdk to forlder withour spaces in path and update path to sdk in Xamarin
  * Download latest version of [Proguard](https://sourceforge.net/projects/proguard/files/proguard/)
  * Replace proguard files in Android-sdk (.\android-sdk\tools\proguard)

* Download xamarin components and add references in projects
  * [OxyPlot](https://components.xamarin.com/view/oxyplot) - Chart component
  * [Json.Net](https://components.xamarin.com/view/json.net)
  * [Xamarin.Auth](https://components.xamarin.com/view/xamarin.auth)