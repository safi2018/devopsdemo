using System;
using System.Drawing;
using System.Windows.Forms;

namespace I95Dev.Connector.UI.Base.Helpers.Controls
{
    internal class InputBox
    {
        /// <summary>
        /// Shows the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="promptText">The prompt text.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static DialogResult Show(string title, string promptText, ref string value)
        {
            var label = new Label
            {
                Text = promptText,
                Font = new Font("Calibri", 11),
                AutoSize = true
            };
            label.SetBounds(9, 20, 372, 13);

            var textBox = new TextBox
            {
                UseSystemPasswordChar = true,
                Text = value,
                Font = new Font("Calibri", 11)
            };
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;

            textBox.SetBounds(12, 45, 372, 20);

            var buttonOk = new Button
            {
                Font = new Font("Calibri", 11, FontStyle.Bold),
                Text = "Submit",
                DialogResult = DialogResult.OK
            };

            buttonOk.SetBounds(200, 75, 75, 23);

            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            var form = new Form
            {
                Text = title,
                ControlBox = false,
                ClientSize = new Size(396, 107)
            };
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk });

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
}