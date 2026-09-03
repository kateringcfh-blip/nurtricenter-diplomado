namespace MsProduccionAlimentos.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> GuardarCambios(CancellationToken cancellationToken = default);
}
