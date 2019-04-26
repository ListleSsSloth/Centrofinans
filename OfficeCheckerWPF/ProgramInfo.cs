using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace OfficeCheckerWPF
{
    public class ProgramInfo
    {
        public readonly string SearchName;
        public readonly CheckBox CheckBox;
        public readonly string TitleName;
        public bool Installed = false;

        public ProgramInfo(string searchName, string titleName, bool createCheckBox = true)
        {
            SearchName = searchName;
            TitleName = titleName;

            if (!createCheckBox) 
                return;

            CheckBox = new CheckBox
            {
                Content = TitleName,
                IsHitTestVisible = false,
                Margin = new Thickness(3)
            };
            SetCheckBoxContentColorTrigger(CheckBox);
        }

        private static void SetCheckBoxContentColorTrigger(FrameworkElement source)
        {
            var style = new Style { TargetType = typeof(CheckBox) };
            style.Triggers.Clear();
            var trigger = new Trigger
            {
                Property = ToggleButton.IsCheckedProperty,
                Value =  true
            };

            var setter = new Setter
            {
                Property = Control.ForegroundProperty,
                Value = System.Windows.Media.Brushes.Blue
            };
            trigger.Setters.Add(setter);
            style.Triggers.Add(trigger);
            source.Style = style;
        }
    }
}