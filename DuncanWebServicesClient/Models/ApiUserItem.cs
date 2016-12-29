using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DuncanWebServicesClient
{
	public class ApiUserItem
	{
		protected List<ApiUserItem.AdditionalUserField> _AdditionalFields;
		protected string _OfficerID;
		protected string _OfficerName;
		protected string _OfficerReinoTechKey;
		protected int _UniqueKey;
		protected string _UserName;
		protected string _UserPassword;

		public ApiUserItem ()
		{
		}

		public List<ApiUserItem.AdditionalUserField> AdditionalFields { get; set; }
		[DefaultValue("")]
		[XmlAttribute]
		public string OfficerID { get; set; }
		[DefaultValue("")]
		[XmlAttribute]
		public string OfficerName { get; set; }
		[DefaultValue("")]
		[XmlAttribute]
		public string OfficerReinoTechKey { get; set; }
		[XmlAttribute]
		public int UniqueKey { get; set; }
		[DefaultValue("")]
		[XmlAttribute]
		public string UserName { get; set; }
		[DefaultValue("")]
		[XmlAttribute]
		public string UserPassword { get; set; }

		public class AdditionalUserField
		{
			protected ApiUserItem.AdditionalUserField.FieldDataType _DataType;
			protected DateTime _DateTimeValue;
			protected string _FieldName;
			protected int _IntValue;
			protected float _RealValue;
			protected string _StringValue;

			public AdditionalUserField()
			{
			}

			[XmlAttribute]
			public ApiUserItem.AdditionalUserField.FieldDataType DataType { get; set; }
			[XmlAttribute]
			public DateTime DateTimeValue { get; set; }
			[DefaultValue("")]
			[XmlAttribute]
			public string FieldName { get; set; }
			[DefaultValue(0)]
			[XmlAttribute]
			public int IntValue { get; set; }
			[DefaultValue(0f)]
			[XmlAttribute]
			public float RealValue { get; set; }
			[DefaultValue("")]
			[XmlAttribute]
			public string StringValue { get; set; }

			public enum FieldDataType
			{
				String = 0,
				Date = 1,
				Time = 2,
				Real = 3,
				Integer = 4,
			}
		}
	}
}



