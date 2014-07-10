namespace WCFNamedPipesExample.Services
{
    /// <summary>
    /// Data transfer object for products
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or set the manufacturer.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the product number.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal Price { get; set; }
    }
}
