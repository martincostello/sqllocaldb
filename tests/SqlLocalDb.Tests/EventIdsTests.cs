// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Reflection;
using Microsoft.Extensions.Logging;

namespace MartinCostello.SqlLocalDb
{
    public static class EventIdsTests
    {
        [Theory]
        [InlineData("NativeApiLoaded", 1)]
        [InlineData("NativeApiLoadFailed", 2)]
        [InlineData("NativeApiNotLoaded", 3)]
        [InlineData("NativeApiVersionOverriddenByUser", 4)]
        [InlineData("NativeApiVersionOverrideNotFound", 5)]
        [InlineData("NoNativeApiFound", 6)]
        [InlineData("NativeApiPathNotFound", 7)]
        [InlineData("NativeFunctionNotFound", 8)]
        [InlineData("NotInstalled", 9)]
        [InlineData("CreatingInstance", 10)]
        [InlineData("CreatingInstanceFailed", 11)]
        [InlineData("CreatedInstance", 12)]
        [InlineData("DeletingInstance", 13)]
        [InlineData("DeletingInstanceFailed", 14)]
        [InlineData("DeletingInstanceFailedAsCannotBeNotFound", 15)]
        [InlineData("DeletingInstanceFailedAsInUse", 16)]
        [InlineData("DeletedInstance", 17)]
        [InlineData("DeletingInstanceFiles", 18)]
        [InlineData("DeletingInstanceFilesFailed", 19)]
        [InlineData("DeletedInstanceFiles", 20)]
        [InlineData("GettingInstanceInfo", 21)]
        [InlineData("GettingInstanceInfoFailed", 22)]
        [InlineData("GotInstanceInfo", 23)]
        [InlineData("GettingInstanceNames", 24)]
        [InlineData("GettingInstanceNamesFailed", 25)]
        [InlineData("GotInstanceNames", 26)]
        [InlineData("GettingVersionInfo", 27)]
        [InlineData("GettingVersionInfoFailed", 28)]
        [InlineData("GotVersionInfo", 29)]
        [InlineData("GettingVersions", 30)]
        [InlineData("GettingVersionsFailed", 31)]
        [InlineData("GotVersions", 32)]
        [InlineData("InvalidLanguageId", 33)]
        [InlineData("InvalidRegistryKey", 34)]
        [InlineData("RegistryKeyNotFound", 35)]
        [InlineData("StartingInstance", 36)]
        [InlineData("StartingInstanceFailed", 37)]
        [InlineData("StartedInstance", 38)]
        [InlineData("StoppingInstance", 39)]
        [InlineData("StoppingInstanceFailed", 40)]
        [InlineData("StoppedInstance", 41)]
        [InlineData("StartingTracing", 42)]
        [InlineData("StartingTracingFailed", 43)]
        [InlineData("StartedTracing", 44)]
        [InlineData("StoppedTracing", 45)]
        [InlineData("StoppingTracingFailed", 46)]
        [InlineData("StoppingTracing", 47)]
        [InlineData("StopTemporaryInstanceFailed", 48)]
        [InlineData("SharingInstance", 49)]
        [InlineData("SharingInstanceFailed", 50)]
        [InlineData("SharedInstance", 51)]
        [InlineData("UnsharingInstance", 52)]
        [InlineData("UnsharingInstanceFailed", 53)]
        [InlineData("UnsharedInstance", 54)]
        [InlineData("NativeApiUnloaded", 55)]
        [InlineData("GenericError", 56)]
        public static void EventId_Name_And_Value_Is_Correct(string name, int expected)
        {
            // Arrange
            FieldInfo? field = typeof(EventIds).GetField(name, BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            object? value = field!.GetValue(null);
            EventId eventId = (EventId)value!;

            // Asset
            eventId.ShouldNotBe(default);
            eventId.Name.ShouldBe(name);
            eventId.Id.ShouldBe(expected);
        }
    }
}
