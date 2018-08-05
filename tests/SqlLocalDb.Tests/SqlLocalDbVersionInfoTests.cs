// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using Shouldly;
using Xunit;

namespace MartinCostello.SqlLocalDb
{
    public static class SqlLocalDbVersionInfoTests
    {
        [Fact]
        public static void Update_Copies_State_From_Other_Instance()
        {
            // Arrange
            var other = new SqlLocalDbVersionInfo()
            {
                Exists = true,
                Name = "OtherName",
                Version = new Version(2, 1),
            };

            var actual = new SqlLocalDbVersionInfo()
            {
                Exists = false,
                Name = "Name",
                Version = new Version(2, 0),
            };

            // Act
            actual.Update(other);

            // Assert
            actual.Exists.ShouldBe(other.Exists);
            actual.Name.ShouldBe(other.Name);
            actual.Version.ShouldBe(other.Version);
        }

        [Fact]
        public static void Update_Does_Not_Copy_State_If_Other_Is_Null()
        {
            // Arrange
            ISqlLocalDbVersionInfo other = null;

            var actual = new SqlLocalDbVersionInfo()
            {
                Exists = false,
                Name = "Name",
                Version = new Version(2, 0),
            };

            // Act
            actual.Update(other);

            // Assert
            actual.Exists.ShouldBeFalse();
            actual.Name.ShouldBe("Name");
            actual.Version.ShouldBe(new Version(2, 0));
        }

        [Fact]
        public static void Update_Does_Not_Copy_State_If_Other_Is_Self()
        {
            // Arrange
            var actual = new SqlLocalDbVersionInfo()
            {
                Exists = false,
                Name = "Name",
                Version = new Version(2, 0),
            };

            // Act
            actual.Update(actual);

            // Assert
            actual.Exists.ShouldBeFalse();
            actual.Name.ShouldBe("Name");
            actual.Version.ShouldBe(new Version(2, 0));
        }
    }
}
