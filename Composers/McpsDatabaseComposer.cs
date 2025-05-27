using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Community.MCPS.Models;
using Umbraco.Community.MCPS.Models.Schemas;

namespace Umbraco.Community.MCPS.Composers;

public class McpsDatabaseComposer : ComponentComposer<McpsDatabaseComponent>
{
}

public class McpsDatabaseComponent : IAsyncComponent
{
    private readonly ICoreScopeProvider _coreScopeProvider;
    private readonly IMigrationPlanExecutor _migrationPlanExecutor;
    private readonly IKeyValueService _keyValueService;
    private readonly IRuntimeState _runtimeState;

    public McpsDatabaseComponent(
        ICoreScopeProvider coreScopeProvider,
        IMigrationPlanExecutor migrationPlanExecutor,
        IKeyValueService keyValueService,
        IRuntimeState runtimeState)
    {
        _coreScopeProvider = coreScopeProvider;
        _migrationPlanExecutor = migrationPlanExecutor;
        _keyValueService = keyValueService;
        _runtimeState = runtimeState;
    }

    public Task InitializeAsync(bool isRestarting, CancellationToken cancellationToken)
    {
        if (_runtimeState.Level < RuntimeLevel.Run)
        {
            return Task.CompletedTask;
        }

        // Create a migration plan for a specific project/feature
        // We can then track that latest migration state/step for this project/feature
        var migrationPlan = new MigrationPlan("McpsDatabasePlan");

        // This is the steps we need to take
        // Each step in the migration adds a unique value
        migrationPlan.From(string.Empty)
            .To<AddPropagationSettingsTable>("mcpspropagationsettings-db")
            .To<AddPropagationRelationsTable>("mcpspropagationrelations-db");


        // Go and upgrade our site (Will check if it needs to do the work or not)
        // Based on the current/latest step
        var upgrader = new Upgrader(migrationPlan);
        upgrader.Execute(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        return Task.CompletedTask;
    }

    public Task TerminateAsync(bool isRestarting, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class AddPropagationRelationsTable : MigrationBase
{
    public AddPropagationRelationsTable(IMigrationContext context) : base(context)
    {
    }
    protected override void Migrate()
    {
        Logger.LogDebug("Running migration {MigrationStep}", "AddPropagationRelationsTable");

        // Lots of methods available in the MigrationBase class - discover with this.
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

public class AddPropagationSettingsTable : MigrationBase
{
    public AddPropagationSettingsTable(IMigrationContext context) : base(context)
    {
    }
    protected override void Migrate()
    {
        Logger.LogDebug("Running migration {MigrationStep}", "AddPropagationSettingsTable");

        // Lots of methods available in the MigrationBase class - discover with this.
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
