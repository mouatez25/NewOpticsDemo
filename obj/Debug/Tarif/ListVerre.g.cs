﻿#pragma checksum "..\..\..\Tarif\ListVerre.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "B940D3C331D1E40E473BE2F49EAAC66DC1E76E60"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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


namespace NewOptics.Tarif {
    
    
    /// <summary>
    /// ListVerre
    /// </summary>
    public partial class ListVerre : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 34 "..\..\..\Tarif\ListVerre.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox MarqueCombo;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\Tarif\ListVerre.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chstockVerreTeinte;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\Tarif\ListVerre.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chstockVerreAphaque;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\Tarif\ListVerre.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chstockVerrePhotochromique;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\Tarif\ListVerre.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnFilter;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\Tarif\ListVerre.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnGroupeStock;
        
        #line default
        #line hidden
        
        
        #line 167 "..\..\..\Tarif\ListVerre.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnImprimerStock;
        
        #line default
        #line hidden
        
        
        #line 231 "..\..\..\Tarif\ListVerre.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid PatientDataGrid;
        
        #line default
        #line hidden
        
        
        #line 483 "..\..\..\Tarif\ListVerre.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel Footer;
        
        #line default
        #line hidden
        
        
        #line 494 "..\..\..\Tarif\ListVerre.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock label;
        
        #line default
        #line hidden
        
        
        #line 495 "..\..\..\Tarif\ListVerre.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtRecherche;
        
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
            System.Uri resourceLocater = new System.Uri("/NewOptics;component/tarif/listverre.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Tarif\ListVerre.xaml"
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
            this.MarqueCombo = ((System.Windows.Controls.ComboBox)(target));
            
            #line 34 "..\..\..\Tarif\ListVerre.xaml"
            this.MarqueCombo.MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.MarqueCombo_MouseRightButtonUp);
            
            #line default
            #line hidden
            return;
            case 2:
            this.chstockVerreTeinte = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 3:
            this.chstockVerreAphaque = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 4:
            this.chstockVerrePhotochromique = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.btnFilter = ((System.Windows.Controls.Button)(target));
            
            #line 45 "..\..\..\Tarif\ListVerre.xaml"
            this.btnFilter.Click += new System.Windows.RoutedEventHandler(this.btnFilter_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnGroupeStock = ((System.Windows.Controls.Button)(target));
            
            #line 104 "..\..\..\Tarif\ListVerre.xaml"
            this.btnGroupeStock.Click += new System.Windows.RoutedEventHandler(this.btnGroupeStock_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnImprimerStock = ((System.Windows.Controls.Button)(target));
            
            #line 167 "..\..\..\Tarif\ListVerre.xaml"
            this.btnImprimerStock.Click += new System.Windows.RoutedEventHandler(this.btnImprimerStock_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.PatientDataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 231 "..\..\..\Tarif\ListVerre.xaml"
            this.PatientDataGrid.MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.PatientDataGrid_MouseRightButtonUp);
            
            #line default
            #line hidden
            
            #line 231 "..\..\..\Tarif\ListVerre.xaml"
            this.PatientDataGrid.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.PatientDataGrid_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Footer = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 10:
            this.label = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            this.txtRecherche = ((System.Windows.Controls.TextBox)(target));
            
            #line 495 "..\..\..\Tarif\ListVerre.xaml"
            this.txtRecherche.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtRecherche_TextChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

