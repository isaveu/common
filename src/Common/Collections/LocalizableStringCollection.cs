/*
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// A collection of <see cref="LocalizableString"/>s with language-search methods.
    /// </summary>
    [Serializable]
    public class LocalizableStringCollection : List<LocalizableString>, ICloneable<LocalizableStringCollection>
    {
        /// <summary>
        /// Adds a new string with an associated language to the collection.
        /// </summary>
        /// <param name="language">The language of the <paramref name="value"/>.</param>
        /// <param name="value">The actual string value to store.</param>
        public void Add([NotNull, Localizable(false)] string language, [CanBeNull] string value)
        {
            #region Sanity checks
            if (language == null) throw new ArgumentNullException(nameof(language));
            #endregion

            Add(new LocalizableString {LanguageString = language, Value = value});
        }

        /// <summary>
        /// Adds a new <c>en</c> string to the collection.
        /// </summary>
        /// <param name="value">The actual string value to store.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo")]
        public void Add(string value)
        {
            Add(new LocalizableString {Value = value});
        }

        /// <summary>
        /// Checks if the collection contains an entry exactly matching the specified language.
        /// </summary>
        /// <param name="language">The exact language to look for.</param>
        /// <returns><c>true</c> if an element with the specified language exists in the collection; <c>false</c> otherwise.</returns>
        /// <seealso cref="GetExactLanguage"/>
        public bool ContainsExactLanguage([NotNull] CultureInfo language)
        {
            #region Sanity checks
            if (language == null) throw new ArgumentNullException(nameof(language));
            #endregion

            return this.Any(entry => Equals(language, entry.Language));
        }

        /// <summary>
        /// Returns the first string in the collection exactly matching the specified language.
        /// </summary>
        /// <param name="language">The exact language to look for.</param>
        /// <returns>The string value found in the collection; <c>null</c> if none was found.</returns>
        /// <seealso cref="ContainsExactLanguage"/>
        [CanBeNull]
        public string GetExactLanguage([NotNull] CultureInfo language)
        {
            #region Sanity checks
            if (language == null) throw new ArgumentNullException(nameof(language));
            #endregion

            var match = this.FirstOrDefault(entry => Equals(language, entry.Language));
            return match?.Value;
        }

        /// <summary>
        /// Returns the best-fitting string in the collection for the specified language.
        /// </summary>
        /// <param name="language">The language to look for.</param>
        /// <returns>The best-fitting string value found in the collection; <c>null</c> if the collection is empty.</returns>
        /// <remarks>
        /// Language preferences in decreasing order:<br/>
        /// 1. exact match<br/>
        /// 2. same language with neutral culture<br/>
        /// 3. en<br/>
        /// 4. en-US<br/>
        /// 5. first entry in collection
        /// </remarks>
        [CanBeNull]
        public string GetBestLanguage([NotNull] CultureInfo language)
        {
            #region Sanity checks
            if (language == null) throw new ArgumentNullException(nameof(language));
            #endregion

            // Try to find exact match
            foreach (LocalizableString entry in this.Where(entry => Equals(language, entry.Language)))
                return entry.Value;

            // Try to find same language with neutral culture
            foreach (LocalizableString entry in this.Where(entry => language.TwoLetterISOLanguageName == entry.Language.TwoLetterISOLanguageName && entry.Language.IsNeutralCulture))
                return entry.Value;

            // Try to find "en"
            var en = LocalizableString.DefaultLanguage;
            foreach (LocalizableString entry in this.Where(entry => en.Equals(entry.Language)))
                return entry.Value;

            // Try to find "en-US"
            var enUs = new CultureInfo("en-US");
            foreach (LocalizableString entry in this.Where(entry => enUs.Equals(entry.Language)))
                return entry.Value;

            // Try to find first entry in collection
            return Count == 0 ? null : this[0].Value;
        }

        /// <summary>
        /// Adds a new string with an associated language to the collection. Preexisting entries with the same language are removed.
        /// </summary>
        /// <param name="language">The language of the <paramref name="value"/>.</param>
        /// <param name="value">The actual string value to store; <c>null</c> to remove existing entries.</param>
        public void Set([NotNull] CultureInfo language, [CanBeNull] string value)
        {
            #region Sanity checks
            if (language == null) throw new ArgumentNullException(nameof(language));
            #endregion

            RemoveAll(entry => language.Equals(entry.Language));
            if (value != null) Add(new LocalizableString {Language = language, Value = value});
        }

        #region Clone
        /// <summary>
        /// Creates a deep copy of this <see cref="LocalizableStringCollection"/> (elements are cloned).
        /// </summary>
        /// <returns>The cloned <see cref="LocalizableStringCollection"/>.</returns>
        public LocalizableStringCollection Clone()
        {
            var newDict = new LocalizableStringCollection();
            newDict.AddRange(this.Select(entry => entry.Clone()));
            return newDict;
        }
        #endregion
    }
}
