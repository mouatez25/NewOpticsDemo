﻿#pragma checksum "..\..\..\Comptoir\DetailVente.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7DBD8E04F3DFBAB2B1813657B269AD2C58175603"
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
using NewOptics.Comptoir;
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


namespace NewOptics.Comptoir {
    
    
    /// <summary>
    /// DetailVente
    /// </summary>
    public partial class DetailVente : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\Comptoir\DetailVente.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridvente;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\Comptoir\DetailVente.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label f;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Comptoir\DetailVente.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtdesign;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\Comptoir\DetailVente.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtQuantité;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\Comptoir\DetailVente.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtPrix;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Comptoir\DetailVente.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CONFIRMERVENTE;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\Comptoir\DetailVente.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button annulerVENTE;
        
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
            System.Uri resourceLocater = new System.Uri("/NewOptics;component/comptoir/detailvente.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Comptoir\DetailVente.xaml"
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
            
            #line 12 "..\..\..\Comptoir\DetailVente.xaml"
            ((NewOptics.Comptoir.DetailVente)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.DXWindow_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.gridvente = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.f = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.txtdesign = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.txtQuantité = ((System.Windows.Controls.TextBox)(target));
            
            #line 31 "..\..\..\Comptoir\DetailVente.xaml"
            this.txtQuantité.KeyDown += new System.Windows.Input.KeyEventHandler(this.txtQuantité_KeyDown);
            
            #line default
            #line hidden
            
            #line 31 "..\..\..\Comptoir\DetailVente.xaml"
            this.txtQuantité.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtQuantité_TextChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.txtPrix = ((System.Windows.Controls.TextBox)(target));
            
            #line 33 "..\..\..\Comptoir\DetailVente.xaml"
            this.txtPrix.KeyDown += new System.Windows.Input.KeyEventHandler(this.txtPrix_KeyDown);
            
            #line default
            #line hidden
            
            #line 33 "..\..\..\Comptoir\DetailVente.xaml"
            this.txtPrix.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtPrix_TextChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.CONFIRMERVENTE = ((System.Windows.Controls.Button)(target));
            
            #line 34 "..\..\..\Comptoir\DetailVente.xaml"
            this.CONFIRMERVENTE.Click += new System.Windows.RoutedEventHandler(this.CONFIRMERVENTE_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.annulerVENTE = ((System.Windows.Controls.Button)(target));
            
            #line 77 "..\..\..\Comptoir\DetailVente.xaml"
            this.annulerVENTE.Click += new System.Windows.RoutedEventHandler(this.annulerVENTE_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

