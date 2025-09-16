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
	/// <summary>
	/// Generating basic crud w/o auth
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="IdKey"></typeparam>
	/// <param name="app"></param>
	/// <param name="repository"></param>
	/// <returns></returns>
	public static WebApplication GenerateCRUD<T, IdKey>(this WebApplication app, IFastRepository<T, IdKey> repository)
	{
		app.GenerateGet(repository);
		app.GeneratePost(repository);
		app.GeneratePut(repository);
		app.GenerateDelete(repository);
		app.GenerateGetById(repository);
		return app;
	}
	/// <summary>
	/// Generating basic crud with auth
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="IdKey"></typeparam>
	/// <param name="app"></param>
	/// <param name="repository"></param>
	/// <returns></returns>
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
		var route = app.MapGet($"/{repository.GetType().Name}/{typeof(T).Name}s", () =>
		{
			return repository.GetAll();
		}).WithTags($"/{repository.GetType().Name}");

		return route;
	}

	public static RouteHandlerBuilder GeneratePost<T, IdKey>(this WebApplication app, IFastRepository<T, IdKey> repository)
	{
		var route = app.MapPost($"/{repository.GetType().Name}/{typeof(T).Name}s", (T item) =>
		{
			repository.Insert(item);
			return Results.Created($"/{repository.GetType().Name}/{typeof(T).Name}s", item);
		}).WithTags($"/{repository.GetType().Name}");

		return route;
	}

	public static RouteHandlerBuilder GeneratePut<T, IdKey>(this WebApplication app, IFastRepository<T, IdKey> repository)
	{
		var route = app.MapPut($"/{repository.GetType().Name}/{typeof(T).Name}s/{{id}}", (int id, T item) =>
		{
			repository.Update(item);
			return Results.NoContent();
		}).WithTags($"/{repository.GetType().Name}");

		return route;
	}
	public static RouteHandlerBuilder GenerateDelete<T, IdKey>(this WebApplication app, IFastRepository<T, IdKey> repository)
	{
		var route = app.MapDelete($"/{repository.GetType().Name}/{typeof(T).Name}s/{{id}}", (IdKey id) =>
		{
			repository.Delete(id);
			return Results.NoContent();
		}).WithTags($"/{repository.GetType().Name}");

		return route;
	}

	public static RouteHandlerBuilder GenerateGetById<T, IdKey>(this WebApplication app, IFastRepository<T, IdKey> repository)
	{
		var route = app.MapGet($"/{repository.GetType().Name}/{typeof(T).Name}s/{{id}}", (IdKey id) =>
		{
			return repository.GetById(id);
		}).WithTags($"/{repository.GetType().Name}");

		return route;
	}
}
