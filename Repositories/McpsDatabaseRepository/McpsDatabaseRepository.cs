using Microsoft.Extensions.Logging;
using System.Text.Json;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Models.Schemas;
using Umbraco.Community.MCPS.Mappers;

namespace Umbraco.Community.MCPS.Repositories;

public class McpsDatabaseRepository(IScopeProvider scopeProvider, ILogger<McpsDatabaseRepository> _logger) : IMcpsDatabaseRepository
{
    // Public PropagationRelation
    public int? CreatePropagationRelation(PropagationRelationsSchema relation)
    {
        using var scope = scopeProvider.CreateScope();
        var relationId = -1;
        var db = scope.Database;
        try
        {
            var query = "SELECT * FROM McpsPropagationRelations WHERE PageId = @0 AND PositionId = @1";
            var existingRelation = db.Fetch<PropagationRelationsSchema>(query, relation.PageId, relation.PositionId).FirstOrDefault();

            if (existingRelation is null)
            {
                var entry = db.Insert("McpsPropagationRelations", "Id", relation);
                if (entry.ToString() is string entryId)
                {
                    relationId = Int32.Parse(entryId);
                }
            }
            else
            {
                relation.Id = existingRelation.Id;
                db.Update("McpsPropagationRelations", "Id", relation);
            }

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in CreatePropagationRelation");
            return -1;
        }
        scope.Complete();
        return relation.Id;
    }

    public bool UpdatePropagationRelations(List<PropagationRelationsSchema> propagationRelations)
    {
        if (propagationRelations.Count == 0) { return false; }


        using var scope = scopeProvider.CreateScope();
        var db = scope.Database;
        try
        {
            foreach (var relation in propagationRelations)
            {
                try
                {
                    db.Update("McpsPropagationRelations", "Id", relation);
                }
                catch
                {
                    scope.Complete();
                    return false;
                }
            }
            scope.Complete();
            return true;

        }
        catch (Exception e)
        {

            _logger.LogError(e, "Error in UpdatePropagationRelations");
            scope.Complete();
            return false;
        }
    }

    public int RemoveUnusedRelations(Guid pageId, int relationCount)
    {
        using var scope = scopeProvider.CreateScope();
        var db = scope.Database;

        try
        {
            var query = "WHERE PageId = @0 AND PositionId >= @1";
            var deletedCount = db.Delete<PropagationRelationsSchema>(query, [pageId.ToString().ToUpper(), relationCount]);
            scope.Complete();
            return deletedCount;
        }
        catch
        {
            scope.Complete();
            return -1;
        }
    }

    public Guid GetTargetGuid(string id)
    {
        using var scope = scopeProvider.CreateScope();
        var db = scope.Database;
        try
        {
            var queryResult = db.Fetch<PropagationRelationsSchema>("SELECT * FROM McpsPropagationRelations WHERE Id = @0", id).FirstOrDefault();
            scope.Complete();

            if (queryResult is not null && queryResult.ReferenceId is Guid referenceId)
            {
                _logger.LogInformation("Source: @0 - Target: @1", [queryResult.PageId, referenceId]);
                return referenceId;
            }
            else
            {
                throw new Exception("Error in McpsDatabaseRepository.GetTargetGuid - db fetch");
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public List<PropagationRelationsSchema> GetPropagationRelations(int settingId)
    {
        using var scope = scopeProvider.CreateScope();
        var db = scope.Database;

        try
        {
            var relations = db.Fetch<PropagationRelationsSchema>("SElECT * FROM McpsPropagationRelations WHERE PropagationSettingId = @0", settingId);
            scope.Complete();
            return relations;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            scope.Complete();
            throw;
        }
    }


    // Public PropagationSetting
    public PropagationSettingSchema CreatePropagationSetting(PropagationSetting propagationSetting)
    {
        using var scope = scopeProvider.CreateScope();
        var db = scope.Database;
        try
        {
            if (propagationSetting.Name is not null)
            {
                var query = "SELECT * FROM McpsPropagationSettings WHERE Name = @0";


                var existingSetting = db.Fetch<PropagationSettingSchema>(query, propagationSetting.Name).FirstOrDefault();
                if (existingSetting is not null)
                {
                    scope.Complete();
                    return existingSetting;
                }
            }

            var propagationSettingObject = McpsSchemaMapper.MapToSchema(propagationSetting);

            if (propagationSetting.FallbackSetting is not null)
            {
                propagationSettingObject.FallbackId = propagationSetting.FallbackSetting.Id;
            }

            var entryObject = db.Insert<PropagationSettingSchema>("McpsPropagationSettings", "Id", propagationSettingObject);
            var insertedObject = db.Fetch<PropagationSettingSchema>("SELECT * FROM McpsPropagationSettings WHERE Id = @0", entryObject).FirstOrDefault();
            scope.Complete();

            if (insertedObject is not null) return insertedObject;
            else throw new Exception("Error in CreatePropagationSetting");

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in CreatePropagationSetting");
            scope.Complete();
            throw;
        }
    }

    public PropagationSettingSchema CreatePropagationSetting(PropagationSettingSchema propagationSetting)
    {
        using var scope = scopeProvider.CreateScope();
        var db = scope.Database;
        try
        {
            if (propagationSetting.Name is not null)
            {
                var query = "SELECT * FROM McpsPropagationSettings WHERE Name = @0";


                var existingSetting = db.Fetch<PropagationSettingSchema>(query, propagationSetting.Name).FirstOrDefault();
                if (existingSetting is not null)
                {
                    scope.Complete();
                    return existingSetting;
                }
            }

            var entryObject = db.Insert<PropagationSettingSchema>("McpsPropagationSettings", "Id", propagationSetting);
            var insertedObject = db.Fetch<PropagationSettingSchema>("SELECT * FROM McpsPropagationSettings WHERE Id = @0", entryObject).FirstOrDefault();
            scope.Complete();

            if (insertedObject is not null) return insertedObject;
            else throw new Exception("Error in CreatePropagationSetting");

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in CreatePropagationSetting");
            scope.Complete();
            throw;
        }
    }

    public PropagationSettingSchema GetPropagationSetting(int propertySettingId)
    {
        using var scope = scopeProvider.CreateScope();
        var db = scope.Database;
        try
        {
            var returnValue = db.Fetch<PropagationSettingSchema>("SELECT * FROM McpsPropagationSettings WHERE Id = @0", propertySettingId).FirstOrDefault();
            scope.Complete();
            if (returnValue is not null)
            {
                return returnValue;
            }
            else
            {
                _logger.LogInformation($"PropagationSetting for ID: {propertySettingId} not found");
                throw new Exception("Error in GetPropertySetting");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            scope.Complete();
            throw;
        }
    }

    public PropagationSettingSchema GetPropagationSetting(string propagationSettingName)
    {
        using var scope = scopeProvider.CreateScope();
        var db = scope.Database;
        try
        {
            var returnValue = db.Fetch<PropagationSettingSchema>("SELECT * FROM McpsPropagationSettings WHERE Name = @0", propagationSettingName).FirstOrDefault();
            scope.Complete();
            if (returnValue is not null)
            {
                return returnValue;
            }
            else
            {
                _logger.LogInformation($"PropagationSetting for Name: {propagationSettingName} not found");
                throw new Exception("Error in GetPropertySetting");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            scope.Complete();
            throw;
        }
    }

    public List<PropagationSettingSchema> GetPropagationSettingsByDocumentType(string documentType)
    {
        using var scope = scopeProvider.CreateScope();
        var db = scope.Database;
        try
        {
            var returnValue = db.Fetch<PropagationSettingSchema>("SELECT * FROM McpsPropagationSettings WHERE ContentTypes LIKE @0", "%"+documentType+"%");
            scope.Complete();
            if (returnValue is not null)
            {
                return returnValue;
            }
            else
            {
                _logger.LogInformation($"PropagationSettings for Name: {documentType} not found");
                throw new Exception("Error in GetPropagationSettingsByDocumentType");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            scope.Complete();
            throw;
        }
    }

    public List<PropagationSettingSchema> GetAllPropagationSettings()
    {
        using var scope = scopeProvider.CreateScope();
        var db = scope.Database;
        try
        {
            var returnValue = db.Fetch<PropagationSettingSchema>("SELECT * FROM McpsPropagationSettings");
            scope.Complete();
            if (returnValue is not null)
            {
                return returnValue;
            }
            else
            {
                throw new Exception("Error in GetPropagationSettingsByDocumentType");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            scope.Complete();
            throw;
        }
    }

}
