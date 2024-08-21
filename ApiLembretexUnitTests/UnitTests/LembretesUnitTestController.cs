using Moq;
using APICalendario.Context;
using APICalendario.DTOs.Mappings;
using APICalendario.Repositories;
using APICalendario.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiLembretexUnitTests.UnitTests; 

public class LembretesUnitTestController
{
    public IUnitOfWork repositorycreate;
    public IUnitOfWork repository;
    public IMapper mapper;
    public ICriaLembretesService criaLembretesService;
    public static DbContextOptions<AppDbContext> dbContextOptions { get; }

    public static string connectionString =
        "Server=localhost;Database=clone;Uid=root;Pwd=root";

    static LembretesUnitTestController()
    {
        dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
    }

    public LembretesUnitTestController()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new LembreteDTOMappingProfile());
        });

        
        var context = new AppDbContext(dbContextOptions);       
        
        mapper = config.CreateMapper();
        repository = new UnitOfWork(context);
        criaLembretesService = new CriaLembretesService();
        repositorycreate = new UnitOfWork(context, criaLembretesService);

    } 
}
