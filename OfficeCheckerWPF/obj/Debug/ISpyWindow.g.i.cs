﻿#pragma checksum "..\..\ISpyWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "D113E161CB51BA2BAF514626C7B5AA7697B041A9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using OfficeCheckerWPF;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace OfficeCheckerWPF {
    
    
    /// <summary>
    /// SpyWindow
    /// </summary>
    public partial class SpyWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\ISpyWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox CamModelBox;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\ISpyWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CamIpAddressTextBox;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\ISpyWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CamLoginTextBox;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\ISpyWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CamPasswordTextBox;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\ISpyWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BaseTestButton;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\ISpyWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button XmlSettingButton;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\ISpyWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BackupSettingButton;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\ISpyWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BackupRestoreSettingButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/OfficeCheckerWPF;component/ispywindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ISpyWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\ISpyWindow.xaml"
            ((OfficeCheckerWPF.SpyWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.WindowLoaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.CamModelBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.CamIpAddressTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.CamLoginTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.CamPasswordTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.BaseTestButton = ((System.Windows.Controls.Button)(target));
            
            #line 93 "..\..\ISpyWindow.xaml"
            this.BaseTestButton.Click += new System.Windows.RoutedEventHandler(this.BaseTestButtonClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.XmlSettingButton = ((System.Windows.Controls.Button)(target));
            
            #line 94 "..\..\ISpyWindow.xaml"
            this.XmlSettingButton.Click += new System.Windows.RoutedEventHandler(this.XmlSettingButtonClick);
            
            #line default
            #line hidden
            return;
            case 8:
            this.BackupSettingButton = ((System.Windows.Controls.Button)(target));
            
            #line 95 "..\..\ISpyWindow.xaml"
            this.BackupSettingButton.Click += new System.Windows.RoutedEventHandler(this.BackupSettingButtonClick);
            
            #line default
            #line hidden
            return;
            case 9:
            this.BackupRestoreSettingButton = ((System.Windows.Controls.Button)(target));
            
            #line 96 "..\..\ISpyWindow.xaml"
            this.BackupRestoreSettingButton.Click += new System.Windows.RoutedEventHandler(this.BackupRestoreSettingButtonClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

