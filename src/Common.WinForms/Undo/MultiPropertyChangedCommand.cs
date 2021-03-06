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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using JetBrains.Annotations;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// An undo command that handles multiple changed properties - usually used with a <see cref="PropertyGrid"/>.
    /// </summary>
    /// <seealso cref="MultiPropertyTracker"/>
    public class MultiPropertyChangedCommand : PreExecutedCommand
    {
        #region Variables
        private readonly object[] _targets;
        private readonly PropertyDescriptor _property;
        private readonly object[] _oldValues;
        private readonly object _newValue;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the command after the properties were first changed.
        /// </summary>
        /// <param name="targets">The objects the property belongs to.</param>
        /// <param name="property">The property that was changed.</param>
        /// <param name="oldValues">The property's old values.</param>
        /// <param name="newValue">The property's current value.</param>
        public MultiPropertyChangedCommand([NotNull] object[] targets, [NotNull] PropertyDescriptor property, [NotNull] object[] oldValues, object newValue)
        {
            _targets = targets ?? throw new ArgumentNullException(nameof(targets));
            _property = property ?? throw new ArgumentNullException(nameof(property));
            _oldValues = oldValues ?? throw new ArgumentNullException(nameof(oldValues));
            if (targets.Length != oldValues.Length) throw new ArgumentException(Resources.TargetsOldValuesLength, nameof(targets));
            _newValue = newValue;
        }

        /// <summary>
        /// Initializes the command after the property was first changed.
        /// </summary>
        /// <param name="targets">The objects the <see cref="PropertyGrid.SelectedObject"/> is target at.</param>
        /// <param name="gridItem">The grid item representing the property being changed.</param>
        /// <param name="oldValues">The property's old values.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "The arguments are passed on to a different overload of the constructor")]
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "This is simply a comfort wrapper for extracting values from the event arguments")]
        public MultiPropertyChangedCommand([NotNull] object[] targets, [NotNull] GridItem gridItem, [NotNull] object[] oldValues)
            // ReSharper disable once AssignNullToNotNullAttribute
            : this(targets, gridItem.PropertyDescriptor, oldValues, gridItem.Value)
        {}
        #endregion

        //--------------------//

        #region Undo / Redo
        /// <summary>
        /// Set the changed property value again.
        /// </summary>
        protected override void OnRedo()
        {
            foreach (object target in _targets)
            {
                // Use refelction to get the specific property for each object and set the new value everywhere
                target.GetType().GetProperty(_property.Name).SetValue(target, _newValue, null);
            }
        }

        /// <summary>
        /// Restore the original property values.
        /// </summary>
        protected override void OnUndo()
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                // Use reflection to get the specific property for each object and set the corresponding old value for each
                _targets[i].GetType().GetProperty(_property.Name).SetValue(_targets[i], _oldValues[i], null);
            }
        }
        #endregion
    }
}
