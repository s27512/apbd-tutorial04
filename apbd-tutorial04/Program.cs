using apbd_tutorial04.database;
using apbd_tutorial04.models;
using Microsoft.AspNetCore.Mvc;
//bogus check, check dto
var builder = WebApplication.CreateBuilder(args);
var animals = MyDatabase.Animals;
var visits = MyDatabase.Visits;

MyDatabase.Animals.Add(new Animal { Id = 1, Name = "Dog", Category = "Mammal", Weight = 25.5f, FurColor = "Brown" });
MyDatabase.Animals.Add(new Animal { Id = 2, Name = "Cat", Category = "Mammal", Weight = 10.2f, FurColor = "White" });
MyDatabase.Animals.Add(new Animal { Id = 3, Name = "Parrot", Category = "Bird", Weight = 0.5f, FurColor = "Green" });


MyDatabase.Visits.Add(new Visit { Date = "2024-04-09", Animal = MyDatabase.Animals[0], Description = "Regular checkup", Price = 50.00f });
MyDatabase.Visits.Add(new Visit { Date = "2024-04-10", Animal = MyDatabase.Animals[1], Description = "Vaccination", Price = 30.00f });
MyDatabase.Visits.Add(new Visit { Date = "2024-04-11", Animal = MyDatabase.Animals[2], Description = "Beak trimming", Price = 15.00f });



// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();


app.MapGet("/api/animals", () => Results.Ok(animals))
    .WithName("GetAnimals")
    .WithOpenApi();

app.MapGet("/api/animals/{id:int}", (int id) =>
    {
        var animal = animals.FirstOrDefault(a => a.Id == id);
        return animal == null ? Results.NotFound($"Animal with {id} does not exist!") : Results.Ok(animal);
    })
    .WithName("GetAnimal")
    .WithOpenApi();

app.MapPost("/api/animals", (Animal animal) =>
    {
    animals.Add(animal);
    return Results.StatusCode(StatusCodes.Status201Created);
    })
    .WithName("AddAnimal")
    .WithOpenApi();

app.MapPut("/api/animals/{id:int}", (int id, Animal modifiedAnimal) =>
    {
        var animal = animals.FirstOrDefault(a => a.Id == id);
        if (animal == null)
        {
            return Results.NotFound($"Ahimal with id {id} does not exist!");
        }

        modifiedAnimal.Id = animal.Id;
        animals.Remove(animal);
        animals.Add(modifiedAnimal);
        return Results.NoContent();
    })
    .WithName("UpdateAnimal")
    .WithOpenApi();

app.MapDelete("/api/animals/{id:int}", (int id) =>
    {
        var animal = animals.FirstOrDefault(a => a.Id == id);
        if (animal == null)
        {
            return Results.NotFound($"Animal with id {id} does not exist!");
        }

        animals.Remove(animal);
        return Results.NoContent();
    })
    .WithName("DeleteAnimal")
    .WithOpenApi();


app.MapGet("/api/visits/{id:int}", (int id) =>
    {
        return visits.Where(v => v.Animal.Id == id);
    })
    .WithName("GetVistsByAnimalId")
    .WithOpenApi();


app.MapPost("/api/visits", (Visit visit) =>
    {
        visits.Add(visit);
        return Results.StatusCode(StatusCodes.Status201Created);
    })
    .WithName("AddVisit")
    .WithOpenApi();


app.Run();