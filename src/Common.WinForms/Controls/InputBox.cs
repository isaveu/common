﻿/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// Shows a simple dialog asking the user to input some text.
    /// </summary>
    public sealed partial class InputBox : Form
    {
        private InputBox()
        {
            InitializeComponent();
            buttonOK.Text = Resources.OK;
            buttonCancel.Text = Resources.Cancel;
        }

        /// <summary>
        /// Displays an input box asking the the user to input some text.
        /// </summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="title">The window title to use.</param>
        /// <param name="prompt">The prompt to display.</param>
        /// <param name="defaultText">The default text to show pre-entered in the input field.</param>
        /// <param name="password">Shall the input characters be hidden as a password?</param>
        /// <returns>The text the user entered if she pressed OK; otherwise <c>null</c>.</returns>
        [CanBeNull]
        public static string Show([CanBeNull] IWin32Window owner, [NotNull, Localizable(true)] string title, [NotNull, Localizable(true)] string prompt, [CanBeNull, Localizable(true)] string defaultText = null, bool password = false)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(title)) throw new ArgumentNullException(nameof(title));
            if (string.IsNullOrEmpty(prompt)) throw new ArgumentNullException(nameof(prompt));
            #endregion

            using (var inputBox = new InputBox
            {
                Text = title,
                labelPrompt = {Text = prompt.Replace("\n", Environment.NewLine)},
                textInput = {Text = defaultText, UseSystemPasswordChar = password},
                ShowInTaskbar = (owner == null)
            })
                return (inputBox.ShowDialog(owner) == DialogResult.OK) ? inputBox.textInput.Text : null;
        }

        private void InputBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                textInput.Text = (files ?? new string[0]).FirstOrDefault();
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
                textInput.Text = (string)e.Data.GetData(DataFormats.Text);
        }

        private void InputBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.FileDrop))
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }
    }
}
