using Apteryx.MongoDB.Driver.Extend;
using Apteryx.Routing.Role.Authority;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerExamples();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("zy1.0", new OpenApiInfo
    {
        Title = "光圈视界 RESTful API 资源服务",
        Version = "v1.0",
        Contact = new OpenApiContact
        {
            Name = "Apteryx Developer",
            Email = "wyspaces@outlook.com"
        }
    });
});
builder.Services.AddApteryxAuthority(new ApteryxConfig()
{
    IsSecurityToken = true,
    AESConfig = new AES256Setting()
    {
        Key = "fND+T_yo@wc!$uEEw!mDjqN9wYcvuO2I",
        IV = "70w@Ox_nF*%0G*KE"
    },
    TokenConfig = new TokenSetting()
    {
        Audience = "www.apteryx.com.cn",
        Expires = 7200,
        Key = "102esdjflskjdflkjsf29384023iksdjflk",
        Issuer = "apteryx"
    },
    MongoDBOptions = new MongoDBOptions()
    {
        //更改成你的MongoDb连接
        ConnectionString = "mongodb://root:pwd@ip:port/apteryx_authority?authSource=admin"
    }
});

builder.Services.AddCors(m => m.AddPolicy("any", a => a
            .SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "光圈视界 Restful APIs Documentation";
        c.SwaggerEndpoint("/swagger/zy1.0/swagger.json", "光圈视界 RESTful API 资源服务");
        c.RoutePrefix = "swagger";
        c.DefaultModelExpandDepth(2);
        c.DefaultModelRendering(ModelRendering.Example);
        c.DefaultModelsExpandDepth(1);
        c.DefaultModelExpandDepth(1);
        c.DisplayOperationId();
        c.DisplayRequestDuration();
        c.DocExpansion(DocExpansion.None);//文档展开形式
        c.EnableDeepLinking();
        c.EnableFilter();
        c.MaxDisplayedTags(20);
        //                c.ShowExtensions();
        c.EnableValidator();
    });
    app.UseApteryxSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("any");
app.MapControllers();

app.Run();
