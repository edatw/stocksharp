namespace StockSharp.Algo.Indicators
{
	using System;

	using Ecng.Common;
	using Ecng.Serialization;

	/// <summary>
	/// The interface describing indicator.
	/// </summary>
	public interface IIndicator : IPersistable, ICloneable<IIndicator>
	{
		/// <summary>
		/// Unique ID.
		/// </summary>
		Guid Id { get; }

		/// <summary>
		/// Indicator name.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Whether the indicator is set.
		/// </summary>
		bool IsFormed { get; }

		/// <summary>
		/// The container storing indicator data.
		/// </summary>
		IIndicatorContainer Container { get; }

		/// <summary>
		/// The indicator change event (for example, a new value is added).
		/// </summary>
		event Action<IIndicatorValue, IIndicatorValue> Changed;

		/// <summary>
		/// The event of resetting the indicator status to initial. The event is called each time when initial settings are changed (for example, the length of period).
		/// </summary>
		event Action Reseted;

		/// <summary>
		/// To handle the input value.
		/// </summary>
		/// <param name="input">The input value.</param>
		/// <returns>The new value of the indicator.</returns>
		IIndicatorValue Process(IIndicatorValue input);

		/// <summary>
		/// To reset the indicator status to initial. The method is called each time when initial settings are changed (for example, the length of period).
		/// </summary>
		void Reset();
	}
}
