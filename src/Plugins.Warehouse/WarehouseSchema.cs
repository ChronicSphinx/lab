namespace Plugins.Warehouse
{
    internal static class WarehouseSchema
    {
        public const string ItemEntity = "lab_warehouseitem";
        public const string TransactionEntity = "lab_warehousetransaction";
        public const string AvailableQuantity = "lab_availablequantity";
        public const string ItemId = "lab_itemid";
        public const string Quantity = "lab_quantity";
        public const string TransactionType = "lab_transactiontype";
        public const int Inbound = 100000000;
        public const int Outbound = 100000001;
    }
}