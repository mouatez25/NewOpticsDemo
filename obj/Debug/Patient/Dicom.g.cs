﻿#pragma checksum "..\..\..\Patient\Dicom.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A7D2918A0E404581C25EB0D8BFE7CE5B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DevExpress.Core;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.DataSources;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Core.ServerMode;
using DevExpress.Xpf.DXBinding;
using GestionClinique.Patient;
using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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


namespace GestionClinique.Patient {
    
    
    /// <summary>
    /// Dicom
    /// </summary>
    public partial class Dicom : DevExpress.Xpf.Core.DXWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 26 "..\..\..\Patient\Dicom.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal GestionClinique.Patient.ImagePanelControl imagePanelControl1;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\Patient\Dicom.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal GestionClinique.Patient.WindowLevelGraphControl windowLevelControl;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Patient\Dicom.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton rbZoom1_1;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\Patient\Dicom.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton rbZoomfit;
        
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
            System.Uri resourceLocater = new System.Uri("/Diabetosoft;component/patient/dicom.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Patient\Dicom.xaml"
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
            this.imagePanelControl1 = ((GestionClinique.Patient.ImagePanelControl)(target));
            return;
            case 2:
            this.windowLevelControl = ((GestionClinique.Patient.WindowLevelGraphControl)(target));
            return;
            case 3:
            this.rbZoom1_1 = ((System.Windows.Controls.RadioButton)(target));
            
            #line 34 "..\..\..\Patient\Dicom.xaml"
            this.rbZoom1_1.Checked += new System.Windows.RoutedEventHandler(this.rbZoom1_1_Checked);
            
            #line default
            #line hidden
            return;
            case 4:
            this.rbZoomfit = ((System.Windows.Controls.RadioButton)(target));
            
            #line 35 "..\..\..\Patient\Dicom.xaml"
            this.rbZoomfit.Checked += new System.Windows.RoutedEventHandler(this.rbZoomfit_Checked);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
