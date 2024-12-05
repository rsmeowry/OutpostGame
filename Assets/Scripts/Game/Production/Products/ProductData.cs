namespace Game.Production.Products
{
    public struct ProductData
    {
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