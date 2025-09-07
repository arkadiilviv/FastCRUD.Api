using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FastCRUD.Api;

public interface IFastRepository<T, IdKey>
{
	List<T> GetAll();
	void Insert(T item);
	void Update(T item);
	void Delete(IdKey id);
	T GetById(IdKey id);
}
public static class FastCRUD
{
	public static WebApplication GenerateCRUD<T, IdKey>(this WebApplication app, IFastRepository<T, IdKey> repository)
	{
		app.GenerateGet(repository);
		app.GeneratePost(repository);
		app.GeneratePut(repository);
		app.GenerateDelete(repository);
		app.GenerateGetById(repository);
		return app;
	}

	public static WebApplication GenerateCRUDWithAuth<T, IdKey>(this WebApplication app, IFastRepository<T, IdKey> repository, string policy = "")
	{
		if (!string.IsNullOrEmpty(policy))
		{
			app.GeneratePost(repository)
				.RequireAuthorization(policy);
			app.GeneratePut(repository)
				.RequireAuthorization(policy);
			app.GenerateDelete(repository)
				.RequireAuthorization(policy);
			app.GenerateGet(repository)
				.RequireAuthorization(policy);
			app.GenerateGetById(repository)
				.RequireAuthorization(policy);
		} else
		{
			app.GeneratePost(repository)
				.RequireAuthorization();
			app.GeneratePut(repository)
				.RequireAuthorization();
			app.GenerateDelete(repository)
				.RequireAuthorization();
			app.GenerateGet(repository)
				.RequireAuthorization();
			app.GenerateGetById(repository)
				.RequireAuthorization();
		}
		return app;
	}

	public static RouteHandlerBuilder GenerateGet<T, IdKey>(this WebApplication app, IFastRepository<T, IdKey> repository)
	{
		var route = app.MapGet($"/{typeof(T).Name}s", () =>
		{
			return repository.GetAll();
		});

		return route;
	}

	public static RouteHandlerBuilder GeneratePost<T, IdKey>(this WebApplication app, IFastRepository<T, IdKey> repository)
	{
		var route = app.MapPost($"/{typeof(T).Name}s", (T item) =>
		{
			repository.Insert(item);
			return Results.Created($"/{typeof(T).Name}s", item);
		});

		return route;
	}

	public static RouteHandlerBuilder GeneratePut<T, IdKey>(this WebApplication app, IFastRepository<T, IdKey> repository)
	{
		var route = app.MapPut($"/{typeof(T).Name}s/{{id}}", (int id, T item) =>
		{
			repository.Update(item);
			return Results.NoContent();
		});

		return route;
	}
	public static RouteHandlerBuilder GenerateDelete<T, IdKey>(this WebApplication app, IFastRepository<T, IdKey> repository)
	{
		var route = app.MapDelete($"/{typeof(T).Name}s/{{id}}", (IdKey id) =>
		{
			repository.Delete(id);
			return Results.NoContent();
		});

		return route;
	}

	public static RouteHandlerBuilder GenerateGetById<T, IdKey>(this WebApplication app, IFastRepository<T, IdKey> repository)
	{
		var route = app.MapGet($"/{typeof(T).Name}s/{{id}}", (IdKey id) =>
		{
			return repository.GetById(id);
		});

		return route;
	}
}
