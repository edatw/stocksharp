namespace StockSharp.Algo.Indicators
{
	using System.ComponentModel;
	using System.Linq;

	using StockSharp.Localization;

	/// <summary>
	/// Exponential Moving Average.
	/// </summary>
	[DisplayName("EMA")]
	[DescriptionLoc(LocalizedStrings.Str785Key)]
	public class ExponentialMovingAverage : LengthIndicator<decimal>
	{
		private decimal _prevFinalValue;
		private decimal _multiplier = 1;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExponentialMovingAverage"/>.
		/// </summary>
		public ExponentialMovingAverage()
		{
			Length = 32;
		}

		/// <summary>
		/// To reset the indicator status to initial. The method is called each time when initial settings are changed (for example, the length of period).
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			_multiplier = 2m / (Length + 1);
			_prevFinalValue = 0;
		}

		/// <summary>
		/// To handle the input value.
		/// </summary>
		/// <param name="input">The input value.</param>
		/// <returns>The resulting value.</returns>
		protected override IIndicatorValue OnProcess(IIndicatorValue input)
		{
			var newValue = input.GetValue<decimal>();

			// буфер нужен только для формирования начального значение - SMA
			if (!IsFormed)
			{
				// пока sma не сформирована, возвращаем или "недоделанную" sma из финальных значенией
				// или "недоделанную" sma c пропущенным первым значением из буфера + промежуточное значение
				if (input.IsFinal)
				{
					Buffer.Add(newValue);

					_prevFinalValue = Buffer.Sum() / Length;

					return new DecimalIndicatorValue(this, _prevFinalValue);
				}
				else
				{
					return new DecimalIndicatorValue(this, (Buffer.Skip(1).Sum() + newValue) / Length);
				}
			}
			else
			{
				// если sma сформирована 
				// если IsFinal = true рассчитываем ema и сохраняем для последующих расчетов с промежуточными значениями
				var curValue = (newValue - _prevFinalValue) * _multiplier + _prevFinalValue;

				if (input.IsFinal)
					_prevFinalValue = curValue;

				return new DecimalIndicatorValue(this, curValue);
			}
		}
	}
}