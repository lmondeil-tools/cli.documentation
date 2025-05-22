namespace cli.slndoc.Services.Mappings;

using cli.slndoc.Models.Exports;
using cli.slndoc.Models.Extracted;

using System.Collections.Generic;

public interface IMappingService
{
    ExportGraph Flatten(ExportCode exportCode, bool iterateOnChildren = true);
    ExportGraph Flatten(ExportWrapper exportWrapper);
    ExportAttribute MapAttribute(ExtractedAttribute attribute);
    ExportClass MapClass(ExtractedClass extractedClass, IEnumerable<ExtractedClass> allClasses, IEnumerable<ExtractedInterface> allInterfaces);
    IEnumerable<ExportClass> MapClasses(IEnumerable<ExtractedClass> extractedClasses, IEnumerable<ExtractedClass> allClasses, IEnumerable<ExtractedInterface> allInterfaces);
    ExportConstructorDependency MapConstructorDependency(string typeName, IEnumerable<ExtractedClass> allClasses, IEnumerable<ExtractedInterface> allInterfaces);
    ExportWrapper MapWrapper(ExtractedWrapper extractedWrapper);
}