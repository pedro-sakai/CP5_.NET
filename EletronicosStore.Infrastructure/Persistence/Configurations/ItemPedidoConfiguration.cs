using EletronicosStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EletronicosStore.Infrastructure.Persistence.Configurations;

public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("TB_ITEM_PEDIDO");
        builder.HasKey(ip => ip.Id);
        
        builder.Property(ip => ip.PrecoUnitario).HasColumnType("decimal(18,2)");

        builder.HasOne(ip => ip.Pedido)
            .WithMany(p => p.Itens)
            .HasForeignKey(ip => ip.PedidoId);

        builder.HasOne(ip => ip.Produto)
            .WithMany(p => p.ItensPedido)
            .HasForeignKey(ip => ip.ProdutoId);
    }
}