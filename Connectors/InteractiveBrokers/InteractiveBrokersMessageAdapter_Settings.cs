namespace StockSharp.InteractiveBrokers
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Net;

	using Ecng.Common;
	using Ecng.ComponentModel;
	using Ecng.Serialization;

	using StockSharp.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	/// <summary>
	/// The messages adapter for InteractiveBrokers.
	/// </summary>
	[Icon("InteractiveBrokers_logo.png")]
	[Doc("http://stocksharp.com/doc/html/bae7b613-dcf6-4abb-b595-6c61fc4e5c46.htm")]
	[DisplayName("Interactive Brokers")]
	[CategoryLoc(LocalizedStrings.AmericaKey)]
	[DescriptionLoc(LocalizedStrings.Str1770Key, "Interactive Brokers")]
	[CategoryOrderLoc(LocalizedStrings.Str174Key, 0)]
	[CategoryOrderLoc(LocalizedStrings.Str186Key, 1)]
	[CategoryOrderLoc(LocalizedStrings.LoggingKey, 2)]
	partial class InteractiveBrokersMessageAdapter
	{
		/// <summary>
		/// Address by default.
		/// </summary>
		public static readonly EndPoint DefaultAddress = new IPEndPoint(IPAddress.Loopback, 7496);

		/// <summary>
		/// Address by default.
		/// </summary>
		public static readonly EndPoint DefaultGatewayAddress = new IPEndPoint(IPAddress.Loopback, 4001);

		/// <summary>
		/// Address.
		/// </summary>
		[CategoryLoc(LocalizedStrings.Str174Key)]
		[DisplayNameLoc(LocalizedStrings.AddressKey)]
		[DescriptionLoc(LocalizedStrings.AddressKey)]
		[PropertyOrder(1)]
		public EndPoint Address { get; set; }

		/// <summary>
		/// Unique ID. Used when several clients are connected to one terminal or gateway.
		/// </summary>
		[CategoryLoc(LocalizedStrings.Str174Key)]
		[DisplayNameLoc(LocalizedStrings.Str361Key)]
		[DescriptionLoc(LocalizedStrings.Str2518Key)]
		[PropertyOrder(2)]
		public int ClientId { get; set; }

		/// <summary>
		/// Whether to use real-time data or 'frozen' on the broker server. By default, the 'frozen' data is used.
		/// </summary>
		[CategoryLoc(LocalizedStrings.Str174Key)]
		[DisplayNameLoc(LocalizedStrings.RealTimeKey)]
		[DescriptionLoc(LocalizedStrings.Str2520Key)]
		[PropertyOrder(3)]
		public bool IsRealTimeMarketData { get; set; }

		/// <summary>
		/// The server messages logging level. The default is <see cref="ServerLogLevels.Detail"/>.
		/// </summary>
		[CategoryLoc(LocalizedStrings.Str174Key)]
		[DisplayNameLoc(LocalizedStrings.Str9Key)]
		[DescriptionLoc(LocalizedStrings.Str2521Key)]
		[PropertyOrder(4)]
		public ServerLogLevels ServerLogLevel { get; set; }

		private IEnumerable<GenericFieldTypes> _fields = Enumerable.Empty<GenericFieldTypes>();

		/// <summary>
		/// Market data fields, which will be received with subscribed to Level1 messages.
		/// </summary>
		[CategoryLoc(LocalizedStrings.Str174Key)]
		[DisplayNameLoc(LocalizedStrings.Str2522Key)]
		[DescriptionLoc(LocalizedStrings.Str2523Key)]
		[PropertyOrder(4)]
		public IEnumerable<GenericFieldTypes> Fields
		{
			get { return _fields; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_fields = value;
			}
		}

		/// <summary>
		/// The connection time.
		/// </summary>
		[Browsable(false)]
		public DateTime ConnectedTime { get; internal set; }

		/// <summary>
		/// Extra authentication.
		/// </summary>
		[Browsable(false)]
		public bool ExtraAuth { get; set; }

		/// <summary>
		/// Optional capabilities.
		/// </summary>
		[Browsable(false)]
		public string OptionalCapabilities { get; set; }

		/// <summary>
		/// Load settings.
		/// </summary>
		/// <param name="storage">Settings storage.</param>
		public override void Load(SettingsStorage storage)
		{
			base.Load(storage);

			Address = storage.GetValue<EndPoint>("Address");
			ClientId = storage.GetValue<int>("ClientId");
			IsRealTimeMarketData = storage.GetValue<bool>("IsRealTimeMarketData");
			ServerLogLevel = storage.GetValue<ServerLogLevels>("ServerLogLevel");
			Fields = storage.GetValue<string>("Fields").Split(",").Select(n => n.To<GenericFieldTypes>()).ToArray();
			ExtraAuth = storage.GetValue<bool>("ExtraAuth");
			OptionalCapabilities = storage.GetValue<string>("OptionalCapabilities");
		}

		/// <summary>
		/// Save settings.
		/// </summary>
		/// <param name="storage">Settings storage.</param>
		public override void Save(SettingsStorage storage)
		{
			base.Save(storage);

			storage.SetValue("Address", Address.To<string>());
			storage.SetValue("ClientId", ClientId);
			storage.SetValue("IsRealTimeMarketData", IsRealTimeMarketData);
			storage.SetValue("ServerLogLevel", ServerLogLevel.To<string>());
			storage.SetValue("Fields", Fields.Select(t => t.To<string>()).Join(","));
			storage.SetValue("ExtraAuth", ExtraAuth);
			storage.SetValue("OptionalCapabilities", OptionalCapabilities);
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return LocalizedStrings.Str2526Params.Put(Address);
		}
	}
}