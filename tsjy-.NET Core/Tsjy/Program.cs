using Faoem.Common.Extensions;
using Faoem.Facility.Extensions;
using Faoem.FacilityStatus.Extensions;
using Faoem.ModbusTcpConnector.Extensions;
using Faoem.Mqtt.Extensions;
using Faoem.OpcUaConnector.Extensions;
using Faoem.S7Connector.Extensions;
using Faoem.Shift.Extensions;
using Faoem.Variable.Extensions;
using Tsjy.Extensions;
using Microsoft.EntityFrameworkCore; // 添加 EF Core 引用
using Tsjy.DbContexts; // 添加 AppDbContext 命名空间

var builder = WebApplication.CreateBuilder(args);

if(!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("Settings/appsettings.json", optional: true, reloadOnChange: true);
}

builder.Services.AddCommon(builder.Configuration);

//// 注册 AppDbContext，使用 SQLite
//builder.Services.AddDbContext<SqliteTsjyDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnectionString")));

builder.Services.AddControllers().Configure();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMqtt(builder.Configuration);
builder.Services.AddVariable(builder.Configuration);
builder.Services.AddS7Connector(builder.Configuration);
builder.Services.AddOpcUaConnector(builder.Configuration);
builder.Services.AddModbusTcpConnector(builder.Configuration);
builder.Services.AddShift(builder.Configuration);
builder.Services.AddFacility(builder.Configuration);
builder.Services.AddFacilityStatus(builder.Configuration);
builder.Services.AddTsjy(builder.Configuration);


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.ConfigureCommon();
app.ConfigureVariable();
app.ConfigureMqtt();
app.ConfigureOpcUaConnector();
app.ConfigureShift();
app.ConfigureFacility();
app.ConfigureFacilityStatus();
app.ConfigureTsjy();

app.MapControllers();

app.Run();