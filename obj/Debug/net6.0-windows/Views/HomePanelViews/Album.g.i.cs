﻿#pragma checksum "..\..\..\..\..\Views\HomePanelViews\Album.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5081024174B4CB449A440F239092EDDA1B32E53C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using VaultsII.Views.HomePanelViews;


namespace VaultsII.Views.HomePanelViews {
    
    
    /// <summary>
    /// Album
    /// </summary>
    public partial class Album : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Overlay;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle RectangleOverlay;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OverlayBack;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Content;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox AlbumNameDisplay;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Add;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Settings;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl Body;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.7.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VaultsII;component/views/homepanelviews/album.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.7.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Overlay = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.RectangleOverlay = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 3:
            this.OverlayBack = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
            this.OverlayBack.Click += new System.Windows.RoutedEventHandler(this.OverlayBack_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Content = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.AlbumNameDisplay = ((System.Windows.Controls.TextBox)(target));
            
            #line 44 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
            this.AlbumNameDisplay.LostFocus += new System.Windows.RoutedEventHandler(this.AlbumNameDisplay_LostFocus);
            
            #line default
            #line hidden
            
            #line 45 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
            this.AlbumNameDisplay.KeyDown += new System.Windows.Input.KeyEventHandler(this.AlbumNameDisplay_KeyDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Add = ((System.Windows.Controls.Button)(target));
            
            #line 55 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
            this.Add.Click += new System.Windows.RoutedEventHandler(this.Add_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Settings = ((System.Windows.Controls.Button)(target));
            
            #line 66 "..\..\..\..\..\Views\HomePanelViews\Album.xaml"
            this.Settings.Click += new System.Windows.RoutedEventHandler(this.Settings_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Body = ((System.Windows.Controls.ItemsControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

