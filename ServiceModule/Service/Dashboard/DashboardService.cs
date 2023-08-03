using DomainModule.Dto;
using DomainModule.Entity;
using DomainModule.Enums;
using DomainModule.Exceptions;
using DomainModule.RepositoryInterface;
using DomainModule.ServiceInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceModule.Service
{
    public class DashboardService : DashboardServiceInterface
    {
        private readonly TodoRepositoryInterface _todoRepo;
        private readonly SharedTodoRepositoryInterface _sharedTodoRepo;
        private readonly UserRepositoryInterface _userRepo;
        public DashboardService(SharedTodoRepositoryInterface sharedTodoRepo, UserRepositoryInterface userRepo, TodoRepositoryInterface todoRepo)
        {
            _sharedTodoRepo = sharedTodoRepo;
            _todoRepo = todoRepo;
            _userRepo = userRepo;

        }
        public async Task<DashboardDto> GetDashboardData(string userId)
        {
            try
            {
                var user = await _userRepo.GetByIdString(userId).ConfigureAwait(false) ?? throw new CustomException("Current User not found");

                var monthDictionary = new Dictionary<int, int>()
                  {
                      {1,0 },
                      {2,0 },
                      {3,0 },
                      {4,0 },
                      {5,0 },
                      {6,0 },
                      {7,0 },
                      {8,0 },
                      {9,0 },
                      {10,0 },
                      {11,0 },
                      {12,0 },
                  };
                var totalUser = await _userRepo.GetQueryable().CountAsync().ConfigureAwait(false);
                var todoQuerable = _todoRepo.GetQueryable();
                var sharedTodoQueryable = _sharedTodoRepo.GetQueryable();

                var totalTodos = 0;
                var totalActiveTodos = 0;
                var totalCompletedTodos = 0;
                var totalSharedTodos = 0;
                var totalHighPriorityTodo = 0;
                var totalMidPriorityTodo = 0;
                var totalLowPriorityTodo = 0;
                if (!user.IsSuperAdmin)
                {
                    todoQuerable = todoQuerable.Where(a => a.CreatedBy == userId);
                    sharedTodoQueryable = sharedTodoQueryable.Where(a => a.UserId == userId);
                }

                totalTodos = await todoQuerable.CountAsync().ConfigureAwait(false);
                totalActiveTodos = await todoQuerable.Where(a => a.Status == TodoEntity.StatusActive).CountAsync().ConfigureAwait(false);
                totalCompletedTodos = await todoQuerable.Where(a => a.Status == TodoEntity.StatusCompleted).CountAsync().ConfigureAwait(false);
                totalSharedTodos = await sharedTodoQueryable.CountAsync().ConfigureAwait(false);
                totalHighPriorityTodo = await todoQuerable.Where(a => a.Status == TodoEntity.StatusActive && a.PriorityLevel == (int)TodoPriorityEnum.High).CountAsync().ConfigureAwait(false);
                totalMidPriorityTodo = await todoQuerable.Where(a => a.Status == TodoEntity.StatusActive && a.PriorityLevel == (int)TodoPriorityEnum.Medium).CountAsync().ConfigureAwait(false);
                totalLowPriorityTodo = await todoQuerable.Where(a => a.Status == TodoEntity.StatusActive && a.PriorityLevel == (int)TodoPriorityEnum.Low).CountAsync().ConfigureAwait(false);
                var monthwiseTodosCountsOfThisYear = await todoQuerable.Where(a => a.CreatedOn.Year == DateTime.Now.Year).GroupBy(m => m.CreatedOn.Month).ToListAsync();
                foreach (var data in monthwiseTodosCountsOfThisYear)
                {
                    monthDictionary[data.Key] = data.Count();
                }


                var dashboardData = new DashboardDto()
                {
                    TotalActiveTodoCount = totalActiveTodos,
                    TotalCompletedTodo = totalCompletedTodos,
                    TotalSharedTodoCount = totalSharedTodos,
                    TotalTodoCount = totalTodos,
                    TotalUser = totalUser,
                    TotalHighPriorityTodo = totalHighPriorityTodo,
                    TotalLowPriorityTodo = totalLowPriorityTodo,
                    TotalMediumPriorityTodo = totalMidPriorityTodo,
                    TotalTodosEachMonth = monthDictionary.Select(a => a.Value).ToList(),
                };
                return dashboardData;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
