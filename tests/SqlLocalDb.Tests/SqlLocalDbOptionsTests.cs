// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.SqlLocalDb
{
    public static class SqlLocalDbOptionsTests
    {
        [Fact]
        public static void SqlLocalDbOptions_Defaults_Are_Correct()
        {
            // Act
            var actual = new SqlLocalDbOptions();

            // Assert
            actual.AutomaticallyDeleteInstanceFiles.ShouldBeFalse();
            actual.Language.ShouldBeNull();
            actual.NativeApiOverrideVersion.ShouldBe(string.Empty);
            actual.StopOptions.ShouldBe(StopInstanceOptions.None);
            actual.StopTimeout.ShouldBe(TimeSpan.FromMinutes(1));
        }

        [Fact]
        public static void SqlLocalDbOptions_LanguageId_Returns_The_LCID()
        {
            // Act
            var actual = new SqlLocalDbOptions()
            {
                Language = CultureInfo.GetCultureInfo("de-DE"),
            };

            // Assert
            actual.LanguageId.ShouldBeGreaterThan(0);
        }
    }
}
