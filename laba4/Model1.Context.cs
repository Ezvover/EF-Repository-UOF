﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace laba4
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class GoodsLabEntities : DbContext
    {
        public GoodsLabEntities()
            : base("name=GoodsLabEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Good> Goods { get; set; }
    
        public virtual ObjectResult<GetGoodsByCategory_Result> GetGoodsByCategory(string category)
        {
            var categoryParameter = category != null ?
                new ObjectParameter("Category", category) :
                new ObjectParameter("Category", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetGoodsByCategory_Result>("GetGoodsByCategory", categoryParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> usp_GetGoods()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("usp_GetGoods");
        }
    }
}
