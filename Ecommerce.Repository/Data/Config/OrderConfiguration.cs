using System;
using Ecommerce.Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Repository.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // ÈäÞæáå Åä ÇáÚäæÇä Ïå ÌÒÁ ãä ÌÏæá ÇáÃæÑÏÑ ãÔ ÌÏæá ãäÝÕá
            builder.OwnsOne(o => o.ShipToAddress, a => { a.WithOwner(); });

            // ÊÍæíá ÇáÜ Enum áäÕ Ýí ÇáÏÇÊÇÈíÒ (ÚÔÇä íÊßÊÈ Pending ÈÏá 0)
            builder.Property(s => s.Status)
                .HasConversion(
                    o => o.ToString(),
                    o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
                );

            // áæ ãÓÍÊ ÇáÃæÑÏÑ¡ ÇãÓÍ ßá ÇáÚäÇÕÑ Çááí ÌæÇå
            builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}