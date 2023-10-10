using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Project_Radon;
using Project_Radon.Settings;
using Windows.Storage;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Foundation.Collections;

// TODO: Import Cubekit.UI (Firecube's GlowUI refer https://github.com/FireCubeStudios/TemplateApp)

namespace Yttrium_browser
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;

            object value = ApplicationData.Current.LocalSettings.Values["themeSetting"];

            if (value != null)
            {
                // Apply theme choice.
                App.Current.RequestedTheme = (ApplicationTheme)(int)value;
            }

        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                    // profile check mechanisms
                    string username = localSettings.Values["username"] as string;
                    if (username == null)
                    {
                        rootFrame.Navigate(typeof(oobe1), null);
                    }

                    else { rootFrame.Navigate(typeof(MainPage), e.Arguments); }
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter

                    
                }
                // Ensure the current window is active
                Window.Current.Activate();

                //titlebar code
                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                //coreTitleBar.ExtendViewIntoTitleBar = true;
                

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                if (coreTitleBar.ExtendViewIntoTitleBar == true)
                {
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                    titleBar.BackgroundColor = Colors.Transparent;
                }

                else
                {
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                    titleBar.BackgroundColor = Colors.Transparent;
                }

                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.BackgroundColor = Colors.Transparent;



                // set min window size
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
                ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));
                

                
            }
            
            
        }


        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            // Handle notification activation
            if (e is ToastNotificationActivatedEventArgs toastActivationArgs)
            {
                // Obtain the arguments from the notification
                ToastArguments args = ToastArguments.Parse(toastActivationArgs.Argument);

                // Obtain any user input (text boxes, menu selections) from the notification
                ValueSet userInput = toastActivationArgs.UserInput;

                // TODO: Show the corresponding content
                
            }
        }
    }
}
