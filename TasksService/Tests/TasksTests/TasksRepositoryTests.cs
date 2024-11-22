using System;
using System.ComponentModel.Design;
using Castle.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using TasksService.DataAccess.Entities;
using TasksService.DataAccess.EntityFramework;
using TasksService.DataAccess.Repositories.Abstractions;
using TasksService.DataAccess.Repositories.Implementations;
using TasksServiceTasks = TasksService.DataAccess.Entities;


namespace TasksTests
{
    public class TasksRepositoryTests : IDisposable
    {
        private readonly TasksDbContext _context;

        public TasksRepositoryTests()
        {
            _context = GetDbContext();
            _context.Database.EnsureCreated();
        }

        private DbContextOptions<TasksDbContext> GetPostgresOptions(string connectionString)
        {
            return new DbContextOptionsBuilder<TasksDbContext>()
                .UseNpgsql(connectionString)
                .EnableSensitiveDataLogging(true)
                .Options;
        }
        private Microsoft.Extensions.Configuration.IConfiguration GetTestConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "ConnectionStrings:TasksDb", "Host=localhost;Port=5432;Database=TasksTestDb;Username=postgres;Password=Gfhjkm_123;Persist Security Info=True"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }
        private TasksDbContext GetDbContext()
        {
            var configuration = GetTestConfiguration();
            var connectionString = configuration.GetConnectionString("TasksDb");
            var options = GetPostgresOptions(connectionString ?? "");
            return new TasksDbContext(options, configuration);
        }
        private Mock<IHistoryRepository> GetMockHistoryRepository()
        {
            return new Mock<IHistoryRepository>();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        private async Task<long> createTestCompanyAsync()
        {
            try
            {
                string companyName = "TestCompany";
                var found = _context.Companies.FirstOrDefault(x => x.Name == companyName);
                if (null != found)
                    return found.Id;
                var company = new TasksCompany() { Name = companyName, Description = companyName };
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                return company.Id;
            }
            catch
            {
            }
            return 0;
        }

        /// <summary>
        /// Creates record in Company_Employees table
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>companyId</returns>
        private async Task<long> createTestCompanyEmployeeAsync(Guid userId)
        {
            try
            {
                var companyId = await createTestCompanyAsync();
                if (0 == companyId)
                    return 0;

                var found = _context.CompanyEmployees.FirstOrDefault(x => x.CompanyId == companyId && x.EmployeeId == userId);
                if (null != found)
                    return companyId;

                var company = new CompanyEmployee() { CompanyId = companyId, EmployeeId = userId };
                _context.CompanyEmployees.Add(company);
                await _context.SaveChangesAsync();
                return company.CompanyId;
            }
            catch
            {
            }
            return 0;
        }

        private async Task<long> createTestProjectAsync(long companyId)
        {
            // Arrange
            var historyRepo = GetMockHistoryRepository();
            var repo = new CompanyProjectsRepository(_context.Configuration, historyRepo.Object);
            Exception? exception = null;
            return await repo.CreateCompanyProject(Guid.Empty, new CompanyProject() { CompanyId = companyId, Name = "Test", Description = "Test project" });
        }

        private async Task<long> createTestTemplate(long companyId)
        {
            var historyRepo = GetMockHistoryRepository();
            var repo = new TemplatesRepository(_context.Configuration, historyRepo.Object);

            // Act & Assert
            var nodes = new List<WfNodesTemplate>()
                {
                    new WfNodesTemplate() { InternalNum = 1, Name = "FirstNode", Description = "First node description", Terminating = false, IconId = 0 },
                    new WfNodesTemplate() { InternalNum = 2, Name = "SecondNode", Description = "Second node description", Terminating = false, IconId = 0 },
                    new WfNodesTemplate() { InternalNum = 3, Name = "ThirdNode", Description = "Third node description", Terminating = true, IconId = 0 }
                };
            var edges = new List<WfEdgesTemplate>()
                {
                    new WfEdgesTemplate(){ Name = "FromFirstToSecond", InternalNum = 1, NodeFrom = 1, NodeTo = 2 },
                    new WfEdgesTemplate(){ Name = "FromSecondToThird", InternalNum = 2, NodeFrom = 2, NodeTo = 3 },
                    new WfEdgesTemplate(){ Name = "FromSecondToFirst", InternalNum = 3, NodeFrom = 2, NodeTo = 1 },
                    new WfEdgesTemplate(){ Name = "FromFirstToThird",  InternalNum = 4, NodeFrom = 1, NodeTo = 3 }
                };

            var long_res = await repo.CreateTemplate(Guid.Empty, "TestTemplate", "Test Template Description", companyId, nodes, edges);
            return long_res;
        }



        [Fact]
        public async void UrgenciesRepo_CanCRUD()
        {
            // Arrange
            var historyRepo = GetMockHistoryRepository();
            var repo = new UrgenciesRepository(_context.Configuration, historyRepo.Object);
            Exception? exception = null;

            try
            {
                // Act & Assert
                var long_res = await repo.CreateUrgency(Guid.Empty, "Test", "TestTest");
                Assert.True(long_res > 0);
                var found = await _context.Urgencies.FirstOrDefaultAsync(x => x.Id == long_res);
                Assert.NotNull(found);
                found.Name = "TestModified";
                found.Description = "TestTestModified";
                var bool_res = await repo.ModifyUrgency(Guid.Empty, 0, found);
                Assert.True(bool_res);
                var mov_val = await _context.Urgencies.Where(x => x.Name == found.Name && x.Description == found.Description).ToListAsync();
                Assert.True(mov_val.Any());
                bool_res = await repo.DeleteUrgency(Guid.Empty, found.Id);
                Assert.True(bool_res);

            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.Null(exception);
        }

        [Fact]
        public async void CompaniesRepo_CanCRUD()
        {
            // Arrange
            var historyRepo = GetMockHistoryRepository();
            var repo = new CompanyRepository(_context.Configuration, historyRepo.Object);
            Exception? exception = null;
            try
            {


                // Act & Assert
                var long_res = await repo.CreateCompany(Guid.Empty, "Horns & Hooves", "Donut's Hole");
                Assert.True(long_res > 0);
                var found = await _context.Companies.FirstOrDefaultAsync(x => x.Id == long_res);
                Assert.NotNull(found);
                found.Name = "TestModified";
                found.Description = "TestTestModified";
                var bool_res = await repo.ModifyCompany(Guid.Empty, 0, found.Id, found.Name, found.Description);
                Assert.True(bool_res);
                var mov_val = await _context.Companies.Where(x => x.Name == found.Name && x.Description == found.Description).ToListAsync();
                Assert.True(mov_val.Any());
                bool_res = await repo.DeleteCompany(Guid.Empty, found.Id);
                Assert.True(bool_res);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.Null(exception);
        }

        [Fact]
        public async void CompanyProjectsRepo_CanCRUD()
        {
            // Arrange
            var historyRepo = GetMockHistoryRepository();
            var repo = new CompanyProjectsRepository(_context.Configuration, historyRepo.Object);
            Exception? exception = null;
            var companyId = await createTestCompanyAsync();

            try
            {
                // Act & Assert
                var long_res = await createTestProjectAsync(companyId);
                Assert.True(long_res > 0);
                var found = await _context.CompanyProjects.FirstOrDefaultAsync(x => x.Id == long_res);
                Assert.NotNull(found);
                found.Name = "TestModified";
                found.Description = "Test project Modified";
                var bool_res = await repo.ModifyCompanyProject(Guid.Empty, 0, found);
                Assert.True(bool_res);
                var mov_val = await _context.CompanyProjects.Where(x => x.Name == found.Name && x.Description == found.Description).ToListAsync();
                Assert.True(mov_val.Any());
                bool_res = await repo.DeleteCompanyProject(Guid.Empty, found.Id);
                Assert.True(bool_res);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.Null(exception);

        }

        [Fact]
        public async void ProjectAreaRepo_CanCRUD()
        {
            // Arrange
            var historyRepo = GetMockHistoryRepository();
            var repo = new ProjectAreaRepository(_context.Configuration, historyRepo.Object);
            Exception? exception = null;
            var companyId = await createTestCompanyAsync();
            var projectId = await createTestProjectAsync(companyId);

            try
            {
                // Act & Assert
                var long_res = await repo.CreateProjectArea(Guid.Empty, new ProjectArea() { Name = "Test", Description = "Test area", ProjectId = projectId });
                Assert.True(long_res > 0);

                var areas = await repo.GetProjectAreas(projectId, long_res);
                Assert.True(areas.Any());
                var area = areas[0];

                area.Name = "TestModified";
                area.Description = "Project area Modified";
                var bool_res = await repo.ModifyProjectArea(Guid.Empty, 0, area);
                Assert.True(bool_res);
                var mov_val = await _context.ProjectAreas.Where(x => x.Name == area.Name && x.Description == areas[0].Description).ToListAsync();
                Assert.True(mov_val.Any());
                bool_res = await repo.DeleteProjectArea(Guid.Empty, area.Id);
                Assert.True(bool_res);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.Null(exception);

        }

        [Fact]
        public async void TemplatesRepo_CanCRUD()
        {
            // Arrange
            var historyRepo = GetMockHistoryRepository();
            var repo = new TemplatesRepository(_context.Configuration, historyRepo.Object);
            Exception? exception = null;

            try
            {
                var companyId = await createTestCompanyAsync();
                long long_res = await createTestTemplate(companyId);
                // Act & Assert
                /*
                var nodes = new List<WfNodesTemplate>()
                {
                    new WfNodesTemplate() { InternalNum = 1, Name = "FirstNode", Description = "First node description", Terminating = false, IconId = 0 },
                    new WfNodesTemplate() { InternalNum = 2, Name = "SecondNode", Description = "Second node description", Terminating = false, IconId = 0 },
                    new WfNodesTemplate() { InternalNum = 3, Name = "ThirdNode", Description = "Third node description", Terminating = true, IconId = 0 }
                };
                var edges = new List<WfEdgesTemplate>()
                {
                    new WfEdgesTemplate(){ Name = "FromFirstToSecond", InternalNum = 1, NodeFrom = 1, NodeTo = 2 },
                    new WfEdgesTemplate(){ Name = "FromSecondToThird", InternalNum = 2, NodeFrom = 2, NodeTo = 3 },
                    new WfEdgesTemplate(){ Name = "FromSecondToFirst", InternalNum = 3, NodeFrom = 2, NodeTo = 1 },
                    new WfEdgesTemplate(){ Name = "FromFirstToThird",  InternalNum = 4, NodeFrom = 1, NodeTo = 3 }
                };

                var long_res = await repo.CreateTemplate("TestTemplate", "Test Template Description", companyId, nodes, edges);
                */

                Assert.True(long_res > 0);

                var query = _context.WfdefinitionsTempls
                     .Include(x => x.WfnodesTempls)
                        .ThenInclude(y => y.WfedgesTemplNodeFromNavigations)
                     .Include(x => x.WfnodesTempls)
                        .ThenInclude(node => node.WfedgesTemplNodeToNavigations)
                     .AsQueryable();

                var found = await query.FirstOrDefaultAsync(x => x.Id == long_res);

                //var found = await _context.WfdefinitionsTempls.FirstOrDefaultAsync(x => x.Id == long_res);
                Assert.NotNull(found);
                found.Name = "TestModified";
                found.Description = "TestTestModofied";

                //var edges = new List<WfNodesTemplate>();
                var edges = found.WfnodesTempls.SelectMany(b => b.WfedgesTemplNodeFromNavigations)
                          .Concat(found.WfnodesTempls.SelectMany(b => b.WfedgesTemplNodeToNavigations))
                           .GroupBy(a => a.Id) // ���������� �� id
                            .Select(g => g.First()) // ����� ������ ������� �� 
                          .ToList();

                found.WfnodesTempls.Add(new WfNodesTemplate() { InternalNum = 4, Name = "FourthNode", Description = "Fourth node description", Terminating = true, IconId = 0 });
                edges.Add(new WfEdgesTemplate() { Name = "FromFirstToFourth", InternalNum = 5, NodeFrom = 1, NodeTo = 4 });
                var bool_res = await repo.UpdateTemplate(Guid.Empty, found.Id, found.Name, found.Description, found.CompanyId, found.WfnodesTempls.ToList(), edges);
                Assert.True(bool_res);
                Assert.True(_context.WfnodesTempls.Where(x => x.DefinitionId == long_res).Count() == 4);


                var mov_val = await _context.WfdefinitionsTempls.Where(x => x.Name == found.Name && x.Description == found.Description).ToListAsync();
                Assert.True(mov_val.Any());
                bool_res = await repo.DeleteTemplate(Guid.Empty, found.Id);
                Assert.True(bool_res);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.Null(exception);
        }

        [Fact]
        public async void TasksRepo_CanCRUD()
        {
            // Arrange
            var taskNewName = "Modified name";
            var taskNewDescription = "Modified description";

            var historyRepo = GetMockHistoryRepository();
            var repo = new TasksRepository(_context.Configuration, historyRepo.Object);
            var urgRepo = new UrgenciesRepository(_context.Configuration, historyRepo.Object);
            var areaRepo = new ProjectAreaRepository(_context.Configuration, historyRepo.Object);
            Exception? exception = null;
            try
            {
                var companyId = await createTestCompanyAsync();
                var testTemplateId = await createTestTemplate(companyId);
                var projectId = await createTestProjectAsync(companyId);
                var urgencyId = await urgRepo.CreateUrgency(Guid.Empty, "Test", "TestTest");
                var newUrgencyId = await urgRepo.CreateUrgency(Guid.Empty, "Modified Test", "Modified TestTest");

                var areaId = await areaRepo.CreateProjectArea(Guid.Empty, new ProjectArea() { Name = "Test", Description = "Test area", ProjectId = projectId });
                // Create task
                TasksServiceTasks.Task taskToCreate = new TasksServiceTasks.Task()
                {
                    Name = "TestTask"
                    ,
                    Description = "Test Task"
                    ,
                    CreatorId = Guid.Empty
                    ,
                    DeadlineDate = DateTime.Now.AddDays(1).ToUniversalTime()
                    ,
                    TemplateId = testTemplateId
                    ,
                    Urgency = urgencyId
                    ,
                    CompanyId = companyId
                    ,
                    ProjectId = projectId
                    ,
                    AreaId = areaId
                    ,
                    CurrentNodeId = 0
                };
                // Create
                var taskId = await repo.CreateTask(Guid.Empty, taskToCreate);
                Assert.True(taskId != 0);
                // Get list
                var tasksList = await repo.GetTasksList(Guid.Empty, companyId, projectId, areaId);
                Assert.True(tasksList.Any());
                // Modify description
                var suscess = await repo.ModifyTaskDescription(Guid.Empty, taskId, taskNewDescription);
                Assert.True(suscess);
                suscess = await repo.ModifyTaskName(Guid.Empty, taskId, taskNewName);
                Assert.True(suscess);
                suscess = await repo.ModifyTaskUrgency(Guid.Empty, taskId, newUrgencyId);
                Assert.True(suscess);


                // Get full task info
                var task = await repo.GetTask(taskId);
                Assert.NotNull(task.task);
                Assert.NotNull(task.nodes);
                Assert.NotNull(task.edges);
                Assert.True(taskNewDescription == task.task.Description);
                Assert.True(taskNewName == task.task.Name);
                Assert.True(newUrgencyId == task.task.Urgency);

                Assert.True(task.nodes.Count != 0);
                Assert.True(task.edges.Count != 0);

                var foundNode = await repo.GetNode(task.nodes[0].Id);
                Assert.True(foundNode.Id == task.nodes[0].Id);
                Assert.True(foundNode.TaskEdgeNodeFromNavigations.Count == task.nodes[0].TaskEdgeNodeFromNavigations.Count);

                var incomingTransitions = await repo.GetToNodeTransitions(foundNode.Id);
                Assert.NotNull(incomingTransitions);
                Assert.True(incomingTransitions.Count != 0);
                var outcomingTransitions = await repo.GetFromNodeTransitions(foundNode.Id);
                Assert.NotNull(outcomingTransitions);
                Assert.True(outcomingTransitions.Count != 0);

                suscess = await repo.ModifyTaskState(Guid.Empty, taskId, foundNode.Id);
                Assert.True(suscess);

                var afterTomorowDate = DateTime.Now.AddDays(2).ToUniversalTime();
                suscess = await repo.ModifyTaskNodeDeadline(Guid.Empty, taskId, foundNode.Id, afterTomorowDate);
                Assert.True(suscess);
                var doers = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() };
                suscess = await repo.AppointNodeDoers(Guid.Empty, foundNode.Id, doers);
                Assert.True(suscess);

                var foundNodeMod = await repo.GetNode(task.nodes[0].Id);
                Assert.NotNull(foundNodeMod);
                Assert.True(foundNodeMod.TaskDoers.Count() == 2);
                var modTask = await repo.GetTask(taskId);
                Assert.True(modTask.task.DeadlineDate?.Date == afterTomorowDate.Date);

            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.Null(exception);
        }

    }
}