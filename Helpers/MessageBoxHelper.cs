using System.Windows;
using Wpf.Ui.Controls;

namespace PrettyScreenSHOT.Helpers
{
    /// <summary>
    /// Helper for showing Wpf.Ui MessageBoxes with a simplified API
    /// </summary>
    public static class MessageBoxHelper
    {
        public static MessageBoxResult Show(
            string message,
            string title,
            MessageBoxButton button = MessageBoxButton.OK)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                var messageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = title,
                    Content = message,
                    ButtonLeftName = GetButtonName(button, true),
                    ButtonRightName = GetButtonName(button, false)
                };

                var result = messageBox.ShowDialogAsync().GetAwaiter().GetResult();
                return MapToMessageBoxResult(result, button);
            });
        }

        private static string GetButtonName(MessageBoxButton button, bool isLeft)
        {
            return button switch
            {
                MessageBoxButton.OK => isLeft ? "OK" : string.Empty,
                MessageBoxButton.OKCancel => isLeft ? "OK" : "Cancel",
                MessageBoxButton.YesNo => isLeft ? "Yes" : "No",
                MessageBoxButton.YesNoCancel => isLeft ? "Yes" : (isLeft ? "No" : "Cancel"),
                _ => isLeft ? "OK" : string.Empty
            };
        }

        private static MessageBoxResult MapToMessageBoxResult(
            Wpf.Ui.Controls.MessageBoxResult wpfUiResult,
            MessageBoxButton buttonType)
        {
            return buttonType switch
            {
                MessageBoxButton.OK => MessageBoxResult.OK,
                MessageBoxButton.OKCancel => wpfUiResult == Wpf.Ui.Controls.MessageBoxResult.Primary
                    ? MessageBoxResult.OK
                    : MessageBoxResult.Cancel,
                MessageBoxButton.YesNo => wpfUiResult == Wpf.Ui.Controls.MessageBoxResult.Primary
                    ? MessageBoxResult.Yes
                    : MessageBoxResult.No,
                MessageBoxButton.YesNoCancel => wpfUiResult switch
                {
                    Wpf.Ui.Controls.MessageBoxResult.Primary => MessageBoxResult.Yes,
                    Wpf.Ui.Controls.MessageBoxResult.Secondary => MessageBoxResult.No,
                    _ => MessageBoxResult.Cancel
                },
                _ => MessageBoxResult.None
            };
        }
    }
}
