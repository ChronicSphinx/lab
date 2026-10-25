using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TALXIS.DevKit.Build.Dataverse.Plugin;

namespace Plugins.Warehouse
{
    [PluginStep("Create", WarehouseSchema.TransactionEntity, PluginStage.PostOperation)]
    public class SubtractQuantityPlugin : PluginBase
    {
        public SubtractQuantityPlugin() : this(null, null)
        {
        }

        public SubtractQuantityPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(SubtractQuantityPlugin))
        {
        }

        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var target = context.InputParameters.Contains("Target")
                ? context.InputParameters["Target"] as Entity
                : null;

            if (target == null || target.LogicalName != WarehouseSchema.TransactionEntity)
            {
                return;
            }

            if (!target.Contains(WarehouseSchema.Quantity)
                || !target.Contains(WarehouseSchema.ItemId)
                || !target.Contains(WarehouseSchema.TransactionType))
            {
                return;
            }

            var transactionType = target[WarehouseSchema.TransactionType] as OptionSetValue;
            if (transactionType == null
                || (transactionType.Value != WarehouseSchema.Inbound
                    && transactionType.Value != WarehouseSchema.Outbound))
            {
                return;
            }

            var itemReference = target[WarehouseSchema.ItemId] as EntityReference;
            if (itemReference == null)
            {
                return;
            }

            var quantity = (int)target[WarehouseSchema.Quantity];
            var service = localPluginContext.OrgSvcFactory.CreateOrganizationService(context.UserId);
            var item = service.Retrieve(
                WarehouseSchema.ItemEntity,
                itemReference.Id,
                new ColumnSet(WarehouseSchema.AvailableQuantity));
            var available = item.GetAttributeValue<int>(WarehouseSchema.AvailableQuantity);
            var newAvailable = transactionType.Value == WarehouseSchema.Inbound
                ? available + quantity
                : available - quantity;
            var itemUpdate = new Entity(WarehouseSchema.ItemEntity, itemReference.Id);
            itemUpdate[WarehouseSchema.AvailableQuantity] = newAvailable;

            service.Update(itemUpdate);
        }
    }
}