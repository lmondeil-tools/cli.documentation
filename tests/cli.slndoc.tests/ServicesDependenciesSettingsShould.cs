namespace cli.slndoc.tests;

using AwesomeAssertions;

using cli.slndoc.Models.Extracted;
using cli.slndoc.Models.Settings;

using ConsoleApp.Dynamic;

using Serialize.Linq.Serializers;

using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

public class ServicesDependenciesSettingsShould
{
    [Fact]
    public void GIVEN_ServicesDependenciesSettings_WHEN_InvokingRootClassesFilter_ShouldNotThrow()
    {
        // Arrange
        //------------------------------------------------------------------------
        var servicesDependenciesSettings = new ServicesDependenciesSettings
        {
            RootClassesFilterAsString = "x => x.BaseTypes.Any(t => Regex.IsMatch(t, @\"IMyModel\\<(\\w+)\\>\"))"
        };

        // Act & Assert
        //------------------------------------------------------------------------
        Action a = () => { var filter = servicesDependenciesSettings.RootClassesFilter; };
        a.Should().NotThrow();
    }

    [Fact]
    public void GIVEN_ServicesDependenciesSettings_With_Filter_WHEN_FilteringOnManyClasses_THEN_ReturnsOnlyFilteredModels()
    {
        // Arrange
        //------------------------------------------------------------------------
        ExtractedClass[] classesToTest = [
            new (){ Name = "01", BaseTypes = ["IMyModel<string>"] },
            new (){ Name = "02", BaseTypes = ["namespace.IMyModel<JsonDocument>"] },
            new (){ Name = "03", BaseTypes = ["JsonDocument" ] }
        ];

        var servicesDependenciesSettings = new ServicesDependenciesSettings
        {
            RootClassesFilterAsString = "x => x.BaseTypes.Any(t => Regex.IsMatch(t, @\"IMyModel\\<(\\w+)\\>\"))"
        };

        // Act & Assert
        //------------------------------------------------------------------------
        var result = classesToTest.Where(servicesDependenciesSettings.RootClassesFilter).ToArray();

        result.Select(x => x.Name).Should().BeEquivalentTo(["01", "02"]);
    }
}