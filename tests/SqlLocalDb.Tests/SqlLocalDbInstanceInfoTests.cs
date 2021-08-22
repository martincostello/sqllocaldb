// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Moq;

namespace MartinCostello.SqlLocalDb
{
    public static class SqlLocalDbInstanceInfoTests
    {
        [Fact]
        public static void Update_Copies_State_From_Other_Instance()
        {
            // Arrange
            var api = Mock.Of<ISqlLocalDbApi>();

            var other = new SqlLocalDbInstanceInfo(api)
            {
                ConfigurationCorrupt = true,
                Exists = true,
                IsAutomatic = true,
                IsRunning = true,
                IsShared = true,
                LastStartTimeUtc = DateTime.UtcNow,
                LocalDbVersion = new Version(2, 1),
                Name = "OtherName",
                NamedPipe = "OtherPipe",
                OwnerSid = "Sidney Poitier",
                SharedName = "OtherSharedName",
            };

            var actual = new SqlLocalDbInstanceInfo(api)
            {
                ConfigurationCorrupt = false,
                Exists = false,
                IsAutomatic = false,
                IsRunning = false,
                IsShared = false,
                LastStartTimeUtc = DateTime.MinValue,
                LocalDbVersion = new Version(2, 0),
                Name = "Name",
                NamedPipe = "OtherPipe",
                OwnerSid = "Sid James",
                SharedName = "SharedName",
            };

            // Act
            actual.Update(other);

            // Assert
            actual.ConfigurationCorrupt.ShouldBe(other.ConfigurationCorrupt);
            actual.Exists.ShouldBe(other.Exists);
            actual.IsAutomatic.ShouldBe(other.IsAutomatic);
            actual.IsRunning.ShouldBe(other.IsRunning);
            actual.IsShared.ShouldBe(other.IsShared);
            actual.LastStartTimeUtc.ShouldBe(other.LastStartTimeUtc);
            actual.LocalDbVersion.ShouldBe(other.LocalDbVersion);
            actual.Name.ShouldBe(other.Name);
            actual.NamedPipe.ShouldBe(other.NamedPipe);
            actual.OwnerSid.ShouldBe(other.OwnerSid);
            actual.SharedName.ShouldBe(other.SharedName);
        }

        [Fact]
        public static void Update_Does_Not_Copy_State_If_Other_Is_Null()
        {
            // Arrange
            var api = Mock.Of<ISqlLocalDbApi>();

            var actual = new SqlLocalDbInstanceInfo(api)
            {
                ConfigurationCorrupt = true,
                Exists = true,
                IsAutomatic = true,
                IsRunning = true,
                IsShared = true,
                LastStartTimeUtc = DateTime.UtcNow,
                LocalDbVersion = new Version(2, 1),
                Name = "Name",
                NamedPipe = "NamedPipe",
                OwnerSid = "OwnerSid",
                SharedName = "SharedName",
            };

            // Act (no Assert)
            actual.Update(null!);
        }

        [Fact]
        public static void Update_Does_Not_Copy_State_If_Other_Is_Self()
        {
            // Arrange
            var api = Mock.Of<ISqlLocalDbApi>();

            var actual = new SqlLocalDbInstanceInfo(api)
            {
                ConfigurationCorrupt = true,
                Exists = true,
                IsAutomatic = true,
                IsRunning = true,
                IsShared = true,
                LastStartTimeUtc = DateTime.UtcNow,
                LocalDbVersion = new Version(2, 1),
                Name = "Name",
                NamedPipe = "NamedPipe",
                OwnerSid = "OwnerSid",
                SharedName = "SharedName",
            };

            // Act (no Assert)
            actual.Update(actual);
        }

        [Fact]
        public static void ToString_Returns_The_Name()
        {
            // Arrange
            var api = Mock.Of<ISqlLocalDbApi>();

            var info = new SqlLocalDbInstanceInfo(api)
            {
                ConfigurationCorrupt = true,
                Exists = true,
                IsAutomatic = true,
                IsRunning = true,
                IsShared = true,
                LastStartTimeUtc = DateTime.UtcNow,
                LocalDbVersion = new Version(2, 1),
                Name = "Name",
                NamedPipe = "NamedPipe",
                OwnerSid = "OwnerSid",
                SharedName = "SharedName",
            };

            // Act and Assert
            string actual = info.ToString();

            // Assert
            actual.ShouldBe("Name");
        }
    }
}
