﻿using DomainModule.Dto;
using DomainModule.Dto.User;
using DomainModule.Entity;
using DomainModule.Exceptions;
using DomainModule.RepositoryInterface;
using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace ServiceModule.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailMessageService _emailMessageService;
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUserRepository userRepo,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            IEmailMessageService emailMessageService)
        {
            _userRepo = userRepo;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _emailMessageService = emailMessageService;
        }
        public async Task Activate(string id)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var user = await _userRepo.GetByIdString(id).ConfigureAwait(false) ?? throw new UserNotFoundException();
                    user.Activate();
                    _userRepo.Update(user);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    await tx.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    throw;
                }

            }

        }


        public async Task<UserResponseDto> Create(UserDto dto)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    await ValidateUser(dto.MobileNumber, dto.EmailAddress);
                    var user = new User(dto.Name, dto.UserName, dto.EmailAddress, dto.Type)
                    {
                        PhoneNumber = dto.MobileNumber,
                        EmailConfirmed = dto.IsEmailConfirmed
                    };
                    var result = await _userManager.CreateAsync(user, dto.Password).ConfigureAwait(false);
                    var userReponseModel = new UserResponseDto() { UserId = user.Id };
                    if (result.Succeeded)
                    {
                        if (dto.Roles != null)
                        {
                            foreach (var roleId in dto.Roles)
                            {
                                var role = await _roleManager.FindByIdAsync(roleId).ConfigureAwait(false) ?? throw new RoleNotFoundException();
                                await _userManager.AddToRoleAsync(user, role.Name).ConfigureAwait(false);
                            }
                        }

                        if (!dto.IsEmailConfirmed)
                        {
                            if (string.IsNullOrEmpty(dto.EmailConfirmationTemplate)) throw new CustomException("No Email template found For email Confirmation");
                            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                            var emailConfirmationLink = $"{dto.CurrentSiteDomain}/Account/Account/ConfirmEmail?email={user.Email}&token={System.Web.HttpUtility.UrlEncode(token)}";
                            var Replacements = new Dictionary<string, string>();
                            Replacements.Add("{User}", dto.Name);
                            Replacements.Add("{ConfirmationLink}", emailConfirmationLink);
                            foreach (var Replacement in Replacements)
                            {
                                dto.EmailConfirmationTemplate = dto.EmailConfirmationTemplate.Replace(Replacement.Key, Replacement.Value);
                            }
                            var emailMessageDto = new EmailMessageDto("Account Confirmation Link", dto.EmailConfirmationTemplate, "freeman18@ethereal.email", EmailMessage.HighPriority);
                            emailMessageDto.EmailRecipients.Add(new EmailRecipientDto(dto.EmailAddress));
                            await _emailMessageService.CreateEmailMessageAndSendEmail(emailMessageDto);
                        }

                    }
                    else
                    {
                        var errors = "</br>";
                        foreach (var error in result.Errors)
                        {
                            errors = errors + $"{error.Description} </br>";
                        }
                        throw new CustomException(errors);
                    }
                    await tx.CommitAsync().ConfigureAwait(false);
                    return userReponseModel;
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
            }

        }


        public async Task Deactivate(string id)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var user = await _userRepo.GetByIdString(id).ConfigureAwait(false) ?? throw new UserNotFoundException();
                    user.Deactivate();
                    _userRepo.Update(user);
                    await _unitOfWork.CompleteAsync();
                    await tx.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
            }

        }

        public async Task Edit(UserEditDto dto)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(dto.Id).ConfigureAwait(false) ?? throw new UserNotFoundException();
                    user.Update(dto.Name, dto.UserName, dto.EmailAddress, dto.MobileNumber);
                    var response = await _userManager.UpdateAsync(user).ConfigureAwait(false);
                    if (response.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                        foreach (var role in userRoles)
                        {
                            await _userManager.RemoveFromRoleAsync(user, role).ConfigureAwait(false);
                        }
                        foreach (var roleId in dto.Roles)
                        {
                            var role = await _roleManager.FindByIdAsync(roleId).ConfigureAwait(false) ?? throw new RoleNotFoundException();
                            await _userManager.AddToRoleAsync(user, role.Name).ConfigureAwait(false);
                        }

                    }
                    else
                    {
                        throw new CustomException(string.Join("</br>", response.Errors.Select(a => a.Description).ToList()));
                    }
                    await tx.CommitAsync().ConfigureAwait(false);

                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
            }
        }



        private async Task ValidateUser(string mobile, string email, User? user = null)
        {
            if (string.IsNullOrWhiteSpace(mobile) || string.IsNullOrWhiteSpace(email))
            {
                if (string.IsNullOrWhiteSpace(mobile)) throw new CustomException("Mobile Number is required");
                if (string.IsNullOrWhiteSpace(email)) throw new CustomException("Email address is required");
                return;
            }
            var userWithSameMobile = await _userRepo.GetByMobile(mobile).ConfigureAwait(false);
            if (userWithSameMobile != null && user != userWithSameMobile) throw new CustomException("User With Same Number Already Exists");
            var userWithSameEmail = await _userRepo.GetByEmail(email).ConfigureAwait(false);
            if (userWithSameEmail != null && user != userWithSameEmail) throw new CustomException("User With Same Email Already Exists");

        }
    }
}
