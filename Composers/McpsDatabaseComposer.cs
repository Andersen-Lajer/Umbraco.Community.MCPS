using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Community.MCPS.Models.Schemas;

namespace Umbraco.Community.MCPS.Composers;

public class McpsDatabaseComposer : ComponentComposer<McpsDatabaseComponent>
{
}

public class McpsDatabaseComponent(
    ICoreScopeProvider coreScopeProvider,
    IMigrationPlanExecutor migrationPlanExecutor,
    IKeyValueService keyValueService,
    IRuntimeState runtimeState) : IAsyncComponent
{
    public Task InitializeAsync(bool isRestarting, CancellationToken cancellationToken)
    {
        if (runtimeState.Level < RuntimeLevel.Run)
        {
            return Task.CompletedTask;
        }

        var migrationPlan = new MigrationPlan("McpsDatabasePlan");

        migrationPlan.From(string.Empty)
            .To<AddPropagationSettingsTable>("mcpspropagationsettings-db")
            .To<AddPropagationRelationsTable>("mcpspropagationrelations-db");

        var upgrader = new Upgrader(migrationPlan);
        upgrader.Execute(migrationPlanExecutor, coreScopeProvider, keyValueService);
        return Task.CompletedTask;
    }

    public Task TerminateAsync(bool isRestarting, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class AddPropagationRelationsTable(IMigrationContext context) : MigrationBase(context)
{
    protected override void Migrate()
    {
        Logger.LogDebug("Running migration {MigrationStep}", "AddPropagationRelationsTable");

        if (TableExists("McpsPropagationRelations") == false)
        {
            Create.Table<PropagationRelationsSchema>().Do();
        }
        else
        {
            Logger.LogDebug("The database table {DbTable} already exists, skipping", "McpsPropagationRelations");
        }
    }
}

public class AddPropagationSettingsTable(IMigrationContext context) : MigrationBase(context)
{
    protected override void Migrate()
    {
        Logger.LogDebug("Running migration {MigrationStep}", "AddPropagationSettingsTable");

        if (TableExists("McpsPropagationSettings") == false)
        {
            Create.Table<PropagationSettingSchema>().Do();
        }
        else
        {
            Logger.LogDebug("The database table {DbTable} already exists, skipping", "McpsPropagationSettings");
        }
    }
}