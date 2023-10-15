using MarcaFacilAPI.DataAccess.Context;
using MarcaFacilAPI.Models;

namespace MarcaFacilAPI.DataAccess
{
    public class ItemRepository
    {
        private readonly PostgreSqlContext _context;

        public ItemRepository(PostgreSqlContext context)
        {
            _context = context;
        }

        public List<Item> GetItems()
        {
            return _context.Item.ToList();
        }

        public void PostItem(Item item)
        {
            _context.Item.Add(item);
            _context.SaveChanges();
        }

        public void PutItem(Item item)
        {
            _context.Item.Update(item);
            _context.SaveChanges();
        }

        public void DeleteItem(Item item)
        {
            _context.Item.Remove(item);
            _context.SaveChanges();
        }

        public Item GetItemById(Guid id)
        {
            return _context.Item.FirstOrDefault(t => t.Id == id);
        }

        //public IEnumerable<Item> GetItemsByPage(int page, int pageSize)
        //{
        //    List<Item> items = _context.Item
        //        .OrderBy(o => o.Name)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    return items;
        //}
    }
}
