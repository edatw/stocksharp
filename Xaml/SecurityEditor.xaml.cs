namespace StockSharp.Xaml
{
	using System;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;

	using Ecng.Common;
	using Ecng.Configuration;
	using Ecng.Xaml;

	using StockSharp.BusinessEntities;

	/// <summary>
	/// The control activating <see cref="SecurityPickerWindow"/>.
	/// </summary>
	public partial class SecurityEditor
	{
		/// <summary>
		/// The command to delete the selected instrument.
		/// </summary>
		public readonly static RoutedCommand ClearCommand = new RoutedCommand();

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityEditor"/>.
		/// </summary>
		public SecurityEditor()
		{
			InitializeComponent();

			SecurityProvider = GetSecurityProvider();

			if (SecurityProvider == null)
			{
				ConfigManager.ServiceRegistered += (t, s) =>
				{
					if (typeof(ISecurityProvider) != t)
						return;

					GuiDispatcher.GlobalDispatcher.AddAction(() => SecurityProvider = (ISecurityProvider)s);
				};
			}
		}

		private ThreadSafeObservableCollection<Security> _itemsSource;
		private ISecurityProvider _securityProvider;

		/// <summary>
		/// The provider of information about instruments.
		/// </summary>
		public ISecurityProvider SecurityProvider
		{
			get { return _securityProvider; }
			set
			{
				if (_securityProvider == value)
					return;

				if (_securityProvider != null)
				{
					_securityProvider.Added -= AddSecurity;
					_securityProvider.Removed -= RemoveSecurity;
					_securityProvider.Cleared -= ClearSecurities;

					SecurityTextBox.ItemsSource = Enumerable.Empty<Security>();
					_itemsSource = null;
				}

				_securityProvider = value;

				if (_securityProvider == null)
					return;

				var itemsSource = new ObservableCollectionEx<Security>();

				_itemsSource = new ThreadSafeObservableCollection<Security>(itemsSource);

				_securityProvider.Added += AddSecurity;
				_securityProvider.Removed += RemoveSecurity;
				_securityProvider.Cleared += ClearSecurities;

				SecurityTextBox.ItemsSource = itemsSource;
			}
		}

		private void AddSecurity(Security security)
		{
			_itemsSource.Add(security);
		}

		private void RemoveSecurity(Security security)
		{
			_itemsSource.Remove(security);
		}

		private void ClearSecurities()
		{
			_itemsSource.Clear();
		}

		/// <summary>
		/// <see cref="DependencyProperty"/> for <see cref="SecurityEditor.SelectedSecurity"/>.
		/// </summary>
		public static readonly DependencyProperty SelectedSecurityProperty =
			 DependencyProperty.Register("SelectedSecurity", typeof(Security), typeof(SecurityEditor),
				new FrameworkPropertyMetadata(null, OnSelectedSecurityPropertyChanged));

		private Security _selectedSecurity;

		/// <summary>
		/// The selected instrument.
		/// </summary>
		public Security SelectedSecurity
		{
			get { return _selectedSecurity; }
			set { SetValue(SelectedSecurityProperty, value); }
		}

		/// <summary>
		/// Gets or sets the text of the currently selected item.
		/// </summary>
		public string Text
		{
			get { return SecurityTextBox.Text; }
			set { SecurityTextBox.Text = value; }
		}

		/// <summary>
		/// The selected instrument change event.
		/// </summary>
		public event Action SecuritySelected;

		private static void OnSelectedSecurityPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			var editor = (SecurityEditor)source;
			editor.UpdatedControls((Security)e.NewValue);
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new SecurityPickerWindow
            {
				SecurityProvider = GetSecurityProvider(),
				SelectionMode = DataGridSelectionMode.Single
            };

            if (wnd.ShowModal(this))
            {
                SelectedSecurity = wnd.SelectedSecurity;
            }
        }

		private void UpdatedControls(Security selectedSecurity)
		{
			_selectedSecurity = selectedSecurity;
			SecurityTextBox.Text = selectedSecurity == null ? string.Empty : selectedSecurity.Id;
			SecuritySelected.SafeInvoke();
		}

		private void ClearCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SelectedSecurity = null;
		}

		private ISecurityProvider GetSecurityProvider()
		{
			return SecurityProvider ?? ConfigManager.TryGetService<ISecurityProvider>();
		}
	}
}