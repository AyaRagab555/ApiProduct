using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductsWithTypeAndBrandSpecifications : BaseSpecifications<Product>
    {
        public ProductsWithTypeAndBrandSpecifications(ProductSpecParams productSpecParams) 
            : base(product => 
                (!productSpecParams.TypeId.HasValue || product.ProductTypeId == productSpecParams.TypeId) &&
                (!productSpecParams.BrandId.HasValue || product.ProductBrandId == productSpecParams.BrandId)
            )
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
            AddOrderBy(product => product.Name);
            if(!string.IsNullOrEmpty(productSpecParams.Sort))
            {
                switch(productSpecParams.Sort)
                {
                    case "PriceAsc":
                        AddOrderBy(product => product.Price);
                        break;
                    case "PriceDesc":
                        AddOrderByDescending(product => product.Price);
                        break;
                    default: 
                        AddOrderBy(product => product.Name);
                        break;
                }
            }
        }

        public ProductsWithTypeAndBrandSpecifications(int id)
            : base(product => product.Id == id)
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);

        }
    } 
}
