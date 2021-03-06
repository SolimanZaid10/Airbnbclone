using Airbnb.Data;
using Airbnb.Models;
using Airbnb.Models.PropertySubModels;
using Airbnb.Models.SiteSettings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airbnb.Services
{
    public interface IAdminServices
    {
        //users
        public List<AppUser> AllUsers();
        public List<AppUser> UserInLast24Hours();
        public List<AppUser> UserInLastMonth();
        public AppUser GetUser(string userId);
        public void DeleteUser(string userId);
        public List<AppUser> FindUserByName(string Name);


        //property
        public List<Property> Allproperties();
        public List<Property> Newproperties();
        public List<Property> PropertiesInLast30Days();
        public Property GetProperity(int PropId);
        public void DeleteProp(int PropId);
        public List<Property> FindPeopByTitle(string Title);
        public List<Property> FindPeopByCityName(string name);

        public void AcceptNewProperty(int id);

        // reservation
        public List<Reservation> Allreservations();
        public List<Reservation> AcceptedReservations();
        public List<Reservation> WaitingReservations();
        public List<Reservation> ReservationsInLast30Days();
        public void DeleteReservation(int ResId);
        public Reservation GetReservation(int ResId);

        //Amenity
        public List<Amenity> Amenities();
        public void AddAmenity(Amenity amenity);
        public void DeleteAmenity(int id);

        //Categories
        public List<Category> Categories();
        public void AddCategory(Category category);
        public void DeleteCategory(int id);

        //house rules
        public List<HouseRule> HouseRules();
        public void AddHouseRules(HouseRule houseRule);
        public void DeleteHouseRules(int id);

        //Guest PlaceType
        public List<GuestPlaceType> GuestPlaceTypes();
        public void AddGuestPlaceType(GuestPlaceType guestPlaceType);
        public void DeleteGuestPlaceType(int id);

        //spaces
        public List<Space> Spaces();
        public void AddSpace(Space space);
        public void DeleteSpace(int id);

        //site settings
        public void AddLogo(Logo logo);
        public Logo getLogo();

    }

    public class AdminServices:IAdminServices
    {
        private readonly ApplicationDbContext _db;
        public AdminServices(ApplicationDbContext db)
        {
            _db = db;
        }


        //operations on user
        public List<AppUser> AllUsers()
        {
            return _db.Users.OrderBy(a=>a.JoinDate).ToList();
        }
        public List<AppUser> UserInLast24Hours()
        {
            return _db.Users.Where(u => u.JoinDate >= DateTime.Now.AddHours(-24)).ToList();
        }
        public List<AppUser> UserInLastMonth()
        {
            return _db.Users.Where(u => u.JoinDate >= DateTime.Now.AddDays(-30)).ToList();
        }
        public AppUser GetUser(string userId)
        {
            return _db.Users.SingleOrDefault(u => u.Id == userId);
        }
        public void DeleteUser(string userId)
        {
            var user = GetUser(userId);
            _db.Users.Remove(user);
            _db.SaveChanges();
        }
        public List<AppUser> FindUserByName(string Name)
        {
            return AllUsers().Where(u => u.FirstName.Contains(Name)).ToList();
        }


        //operations on properties

        public List<Property> FindPeopByCityName(string name)
        {
            if (name != null)
            {
                var properties = _db.Properties.Where(p => p.City.Name == name).ToList();

                return properties;
            }
            return null;
        }

        public List<Property> Allproperties()
        {
            var p = _db.Properties.Where(p=>p.Accepted==true).ToList();
            return p;
        }
        public List<Property> Newproperties()
        {
            var p = _db.Properties.Where(p => p.Accepted == false).ToList();
            return p;
        }
        public List<Property> PropertiesInLast30Days()
        {
            return _db.Properties.Where(P => P.Date >= DateTime.Now.AddDays(-30)).ToList();
        }
        public Property GetProperity(int PropId)
        {
            return _db.Properties.ToList().SingleOrDefault(p=>p.Id == PropId);
        }
        public void AcceptNewProperty(int id)
        {
           var property =  GetProperity(id);
           property.Accepted = true;
           _db.SaveChanges();
        }
        public void DeleteProp (int PropId)
        {
            var Property = GetProperity(PropId);
            _db.Properties.Remove(Property);
            _db.SaveChanges();
        }
        public List<Property> FindPeopByTitle(string Title)
        {
            return _db.Properties.Where(p=>p.Title.Contains(Title)).ToList();
        }

        //operations on reservation
        public List<Reservation> Allreservations()
        {
            var r = _db.Reservations.ToList();
            return r;
        }
        public List<Reservation> AcceptedReservations()
        {
            var r = _db.Reservations.Where(p => p.Accepted == true).ToList();
            return r;
        }
        public List<Reservation> WaitingReservations()
        {
            var r = _db.Reservations.Where(p => p.Accepted == false).ToList();
            return r;
        }
        public List<Reservation> ReservationsInLast30Days()
        {
            return _db.Reservations.Where(P => P.Date >= DateTime.Now.AddDays(-30)).ToList();
        }
        public Reservation GetReservation(int ResId)
        {
            return _db.Reservations.ToList().SingleOrDefault(r => r.Id == ResId);
        }
        public void DeleteReservation(int ResId)
        {
            var Reservation = GetReservation(ResId);
            _db.Reservations.Remove(Reservation);
            _db.SaveChanges();
        }
        

        //Amenity
        public List<Amenity> Amenities()
        {
            return _db.Amenities.ToList();
        }
        public void AddAmenity(Amenity amenity)
        {
            _db.Amenities.Add(amenity);
            _db.SaveChanges();
        }
        public void DeleteAmenity(int id)
        {
            var amenity =_db.Amenities.SingleOrDefault(a => a.Id == id);
            _db.Amenities.Remove(amenity);
            _db.SaveChanges();
        }

        //Category
        public List<Category> Categories()
        {
            return _db.Categories.ToList();
        }
        public void AddCategory(Category category)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
        }
        public void DeleteCategory(int id)
        {
            var category = _db.Categories.SingleOrDefault(a => a.Id == id);
            _db.Categories.Remove(category);
            _db.SaveChanges();
        }

        //House Rules
        public List<HouseRule> HouseRules()
        {
            return _db.HouseRules.ToList();
        }
        public void AddHouseRules(HouseRule houseRule)
        {
            _db.HouseRules.Add(houseRule);
            _db.SaveChanges();
        }
        public void DeleteHouseRules(int id)
        {
            var houseRule = _db.HouseRules.SingleOrDefault(a => a.Id == id);
            _db.HouseRules.Remove(houseRule);
            _db.SaveChanges();
        }

        //Guest PlaceType
        public List<GuestPlaceType> GuestPlaceTypes()
        {
            return _db.GuestPlaceTypes.ToList();
        }
        public void AddGuestPlaceType(GuestPlaceType guestPlaceType)
        {
            _db.GuestPlaceTypes.Add(guestPlaceType);
            _db.SaveChanges();
        }
        public void DeleteGuestPlaceType(int id)
        {
            var guestPlaceType = _db.GuestPlaceTypes.SingleOrDefault(a => a.Id == id);
            _db.GuestPlaceTypes.Remove(guestPlaceType);
            _db.SaveChanges();
        }

        //spaces
        public List<Space> Spaces()
        {
            return _db.Spaces.ToList();
        }
        public void AddSpace(Space space)
        {
            _db.Spaces.Add(space);
            _db.SaveChanges();
        }
        public void DeleteSpace(int id)
        {
            var space = _db.Spaces.SingleOrDefault(a => a.Id == id);
            _db.Spaces.Remove(space);
            _db.SaveChanges();
        }

        //site settings
        public void AddLogo(Logo logo)
        {
            if (logo != null)
            {
                if (_db.SiteLogo.ToList() != null)
                {
                    _db.SiteLogo.ToList().Clear();
                }
                _db.SiteLogo.Add(logo);
                _db.SaveChanges();
            }
        }
        public Logo getLogo()
        {
            
                var logo = _db.SiteLogo.FirstOrDefault();
                return logo;
            
        }


    }
}
