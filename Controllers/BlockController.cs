using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Controllers;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Extensions;

[Route("mcps/api/documenttypes")]
public class DocumentTypeApiController : UmbracoApiController
{
    private readonly IContentTypeService _contentTypeService;

    public DocumentTypeApiController(IContentTypeService contentTypeService)
    {
        _contentTypeService = contentTypeService;
    }

    [HttpGet("all")]
    public IActionResult GetAllDocumentTypes()
    {
        // Retrieve all document types
        var allDocumentTypes = _contentTypeService.GetAll();
        var result = new List<object>();

        foreach (var documentType in allDocumentTypes)
        {
            result.Add(new
            {
                Id = documentType.Id,
                Alias = documentType.Alias,
                Name = documentType.Name,
                Description = documentType.Description,
                CreateDate = documentType.CreateDate,
                UpdateDate = documentType.UpdateDate,
                AllowedAsRoot = documentType.AllowedAsRoot,
                IsElement = documentType.IsElement,
                Variations = documentType.Variations.ToString(),
                Guid = documentType.GetUdi().ToString(),
                PropertyTypes = documentType.PropertyTypes
            });
        }

        return Ok(result);
    }
}
