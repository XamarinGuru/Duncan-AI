

using Android.Util;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;


namespace Reino.ClientConfig
{   
	/// <summary>
	/// Base class for objects that will be read from a file and created dynamically.
	/// (ISSUE_AP.XML or HOST_ISSUE_AP.XML)
	///  
	/// To prevent individual fields/properties from being included by the XML Serializer, 
	/// use the "XmlIgnoreAttribute" attribute.
	/// </summary>
	
	public class TObjBase:
		IComparable<TObjBase>, IEquatable<TObjBase>
	{
		#region Properties and Members
		internal string _Name = "";
		public string Name
		{
			get { return _Name; }
			set 
            { 
                // There are some objects that shouldn't have the name upper-cased.
                // TSheet doesn't have a text buffer or display name, so we need to
                // use the Name property which is allowed (preferred) to be mixed case.

                if (
                    (this is TSheet)
#if __ANDROID__
                    // android SQLite column names are case sensitive - leave them as named
                    || (this is TTableFldDef)
#endif
                    )
                    _Name = value;
                else
                    _Name = value.ToUpper();
            }
		}
        [XmlIgnoreAttribute]
        public TObjBase Parent;

        // This is used to replace property values from the registry.
        public List<string> RegistryItems = new List<string>();
		#endregion



		/// <summary>
		/// Helper function to determine if a particular class is of the same type or a descendant of this class
		/// </summary>
		public virtual bool IsAClassMember( Type DesiredType )
		{
			return (DesiredType.IsAssignableFrom(this.GetType()));
		}

		/// <summary>
		/// Determines whether two TObjBase objects have the same name.
		/// (For supporting the IEquatable interface)
		/// </summary>
		public virtual bool Equals(TObjBase obj)
		{
			return _Name.Equals(obj.Name);
		}

		/// <summary>
		/// Compares this instance to a specified object and returns an indication of their relative values.
		/// (For supporting the IComparable interface)
		/// </summary>
		public virtual int CompareTo(TObjBase obj)
		{
			return _Name.CompareTo(obj.Name);
		}

        /// <summary>
        /// Called after the object has been created by the deserializer.  Gives the object the
        /// opportunity to resolve and object instances. For example, can set its _Parent
        /// property.
        /// </summary>
        public virtual int PostDeserialize(TObjBase iParent)
        {
            Parent = iParent;
            return 0;
        }

        /// <summary>
        /// This routine updates all properties that need to have their values replaced with entries
        /// from the handheld registry. This method should be called PRIOR to PostDeserialize
        /// </summary>
        /// <param name="iRegistry"></param>
        public virtual void ResolveRegistryItems(ITTRegistry iRegistry)
        {
            // Don't need to do anything if there is no list at all
            if (this.RegistryItems == null)
                return;

            // Don't need to do anything for current object if the registry substitutions list is empty
            if (this.RegistryItems.Count == 0)
            {
                this.RegistryItems = null;
                return;
            }

            // Get list of public properties
            PropertyInfo[] props = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);

            // Loop through list of all properties
            foreach (string RegItemSrcStr in this.RegistryItems)
            {
                // Format of RegItemSrcStr is "PROPNAME=REGISTRYSECTION REGISTRYITEM"
                
                // Extract property name. 
                string loPropName = RegItemSrcStr;
                int loIdx = loPropName.IndexOf("=");
                if (loIdx >= 0)
                    loPropName = loPropName.Substring(0, loIdx);
                
                // Extract registry section name
                string loRegSectionName = RegItemSrcStr.Remove(0, loIdx + 1);
                string loRegItemName = loRegSectionName;
                loIdx = loRegSectionName.IndexOf(" ");
                if (loIdx >= 0)
                    loRegSectionName = loRegSectionName.Substring(0, loIdx);

                // Extract registry key name
                loRegItemName = loRegItemName.Remove(0, loIdx + 1);

                // Now get the corresponding string value from the registry
                string loRegSubtituteStr = iRegistry.GetValue(loRegSectionName, loRegItemName);
                
                // Now find the corresponding property
                foreach (PropertyInfo info in props)
                {
                    try
                    {
                        if (info.Name == loPropName)
                        {
                            // We need to set the property value, but must translate as necessary
                            // depending on the datatype of the property.

                            if (info.PropertyType == typeof(string))
                            {
                                info.SetValue(this, loRegSubtituteStr, null);
                            }
                            else if (info.PropertyType == typeof(int))
                            {
                                if (!string.IsNullOrEmpty(loRegSubtituteStr))
                                    info.SetValue(this, Convert.ToInt32(loRegSubtituteStr), null);
                            }
                            else if (info.PropertyType == typeof(double))
                            {
                                if (!string.IsNullOrEmpty(loRegSubtituteStr))
                                    info.SetValue(this, Convert.ToDouble(loRegSubtituteStr), null);
                            }
                            else if (info.PropertyType == typeof(DateTime))
                            {
                                // Dates in REGISTRY.DAT should follow fixed date format reglardless of locale
                                DateTime loDateTime = DateTime.Now;
                                if (!string.IsNullOrEmpty(loRegSubtituteStr))
                                {
                                   /* ReinoControls.TextBoxBehavior.DateStringToOSDate(ReinoTablesConst.DATE_TYPE_DATAMASK,
                                    loRegSubtituteStr, ref loDateTime);
                                    info.SetValue(this, loDateTime, null);
                                    */
                                }
                            }
                            else
                            {
                                // Datatype of property not handled. Just write to debug info instead of raising error?
                                Log.Error("REGISTRY", "Unhandled datatype in ResolveRegistryItems(): " + info.PropertyType.Name);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("REGISTRY", "Expected registry value not found: " + loRegSectionName + ", " + loRegItemName);
                    }
                }
                    
            }

            // Now release reference to this list because we should never need it again
            this.RegistryItems = null;
        }

#if USE_DEFN_IMPLEMENTATION
#endif
	}

    /// <summary>
    /// Interface to provide access to Capacity property of a generic list
    /// </summary>
    public interface IListCapacity
    {
        int Capacity { get; set; }
    }

    /// <summary>
    /// Defines a generic (strongly typed) list that implements the IListCapacity. This allows
    /// setting the list's capacity without typecasting to a strong type.
    /// </summary>
    public class ListWithCapacity<T> : List<T>, IListCapacity
    {
    }

    /// <summary>
    /// Implements a "Generic" list of TObjBase instances.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListObjBase<T> : List<T>, IListCapacity
    {
        public int PostDeserializeListItems(TObjBase iParent)
        {
            /*
            IEnumerator loEnumerator = GetEnumerator();

            loEnumerator.Reset();
            while (loEnumerator.MoveNext())
            {
                ((TObjBase)loEnumerator.Current).PostDeserialize(iParent);
            }
            return 0;
             */

            // post deserialize can cause items to be added to lists, rendering an enumerator approach invalid
            for (int loObjectIndex = 0; loObjectIndex < this.Count; loObjectIndex++)
            {
                (this[loObjectIndex] as TObjBase).PostDeserialize(iParent);
            }

            return 0;
        }

        public void ResolveRegistryItemsForListItems(ITTRegistry iRegistry)
        {
            for (int loObjectIndex = 0; loObjectIndex < this.Count; loObjectIndex++)
            {
                (this[loObjectIndex] as TObjBase).ResolveRegistryItems(iRegistry);
            }
        }
    }

	// Class that contains functions that can be used as predicates for searches in collections based on TObjBase
	public class TObjBasePredicate
    {
		private string _CompareName;
		private Type _CompareClassType;

        // Constructor used when comparing strings (ie. Name or DisplayName)
		public TObjBasePredicate(string CompareName)
        {
			_CompareName = CompareName;
        }

		// Constructor used when comparing class types
		public TObjBasePredicate(Type CompareClassType)
		{
			_CompareClassType = CompareClassType;
		}

		// Compare by Name (Case-Sensitive)
		public bool CompareByName(Reino.ClientConfig.TObjBase pObject)
        {
		  return (pObject._Name == _CompareName);
        }


        // Compare by Name - Starts With (Case-Sensitive)
        public bool CompareByName_StartsWith(Reino.ClientConfig.TObjBase pObject)
        {
            return (pObject._Name.StartsWith(_CompareName));
        }


		// Compare by Name (Case-Insensitive)
		public bool CompareByName_CaseInsensitive(Reino.ClientConfig.TObjBase pObject)
		{
			return (System.String.Compare(pObject._Name, this._CompareName, true) == 0);
		}

		// Compare by DisplayName (Case-Insensitive)
		public bool CompareByDisplayName_CaseInsensitive(Reino.ClientConfig.TObjBase pObject)
		{
			// We'll do a case-insensitive comparison on object's DisplayName property. 
			// (There are only a few classes that this is applicable for)
			if (pObject is TCustomerCfg)
				return (System.String.Compare(((TCustomerCfg)(pObject)).DisplayName, this._CompareName, true) == 0);
			if (pObject is TIssStruct)
				return (System.String.Compare(((TIssStruct)(pObject)).ObjDisplayName, this._CompareName, true) == 0);
			if (pObject is TTableFldDef)
				return (System.String.Compare(((TTableFldDef)(pObject)).DisplayName, this._CompareName, true) == 0);

			// If we get this far, somebody's trying to search by DisplayName for an unsupported object
			throw new Exception("Searches by DisplayName are not supported for class type: " + pObject.GetType().Name);
        }
		
        // Compare by Class Type
		public bool CompareByClassType(Reino.ClientConfig.TObjBase pObject)
		{
			return _CompareClassType.IsAssignableFrom(pObject.GetType());
		}
    }
    
    public interface ITTRegistry
    {
        string GetValue(string iSection, string iItem);
        string ParseAndGetValue(string iSrcString);
    }

    /// <summary>
    /// Include this metatag in front of properties and fields that are host-side only
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
    public class HostSideOnlyAttribute : Attribute
    {
    }
}
