// Updated by XamlIntelliSenseFileGenerator 9/27/2023 4:50:14 PM
#pragma checksum "..\..\..\..\..\Views\HomePanelViews\Startadssad.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "B13B77B481A8338AC5725D859C49AEFFDF2D5CA1"
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
    /// StartView
    /// </summary>
    public partial class StartView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {

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
            System.Uri resourceLocater = new System.Uri("/VaultsII;V1.0.0.0;component/views/homepanelviews/startadssad.xaml", System.UriKind.Relative);

#line 1 "..\..\..\..\..\Views\HomePanelViews\Startadssad.xaml"
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
            switch (connectionId) {
                case 1:
                    this.AddMonitoredFolder = ((System.Windows.Controls.Button)(target));

#line 20 "..\..\..\..\..\Views\HomePanelViews\Startadssad.xaml"
                    this.AddMonitoredFolder.Click += new System.Windows.RoutedEventHandler(this.AddMonitoredFolder_Click);

#line default
#line hidden
                    return;
                case 2:
                    this.RemoveMonitoredFolder = ((System.Windows.Controls.Button)(target));

#line 25 "..\..\..\..\..\Views\HomePanelViews\Startadssad.xaml"
                    this.RemoveMonitoredFolder.Click += new System.Windows.RoutedEventHandler(this.RemoveMonitoredFolder_Click);

#line default
#line hidden
                    return;
                case 3:
                    this.CreateNewAlbum = ((System.Windows.Controls.Button)(target));

#line 30 "..\..\..\..\..\Views\HomePanelViews\Startadssad.xaml"
                    this.CreateNewAlbum.Click += new System.Windows.RoutedEventHandler(this.CreateNewAlbum_Click);

#line default
#line hidden
                    return;
                case 4:
                    this.SortNewContent = ((System.Windows.Controls.Button)(target));

#line 35 "..\..\..\..\..\Views\HomePanelViews\Startadssad.xaml"
                    this.SortNewContent.Click += new System.Windows.RoutedEventHandler(this.SortNewContent_Click);

#line default
#line hidden
                    return;
                case 5:
                    this.MonitoredFoldersListOne = ((System.Windows.Controls.TextBlock)(target));
                    return;
            }
            this._contentLoaded = true;
        }
    }
}

