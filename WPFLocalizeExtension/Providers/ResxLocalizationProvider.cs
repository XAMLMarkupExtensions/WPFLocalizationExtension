﻿#region Copyright information
// <copyright file="ResxLocalizationProvider.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     http://wpflocalizeextension.codeplex.com/license
// </copyright>
// <author>Uwe Mayer</author>
#endregion

#if SILVERLIGHT
namespace SLLocalizeExtension.Providers
#else
namespace WPFLocalizeExtension.Providers
#endif
{
    #region Uses
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Resources;
    using System.Reflection;
    using System.Globalization;
    using System.IO;
    using System.Collections.ObjectModel;
    using System.Windows.Media;
    using XAMLMarkupExtensions.Base;
    #endregion

    /// <summary>
    /// A singleton RESX provider that uses attached properties and the Parent property to iterate through the visual tree.
    /// </summary>
    public class ResxLocalizationProvider : ResxLocalizationProviderBase
    {
        #region Dependency Properties
        /// <summary>
        /// <see cref="DependencyProperty"/> DefaultDictionary to set the fallback resource dictionary.
        /// </summary>
        public static readonly DependencyProperty DefaultDictionaryProperty =
                DependencyProperty.RegisterAttached(
                "DefaultDictionary",
                typeof(string),
                typeof(ResxLocalizationProvider),
                new PropertyMetadata(null, AttachedPropertyChanged));

        /// <summary>
        /// <see cref="DependencyProperty"/> DefaultAssembly to set the fallback assembly.
        /// </summary>
        public static readonly DependencyProperty DefaultAssemblyProperty =
            DependencyProperty.RegisterAttached(
                "DefaultAssembly",
                typeof(string),
                typeof(ResxLocalizationProvider),
                new PropertyMetadata(null, AttachedPropertyChanged));
        #endregion

        #region Dependency Property Callback
        /// <summary>
        /// Indicates, that one of the attached properties changed.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">The event argument.</param>
        private static void AttachedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Instance.OnProviderChanged(obj);
        }
        #endregion

        #region Dependency Property Management
        #region Get
        /// <summary>
        /// Getter of <see cref="DependencyProperty"/> default dictionary.
        /// </summary>
        /// <param name="obj">The dependency object to get the default dictionary from.</param>
        /// <returns>The default dictionary.</returns>
        public static string GetDefaultDictionary(DependencyObject obj)
        {
            return (obj != null) ? (string)obj.GetValue(DefaultDictionaryProperty) : null;
        }

        /// <summary>
        /// Getter of <see cref="DependencyProperty"/> default assembly.
        /// </summary>
        /// <param name="obj">The dependency object to get the default assembly from.</param>
        /// <returns>The default assembly.</returns>
        public static string GetDefaultAssembly(DependencyObject obj)
        {
            return (obj != null) ? (string)obj.GetValue(DefaultAssemblyProperty) : null;
        }
        #endregion

        #region Set
        /// <summary>
        /// Setter of <see cref="DependencyProperty"/> default dictionary.
        /// </summary>
        /// <param name="obj">The dependency object to set the default dictionary to.</param>
        /// <param name="value">The dictionary.</param>
        public static void SetDefaultDictionary(DependencyObject obj, string value)
        {
            obj.SetValue(DefaultDictionaryProperty, value);
        }

        /// <summary>
        /// Setter of <see cref="DependencyProperty"/> default assembly.
        /// </summary>
        /// <param name="obj">The dependency object to set the default assembly to.</param>
        /// <param name="value">The assembly.</param>
        public static void SetDefaultAssembly(DependencyObject obj, string value)
        {
            obj.SetValue(DefaultAssemblyProperty, value);
        }
        #endregion
        #endregion

        #region Variables
        /// <summary>
        /// A dictionary for notification classes for changes of the individual target Parent changes.
        /// </summary>
        private Dictionary<DependencyObject, ParentChangedNotifier> parentNotifiers = new Dictionary<DependencyObject, ParentChangedNotifier>();
        #endregion

        #region Singleton Variables, Properties & Constructor
        /// <summary>
        /// The instance of the singleton.
        /// </summary>
        private static ResxLocalizationProvider instance;

        /// <summary>
        /// Lock object for the creation of the singleton instance.
        /// </summary>
        private static readonly object InstanceLock = new object();

        /// <summary>
        /// Gets the <see cref="ResxLocalizationProvider"/> singleton.
        /// </summary>
        public static ResxLocalizationProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (instance == null)
                            instance = new ResxLocalizationProvider();
                    }
                }

                // return the existing/new instance
                return instance;
            }
        }
		
		/// <summary>
        /// Resets the instance that is used for the ResxLocationProvider
        /// </summary>
        public static void Reset()
       {
          instance = null;
       }

        /// <summary>
        /// The singleton constructor.
        /// </summary>
        private ResxLocalizationProvider()
        {
            ResourceManagerList = new Dictionary<string, ResourceManager>();
            AvailableCultures = new ObservableCollection<CultureInfo>();
            AvailableCultures.Add(CultureInfo.InvariantCulture);
        }
        #endregion

        #region Abstract assembly & dictionary lookup
        /// <summary>
        /// An action that will be called when a parent of one of the observed target objects changed.
        /// </summary>
        /// <param name="obj">The target <see cref="DependencyObject"/>.</param>
        private void ParentChangedAction(DependencyObject obj)
        {
            OnProviderChanged(obj);
        }

        /// <summary>
        /// Get the assembly from the context, if possible.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>The assembly name, if available.</returns>
        protected override string GetAssembly(DependencyObject target)
        {
            if (target == null)
                return null;

            return target.GetValueOrRegisterParentNotifier<string>(ResxLocalizationProvider.DefaultAssemblyProperty, ParentChangedAction, parentNotifiers); 
        }

        /// <summary>
        /// Get the dictionary from the context, if possible.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>The dictionary name, if available.</returns>
        protected override string GetDictionary(DependencyObject target)
        {
            if (target == null)
                return null;

            return target.GetValueOrRegisterParentNotifier<string>(ResxLocalizationProvider.DefaultDictionaryProperty, ParentChangedAction, parentNotifiers);
        }
        #endregion
    }
}
