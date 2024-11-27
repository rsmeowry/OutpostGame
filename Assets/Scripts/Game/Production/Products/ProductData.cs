namespace Game.Production.Products
{
    public struct ProductData
    {
        // TODO: more product data?
        public float BuyPrice;
        public float SellPrice;
        public int BaseStock;

        public ProductData(float buyPrice, float sellPrice, int baseStock)
        {
            BuyPrice = buyPrice;
            SellPrice = sellPrice;
            BaseStock = baseStock;
        }
    }
}