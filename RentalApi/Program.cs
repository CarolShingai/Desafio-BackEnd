using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Serviços necessários para o Swagger
builder.Services.AddEndpointsApiExplorer(); // entrada
builder.Services.AddSwaggerGen(); 

var app = builder.Build();

// Ativar o Swagger apenas em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// var meusDados = new List<string> { "Batata", "Cenoura", "Chocolate" };

//motos: ano, modelos, placa(tipo unico) 
// tabelas separadas entregador | admin


// ARQ hexagonal
// -> recebe as requisições HTTP entrada post | delete | get
// saida banco de dados | mensageria

// Get -> Interface/IProduto -> ObterProduto
// Post -> Interface/IProduto -> AdicionarProduto
// Delete -> Interface/IProduto -> RemoverProduto

// app.MapGet("/meusdados", () => {
//     return Results.Ok(meusDados);
// });

// // Post
// app.MapPost("/meusdados", (string novoDado) => {
//     meusDados.Add(novoDado);
//     return Results.Created($"/meusdados/{novoDado}", novoDado);
// });

// app.MapDelete("/meusdados/{index}", (int index) => {
//     if (index < 0 || index >= meusDados.Count)
//     {
//         return Results.NotFound();
//     }
//     meusDados.RemoveAt(index);
//     return Results.NoContent();
// });

app.Run();
