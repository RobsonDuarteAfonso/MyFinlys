using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IBankRepository _bankRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IRegisterRepository _registerRepository;
    private readonly ICreditCardRepository _creditCardRepository;

    public AccountService(
        IAccountRepository accountRepo,
        IBankRepository bankRepo,
        IUserRepository userRepo,
        IEventRepository eventRepo,
        IRegisterRepository registerRepo,
        ICreditCardRepository creditCardRepo)
    {
        _accountRepository = accountRepo;
        _bankRepository = bankRepo;
        _userRepository = userRepo;
        _eventRepository = eventRepo;
        _registerRepository = registerRepo;
        _creditCardRepository = creditCardRepo;
    }

    public async Task<AccountDetailDto?> GetByIdAsync(Guid id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        return account is null ? null : AccountMapper.ToDetailDto(account);
    }

    public async Task<AccountDetailDto?> GetByNumberAsync(string number)
    {
        var account = await _accountRepository.GetByNumberAsync(number);
        return account is null ? null : AccountMapper.ToDetailDto(account);
    }

    public async Task<IEnumerable<AccountSummaryDto>> GetByUserIdAsync(Guid userId)
    {
        var accounts = await _accountRepository.GetByUserIdAsync(userId);
        return accounts.Select(a => AccountMapper.ToSummaryDto(a, userId));
    }

    public async Task<IEnumerable<AccountSummaryDto>> GetAllAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Select(a => AccountMapper.ToSummaryDto(a, null));
    }

    public async Task<PaginatedResult<AccountSummaryDto>> GetAllPaginatedAsync(PaginationParams @params)
    {
        @params.Validate();
        var allAccounts = await _accountRepository.GetAllAsync();
        var filteredAccounts = allAccounts.ToList();
        var totalCount = filteredAccounts.Count;
        
        var items = filteredAccounts
            .Skip((@params.PageNumber - 1) * @params.PageSize)
            .Take(@params.PageSize)
            .Select(a => AccountMapper.ToSummaryDto(a))
            .ToList();

        return new PaginatedResult<AccountSummaryDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = @params.PageNumber,
            PageSize = @params.PageSize
        };
    }

    public async Task<AccountDetailDto> CreateAsync(AccountCreateDto dto)
    {
        if (!Enum.TryParse<AccountType>(dto.Type, true, out var parsedType))
            throw new ArgumentException("Invalid account type.", nameof(dto.Type));

        var bank = await _bankRepository.GetByIdAsync(dto.BankId)
                   ?? throw new ArgumentException("Bank not found.", nameof(dto.BankId));

        var account = Account.Create(dto.Number, parsedType, bank.Id);

        var users = new List<User>();
        var isFirst = true;
        foreach (var userId in dto.UserIds)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new ArgumentException($"User not found: {userId}");
            account.AddUser(user, isFirst ? AccessLevel.Owner : AccessLevel.Editor);
            users.Add(user);
            isFirst = false;
        }

        await _accountRepository.AddAsync(account);
        await _accountRepository.SaveChangesAsync();
        
        // Set the Bank navigation property for the mapper
        var bankProperty = typeof(Account).GetProperty("Bank", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        bankProperty?.SetValue(account, bank);

        // Associate the User objects in memory for the mapper after save to avoid EF Core insert traverses
        foreach (var userAccount in account.UserAccounts)
        {
            var user = users.FirstOrDefault(u => u.Id == userAccount.UserId);
            if (user != null)
            {
                userAccount.User = user;
            }
        }
        
        return AccountMapper.ToDetailDto(account);
    }

    public async Task<AccountDetailDto?> UpdateAsync(Guid id, AccountUpdateDto dto)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account is null)
            return null;

        if (!Enum.TryParse<AccountType>(dto.Type, true, out var parsedType))
            throw new ArgumentException("Invalid account type.", nameof(dto.Type));

        account.Update(dto.Number, parsedType, dto.BankId);
        account.SetUpdatedAt();

        // 1. Sync UserAccounts list
        var existingUserIds = account.UserAccounts.Select(ua => ua.UserId).ToList();
        var proposedUsers = dto.Users ?? new List<UserAssociationDto>();
        var proposedUserIds = proposedUsers.Select(u => u.UserId).ToList();

        // Remove users no longer associated
        foreach (var existingId in existingUserIds)
        {
            if (!proposedUserIds.Contains(existingId))
            {
                account.RemoveUser(existingId);
            }
        }

        var users = new List<User>();
        
        // Pre-fetch already associated users that remain
        foreach (var ua in account.UserAccounts)
        {
            if (ua.User != null)
            {
                users.Add(ua.User);
            }
            else
            {
                var userObj = await _userRepository.GetByIdAsync(ua.UserId);
                if (userObj != null)
                {
                    users.Add(userObj);
                }
            }
        }

        // Add new ones or update existing ones' roles
        foreach (var propUser in proposedUsers)
        {
            if (!Enum.TryParse<AccessLevel>(propUser.AccessLevel, true, out var parsedLevel))
                parsedLevel = AccessLevel.Editor;

            var existingUa = account.UserAccounts.FirstOrDefault(ua => ua.UserId == propUser.UserId);
            if (existingUa != null)
            {
                if (existingUa.AccessLevel != AccessLevel.Owner)
                {
                    account.UpdateUserAccess(propUser.UserId, parsedLevel);
                }
            }
            else
            {
                var userObj = await _userRepository.GetByIdAsync(propUser.UserId)
                           ?? throw new ArgumentException($"User not found: {propUser.UserId}");
                account.AddUser(userObj, parsedLevel);
                users.Add(userObj);
            }
        }

        await _accountRepository.UpdateAsync(account);
        await _accountRepository.SaveChangesAsync();

        // Set the Bank navigation property for the mapper
        var bank = await _bankRepository.GetByIdAsync(dto.BankId);
        if (bank != null)
        {
            var bankProperty = typeof(Account).GetProperty("Bank", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            bankProperty?.SetValue(account, bank);
        }

        // Set User properties in memory after save to avoid EF Core insert traverses
        foreach (var userAccount in account.UserAccounts)
        {
            var user = users.FirstOrDefault(u => u.Id == userAccount.UserId);
            if (user != null)
            {
                userAccount.User = user;
            }
        }

        return AccountMapper.ToDetailDto(account);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _accountRepository.GetByIdAsync(id);
        if (existing is null)
            return false;

        // Check active events
        var events = await _eventRepository.GetByAccountIdAsync(id);
        if (events.Any(e => !e.IsDeleted))
        {
            throw new System.InvalidOperationException("Cannot delete account because there are active recurring events linked to it.");
        }

        // Check active registers
        var registers = await _registerRepository.GetByAccountIdAsync(id);
        if (registers.Any(r => !r.IsDeleted))
        {
            throw new System.InvalidOperationException("Cannot delete account because there are transaction records linked to it.");
        }

        // Check active credit cards
        var cards = await _creditCardRepository.GetByAccountAsync(id);
        if (cards.Any(c => !c.IsDeleted))
        {
            throw new System.InvalidOperationException("Cannot delete account because there are credit cards linked to it.");
        }

        await _accountRepository.DeleteAsync(id);
        await _accountRepository.SaveChangesAsync();
        return true;
    }
}
