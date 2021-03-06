﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace DynamicConfig.ConfigTray
{
    /// <summary>
    /// Class implementing support for "minimize to tray" functionality.
    /// </summary>
    internal static class MinimizeToTray
    {
        private static MinimizeToTrayInstance instance;

        /// <summary>
        /// Enables "minimize to tray" behavior for the specified Window.
        /// </summary>
        /// <param name="window">Window to enable the behavior for.</param>
        public static void Enable(Window window)
        {
            // No need to track this instance; its event handlers will keep it alive
            instance = new MinimizeToTrayInstance(window);
        }

        public static void DisposeNotifyIcon()
        {
            if(instance != null)
                instance.Dispose();

           
        }

        /// <summary>
        /// Class implementing "minimize to tray" functionality for a Window instance.
        /// </summary>
        private class MinimizeToTrayInstance : IDisposable
        {
            private Window _window;

            private NotifyIcon _notifyIcon;

            private bool _balloonShown;

            private ContextMenu _ctmStrip;

            public MinimizeToTrayInstance(Window window)
            {
                _window = window;
                _window.StateChanged += new EventHandler(HandleStateChanged);

                _ctmStrip = new ContextMenu();
                var menuItem = new MenuItem();
                menuItem.Index = 0;
                menuItem.Text = "Exit";
                menuItem.Click += MenuItemOnClick;
                _ctmStrip.MenuItems.AddRange(new MenuItem[] { menuItem });
            }

            private void MenuItemOnClick(object sender, EventArgs eventArgs)
            {
                _window.Close();
            }

            /// <summary>
            /// Handles the Window's StateChanged event.
            /// </summary>
            /// <param name="sender">Event source.</param>
            /// <param name="e">Event arguments.</param>
            private void HandleStateChanged(object sender, EventArgs e)
            {
                if (_notifyIcon == null)
                {
                    // Initialize NotifyIcon instance "on demand"
                    _notifyIcon = new NotifyIcon();
                    _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

                    _notifyIcon.ContextMenu = _ctmStrip;

                    _notifyIcon.MouseClick += new MouseEventHandler(HandleNotifyIconOrBalloonClicked);
                    _notifyIcon.BalloonTipClicked += new EventHandler(HandleNotifyIconOrBalloonClicked);
                }

                // Update copy of Window Title in case it has changed
                _notifyIcon.Text = _window.Title;

                // Show/hide Window and NotifyIcon
                var minimized = (_window.WindowState == WindowState.Minimized);
                _window.ShowInTaskbar = !minimized;
                _notifyIcon.Visible = minimized;
                if (minimized && !_balloonShown)
                {
                    // If this is the first time minimizing to the tray, show the user what happened
                    _notifyIcon.ShowBalloonTip(1000, null, _window.Title, ToolTipIcon.None);
                    _balloonShown = true;
                }
            }

            /// <summary>
            /// Handles a click on the notify icon or its balloon.
            /// </summary>
            /// <param name="sender">Event source.</param>
            /// <param name="e">Event arguments.</param>
            private void HandleNotifyIconOrBalloonClicked(object sender, EventArgs e)
            {
                // Restore the Window
                _window.WindowState = WindowState.Normal;
            }

            public void Dispose()
            {
                _notifyIcon?.Dispose();
                _ctmStrip?.Dispose();
            }

            ~MinimizeToTrayInstance()
            {
                Dispose();
            }
        }
    }
}