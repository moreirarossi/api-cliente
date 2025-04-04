using Application.Commands;
using Application.Querys;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Rommanel.Application.Mapping;
using Rommanel.Application.Validations;
using Rommanel.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
{
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateClienteCommand).Assembly));
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateClienteCommand).Assembly));
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DeleteClienteCommand).Assembly));
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetClienteByIdQuery).Assembly));
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetClienteQuery).Assembly));

    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStrings")));

    services.AddAutoMapper(typeof(ClienteProfile).Assembly);
    services.AddFluentValidationAutoValidation();
    services.AddValidatorsFromAssemblyContaining<ClienteValidator<CreateClienteCommand>>();
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(ClienteProfile).Assembly);

// Adicionar serviços de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevServer", policy =>
    {
        policy.WithOrigins("*")  // Permitir o endereço do seu frontend
            .AllowAnyHeader()                   // Permitir qualquer cabeçalho
            .AllowAnyMethod();                   // Permitir qualquer método
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAngularDevServer");  // Adicione essa linha para habilitar o CORS

app.UseRouting();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
}

app.Run();
