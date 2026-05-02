using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        sale.SaleNumber = command.SaleNumber;
        sale.SaleDate = command.SaleDate;
        sale.CustomerId = command.CustomerId;
        sale.CustomerName = command.CustomerName;
        sale.BranchId = command.BranchId;
        sale.BranchName = command.BranchName;

        sale.Items.Clear();
        foreach (var itemCmd in command.Items)
        {
            var item = new SaleItem
            {
                Id = itemCmd.Id ?? Guid.NewGuid(),
                SaleId = sale.Id,
                ProductId = itemCmd.ProductId,
                ProductName = itemCmd.ProductName,
                Quantity = itemCmd.Quantity,
                UnitPrice = itemCmd.UnitPrice
            };
            item.ApplyDiscount();
            sale.Items.Add(item);
        }

        sale.RecalculateTotal();
        sale.MarkAsModified();

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}
