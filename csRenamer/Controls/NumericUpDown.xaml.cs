using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace csRenamer.Controls
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        private bool _valueInitialized = false;
        public NumericUpDown()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (!_valueInitialized)
                {
                    SetValue(ValueProperty, Min);
                }
            };
            DataContext = this;
            
        }

        public static readonly DependencyProperty ValueProperty =
                                DependencyProperty.Register(
                                    nameof(Value),
                                    typeof(int),
                                    typeof(NumericUpDown),
                                    new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDown)d;
            int newValue = (int)e.NewValue;

            // Asegurar que el nuevo valor esté dentro del rango
            if (newValue < control.Min)
                control.Value = control.Min;
            else if (newValue > control.Max)
                control.Value = control.Max;
            else
                control._valueInitialized = true;
        }

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register(nameof(Min), typeof(int), typeof(NumericUpDown), new PropertyMetadata(0));

        public int Min
        {
            get => (int)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(nameof(Max), typeof(int), typeof(NumericUpDown), new PropertyMetadata(100));

        public int Max
        {
            get => (int)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(int), typeof(NumericUpDown), new PropertyMetadata(1));

        public int Step
        {
            get => (int)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        private DispatcherTimer _timer;
        private Action? _activeAction;

        private void StartTimer(Action action)
        {
            _activeAction = action;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
            action(); // Ejecutar al menos una vez al presionar
        }

        private void StopTimer(object sender, RoutedEventArgs e)
        {
            _timer?.Stop();
            _timer = null;
            _activeAction = null;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _activeAction?.Invoke();
        }

        private void IncreaseButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartTimer(() =>
            {
                if (Value + Step <= Max)
                    Value += Step;
            });
        }

        private void DecreaseButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartTimer(() =>
            {
                if (Value - Step >= Min)
                    Value -= Step;
            });
        }
    }
}
