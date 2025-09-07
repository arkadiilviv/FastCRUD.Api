using FastCRUD.Api;
namespace FasterAPI
{
	public class HumanModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Age { get; set; }
		public string Occupation { get; set; }
	}
	public class HumanRepositoryWOAuth : HumanRepository { }
	public class HumanRepository : IFastRepository<HumanModel, int>
	{
		private readonly List<HumanModel> _humans = new()
		{
			new HumanModel { Id = 1, Name = "Alice", Age = 30, Occupation = "Engineer" },
			new HumanModel { Id = 2, Name = "Bob", Age = 25, Occupation = "Designer" },
			new HumanModel { Id = 3, Name = "Charlie", Age = 35, Occupation = "Teacher" }
		};
		public List<HumanModel> GetAll()
		{
			return _humans;
		}
		public void Insert(HumanModel item)
		{
			_humans.Add(item);
		}
		public void Update(HumanModel item)
		{
			var index = _humans.FindIndex(h => h.Id == item.Id);
			if (index != -1)
			{
				_humans[index] = item;
			}
		}
		public HumanModel GetById(int id)
		{
			return _humans.FirstOrDefault(x => x.Id == id, new HumanModel());
		}

		public void Delete(int id)
		{
			_humans.Remove(_humans.FirstOrDefault(x => x.Id == id));
		}
	}
}
