using MarcaFacilAPI.DataAccess.Context;
using MarcaFacilAPI.Models;

namespace MarcaFacilAPI.DataAccess
{
    public class PlaceRepository
    {
        private readonly PostgreSqlContext _context;

        public PlaceRepository(PostgreSqlContext context)
        {
            _context = context;
        }

        public List<Place> GetPlaces()
        {
            return _context.Place.ToList();
        }

        public void PostPlace(Place place)
        {
            _context.Place.Add(place);
            _context.SaveChanges();
        }

        public void PutPlace(Place place)
        {
            _context.Place.Update(place);
            _context.SaveChanges();
        }

        public void DeletePlace(Place place)
        {
            _context.Place.Remove(place);
            _context.SaveChanges();
        }

        public Place GetPlaceById(Guid id)
        {
            return _context.Place.FirstOrDefault(t => t.Id == id);
        }

        //public IEnumerable<Place> GetPlacesByPage(int page, int pageSize)
        //{
        //    List<Place> places = _context.Place
        //        .OrderBy(o => o.Name)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    return places;
        //}
    }
}
