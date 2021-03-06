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

using System.Globalization;
using FluentAssertions;
using NanoByte.Common.Collections;
using Xunit;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// Contains test methods for <see cref="SetLocalizableString"/>.
    /// </summary>
    public class SetLocalizableStringTest
    {
        /// <summary>
        /// Makes sure <see cref="SetLocalizableString"/> correctly performs executions and undos.
        /// </summary>
        [Fact]
        public void TestExecute()
        {
            var collection = new LocalizableStringCollection
            {
                "neutralValue1",
                {"en-US", "americaValue1"}
            };

            var neutralCommand = new SetLocalizableString(collection, new LocalizableString {Value = "neutralValue2"});
            var americanCommand = new SetLocalizableString(collection, new LocalizableString {Language = new CultureInfo("en-US"), Value = "americaValue2"});

            neutralCommand.Execute();
            collection.GetExactLanguage(LocalizableString.DefaultLanguage)
                .Should().Be("neutralValue2", because: "Unspecified language should default to English generic");

            neutralCommand.Undo();
            collection.GetExactLanguage(LocalizableString.DefaultLanguage)
                .Should().Be("neutralValue1", because: "Unspecified language should default to English generic");

            americanCommand.Execute();
            collection.GetExactLanguage(new CultureInfo("en-US"))
                .Should().Be("americaValue2");

            americanCommand.Undo();
            collection.GetExactLanguage(new CultureInfo("en-US"))
                .Should().Be("americaValue1");
        }
    }
}
