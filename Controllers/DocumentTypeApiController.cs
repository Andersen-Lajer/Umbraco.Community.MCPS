using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Extensions;

namespace Umbraco.Community.MCPS.Controllers;

[Route("mcps/api/documenttypes")]
public class DocumentTypeApiController(IContentTypeService contentTypeService) : UmbracoApiController
{
    [HttpGet("all")]
    public IActionResult GetAllDocumentTypes()
    {
        var allDocumentTypes = contentTypeService.GetAll();
        var result = new List<object>();

        foreach (var documentType in allDocumentTypes)
        {
            result.Add(new
            {
                documentType.Id,
                documentType.Alias,
                documentType.Name,
                documentType.Description,
                documentType.CreateDate,
                documentType.UpdateDate,
                documentType.AllowedAsRoot,
                documentType.IsElement,
                Variations = documentType.Variations.ToString(),
                Guid = documentType.GetUdi().ToString(),
                documentType.PropertyTypes
            });
        }
        return Ok(result);
    }
}