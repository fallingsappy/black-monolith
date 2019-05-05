using System.Windows;
using System.Windows.Controls;

namespace TabSwitcher
{
	public partial class TabSwitcherControl : UserControl
	{
		public event RoutedEventHandler Back;
		public event RoutedEventHandler Forward;

		public TabSwitcherControl() => InitializeComponent();

		private void BackvardButton_Click(object sender, RoutedEventArgs e) => Back?.Invoke(this, new RoutedEventArgs());
		private void ForvardButton_Click(object sender, RoutedEventArgs e) => Forward?.Invoke(this, new RoutedEventArgs());
	}
}