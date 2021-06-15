﻿using Airbnb.Data;
using Airbnb.Models;
using Airbnb.Models.PropertySubModels;
using Airbnb.ViewModels;
using Airbnb.ViewModels.Listing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Airbnb.Controllers.Listing
{

    public class ListingController : Controller
    {
        private ApplicationDbContext _db;
        private readonly IWebHostEnvironment hostingEnvironment;

        public ListingController(ApplicationDbContext applicationDbContext,
                                 IWebHostEnvironment hostingEnvironment)
        {
            _db = applicationDbContext;
            this.hostingEnvironment = hostingEnvironment;
        }
       
        public IActionResult New()
        {
            ListingViewModel listingViewModel = new ListingViewModel()
            {
                amenty = _db.Amenities.ToList(),
                Categories = _db.Categories.ToList(),
                GuestPlaceTypes = _db.GuestPlaceTypes.ToList(),
                Spaces=_db.Spaces.ToList(),
                propertyPhotos=_db.PropertyPhoto.ToList(),
                houseRoles = _db.HouseRules.ToList()

            };

            ViewData["Countries"] = _db.Countries;

            return View(listingViewModel);
        }
        [HttpPost]
        public IActionResult New(ListingViewModel listingViewModel)
        {
            var minNights = 0;
            var maxNights = 0;

            if (listingViewModel.MinNights != null)
                minNights = int.Parse(listingViewModel.MinNights.Split(' ')[0]);
            else minNights = 0;
            if (listingViewModel.MaxNights != null)
                maxNights = int.Parse(listingViewModel.MaxNights.Split(' ')[0]);
            else maxNights = 0;
            var name = _db.Categories.FirstOrDefault(x => x.Name == listingViewModel.Categoryname);
            var id = name.Id;
            var guestPlaceType = _db.GuestPlaceTypes.FirstOrDefault(x => x.Name == listingViewModel.GuestPlaceTypeName);
            var PlaceTypeId = guestPlaceType.Id;
            Property NewProperty = new Property();
            NewProperty.CategoryId = id;
            NewProperty.GuestPlaceTypeId = PlaceTypeId;
            NewProperty.NumberOfBedRooms = int.Parse(listingViewModel.NumOfBedrooms);
            NewProperty.NumberOfBeds = listingViewModel.NumOfBeds;
            NewProperty.Zipcode = listingViewModel.ZipCode;
            NewProperty.BuildingNo = listingViewModel.Apt_Suite;
            NewProperty.Street = listingViewModel.Street;
            NewProperty.Title = listingViewModel.Title;
            NewProperty.Description = listingViewModel.Description;
            NewProperty.NumberOfBathrooms = listingViewModel.NumberOfBathRooms;
            NewProperty.MinStay = minNights;
            NewProperty.MaxStay = maxNights;
            NewProperty.Price = listingViewModel.Price;
            NewProperty.Capacity = listingViewModel.NumOfGuests;
            NewProperty.CityId = listingViewModel.CityId;
            NewProperty.Coordinates = new NetTopologySuite.Geometries.Point(listingViewModel.Lon, listingViewModel.Lat) { SRID = 4326 };
            _db.Add(NewProperty);
            _db.SaveChanges();
            string UniqueFileName = null;
            if (listingViewModel.Images != null)
            {
                for(var i = 0; i < listingViewModel.Images.Count; i++)
                {
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Images");
                    UniqueFileName = Guid.NewGuid().ToString() + "_" + listingViewModel.Images[i].FileName;
                    string FilePath = Path.Combine(uploadsFolder, UniqueFileName);
                    listingViewModel.Images[i].CopyTo(new FileStream(FilePath, FileMode.Create));
                    PropertyPhoto photo = new PropertyPhoto();
                    photo.PropertyId = NewProperty.Id;
                    photo.Url = UniqueFileName;
                    _db.Add(photo);
                    _db.SaveChanges();
                }
               
            }
            //amenty
            if (listingViewModel.IsChecked != null)
            {
                foreach (var item in listingViewModel.IsChecked)
                {
                    PropertyAmenity amenity = new PropertyAmenity();
                    amenity.AmenityId = item;
                    amenity.PropertyId = NewProperty.Id;
                    _db.Add(amenity);
                    _db.SaveChanges();
                }
            }
            //SpacesCanGuestUse
            if (listingViewModel.IsSpacesChecked != null)
            {
                foreach (var item in listingViewModel.IsSpacesChecked)
                {
                    PropertySpace propertySpace = new PropertySpace();
                    propertySpace.PropertyId = NewProperty.Id;
                    propertySpace.SpaceId = item;
                    _db.Add(propertySpace);
                    _db.SaveChanges();
                }
            }
            //HouseRoles
            if (listingViewModel.IsHouseRoleChecked != null)
            {
                foreach (var item in listingViewModel.IsHouseRoleChecked)
                {
                    PropertyHouseRule propertyHouseRule = new PropertyHouseRule();
                    propertyHouseRule.PropertyId = NewProperty.Id;
                    propertyHouseRule.HouseRuleId = item;
                    _db.Add(propertyHouseRule);
                    _db.SaveChanges();
                }
            }
            return RedirectToAction(nameof(New));
        }

        public IActionResult States(int id)
        {
            return PartialView("/Views/Listing/Partial/StatesViewPartial.cshtml", _db.Countries.SingleOrDefault(c => c.Id == id).States);
        }

        public IActionResult Cities(int id)
        {
            return PartialView("/Views/Listing/Partial/CitiesViewPartial.cshtml", _db.States.SingleOrDefault(c => c.Id == id).Cities);
        }

        public IActionResult CityLocation(int id)
        {
            var coordinates = _db.Cities.SingleOrDefault(c => c.Id == id).Coordinates;

            var coords = new
            {
                lat = coordinates.Coordinate.Y,
                lon = coordinates.Coordinate.X,
            };

            return Json(coords);
        }
    }
}
