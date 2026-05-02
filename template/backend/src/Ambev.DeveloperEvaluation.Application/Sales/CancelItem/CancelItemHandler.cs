using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem;

public class CancelItemHandler : IRequestHandler<CancelItemCommand, CancelItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public CancelItemHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<CancelItemResult> Handle(CancelItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelItemValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        sale.CancelItem(request.ItemId);
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        return new CancelItemResult
        {
            SaleId = sale.Id,
            ItemId = request.ItemId,
            IsCancelled = true,
            SaleTotalAmount = sale.TotalAmount
        };
    }
}
