﻿#pragma checksum "..\..\ManualEdit.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "ABE2F54EB0B1046308D3C8E7DCE2638075131EDC7BC40F7DBE1EF980EE0A8530"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DDrop.Controls.PixelDrawer;
using DDrop.Controls.Zoomborder;
using DDrop.Utility.ImageOperations;
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


namespace DDrop {
    
    
    /// <summary>
    /// ManualEdit
    /// </summary>
    public partial class ManualEdit : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\ManualEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DDrop.ManualEdit AppEditWindow;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\ManualEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SaveInputPhotoEditButton;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\ManualEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid PhotoForEdit;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\ManualEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PixelsInMillimeterHorizontalTextBox;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\ManualEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton HorizontalRulerToggleButton;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\ManualEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PixelsInMillimeterVerticalTextBox;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\ManualEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton VerticalRulerToggleButton;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\ManualEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border Border;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\ManualEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DDrop.Controls.Zoomborder.ZoomBorder ZoomBorder;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\ManualEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DDrop.Controls.PixelDrawer.PixelDrawer EditWindowPixelDrawer;
        
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
            System.Uri resourceLocater = new System.Uri("/DDrop;component/manualedit.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ManualEdit.xaml"
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
            this.AppEditWindow = ((DDrop.ManualEdit)(target));
            
            #line 10 "..\..\ManualEdit.xaml"
            this.AppEditWindow.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.SaveInputPhotoEditButton = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\ManualEdit.xaml"
            this.SaveInputPhotoEditButton.Click += new System.Windows.RoutedEventHandler(this.SaveInputPhotoEditButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.PhotoForEdit = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 4:
            this.PixelsInMillimeterHorizontalTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.HorizontalRulerToggleButton = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            
            #line 32 "..\..\ManualEdit.xaml"
            this.HorizontalRulerToggleButton.Checked += new System.Windows.RoutedEventHandler(this.HorizontalRulerToggleButton_Checked);
            
            #line default
            #line hidden
            
            #line 32 "..\..\ManualEdit.xaml"
            this.HorizontalRulerToggleButton.Unchecked += new System.Windows.RoutedEventHandler(this.HorizontalRulerToggleButton_Unchecked);
            
            #line default
            #line hidden
            return;
            case 6:
            this.PixelsInMillimeterVerticalTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.VerticalRulerToggleButton = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            
            #line 43 "..\..\ManualEdit.xaml"
            this.VerticalRulerToggleButton.Checked += new System.Windows.RoutedEventHandler(this.VerticalRulerToggleButton_Checked);
            
            #line default
            #line hidden
            
            #line 43 "..\..\ManualEdit.xaml"
            this.VerticalRulerToggleButton.Unchecked += new System.Windows.RoutedEventHandler(this.VerticalRulerToggleButton_Unchecked);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Border = ((System.Windows.Controls.Border)(target));
            return;
            case 9:
            this.ZoomBorder = ((DDrop.Controls.Zoomborder.ZoomBorder)(target));
            return;
            case 10:
            this.EditWindowPixelDrawer = ((DDrop.Controls.PixelDrawer.PixelDrawer)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
