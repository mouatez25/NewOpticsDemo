﻿#pragma checksum "..\..\..\Fournisseur\AjouterFournisseur.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CE3A05A71E3F10A987DD1E6CD9BA8A92BB666087"
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
using NewOptics.Stock;
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


namespace NewOptics.Fournisseur {
    
    
    /// <summary>
    /// AjouterFournisseur
    /// </summary>
    public partial class AjouterFournisseur : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\..\Fournisseur\AjouterFournisseur.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid FournVousGrid;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\Fournisseur\AjouterFournisseur.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label f;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Fournisseur\AjouterFournisseur.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtRaison;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\Fournisseur\AjouterFournisseur.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtAdresse;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Fournisseur\AjouterFournisseur.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtHoraire;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\..\Fournisseur\AjouterFournisseur.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtAutre;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\Fournisseur\AjouterFournisseur.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtemail;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\Fournisseur\AjouterFournisseur.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCreer;
        
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
            System.Uri resourceLocater = new System.Uri("/NewOptics;component/fournisseur/ajouterfournisseur.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Fournisseur\AjouterFournisseur.xaml"
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
            this.FournVousGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.f = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.txtRaison = ((System.Windows.Controls.TextBox)(target));
            
            #line 36 "..\..\..\Fournisseur\AjouterFournisseur.xaml"
            this.txtRaison.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtRaison_TextChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.txtAdresse = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.txtHoraire = ((System.Windows.Controls.TextBox)(target));
            
            #line 48 "..\..\..\Fournisseur\AjouterFournisseur.xaml"
            this.txtHoraire.KeyDown += new System.Windows.Input.KeyEventHandler(this.txtHoraire_KeyDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.txtAutre = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.txtemail = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.btnCreer = ((System.Windows.Controls.Button)(target));
            
            #line 64 "..\..\..\Fournisseur\AjouterFournisseur.xaml"
            this.btnCreer.Click += new System.Windows.RoutedEventHandler(this.btnCreer_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

