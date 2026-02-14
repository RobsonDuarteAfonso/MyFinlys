using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Interfaces;

public interface IPaginationService
{
    void ValidatePaginationParams(PaginationParams @params);
}

public class PaginationService : IPaginationService
{
    public void ValidatePaginationParams(PaginationParams @params)
    {
        @params.Validate();
    }
}
