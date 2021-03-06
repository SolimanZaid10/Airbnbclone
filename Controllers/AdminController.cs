using Airbnb.Models;
using Airbnb.Models.PropertySubModels;
using Airbnb.Models.SiteSettings;
using Airbnb.Services;
using Airbnb.ViewModels;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Airbnb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        readonly IAdminServices _db;
        private readonly RoleManager<IdentityRole> roleManager;
        IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<AppUser> userManager;

        public AdminController(IAdminServices db, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            this.roleManager = roleManager;
            this.userManager = userManager;
            webHostEnvironment = hostEnvironment;

        }

        public IActionResult CreateRole()
        {
            return PartialView("Views/Shared/CreateRoleViewPartial.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole { Name = model.RoleName };
                var result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard","Admin");
                }
                foreach (var Error in result.Errors)
                {
                    ModelState.AddModelError("", Error.Description);
                }
            }
            return PartialView("Views/Shared/CreateRoleViewPartial.cshtml",model);
        }
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return PartialView("Views/Shared/AllRolesPartialView.cshtml", roles);
        }
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Dashboard", "Admin");
            }
            else
                ModelState.AddModelError("", "No role found");
                return PartialView("Views/Shared/AllRolesPartialView.cshtml", roleManager.Roles);
        }

        public IActionResult AllAdmins()
        {
            var users = _db.AllUsers();
            List<AppUser> Admins = new List<AppUser>();
            foreach(var user in users)
            {
                bool result =  userManager.IsInRoleAsync(user, "Admin").Result;
                if (result == true)
                {
                    Admins.Add(user);
                }
            }
            return PartialView("Views/Shared/AllUserPartialView.cshtml", Admins);
        }

        //Amenities
        public IActionResult AllAmenities()
        {
            return PartialView("Views/Shared/AmenitiesPartialView.cshtml", _db.Amenities());
        }
        [HttpGet]
        public IActionResult AddAmenity()
        {
            return View("Views/Shared/AddNewAmenityPartialView.cshtml");
        }
        [HttpPost]
        public IActionResult AddAmenity(Amenity amenity)
        {
            if (ModelState.IsValid)
            {
                var Amenity = new Amenity
                {
                    Icon = amenity.Icon,
                    Description = amenity.Description,
                    Name = amenity.Name,
                    Type = amenity.Type
                };
                _db.AddAmenity(Amenity);
                return RedirectToAction("Dashboard", "Admin");
            }
            
                return View("Views/Shared/AddNewAmenityPartialView.cshtml",amenity);
        }
        public IActionResult deleteAmenity(int id)
        {
            _db.DeleteAmenity(id);
            return RedirectToAction("Dashboard", "Admin");
        }
        
        //Categories
        public IActionResult AllCategories()
        {
            return PartialView("Views/Shared/CategoriesPartialView.cshtml", _db.Categories());
        }
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View("Views/Shared/AddNewCategoryPartialView.cshtml");
        }
        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                var Category = new Category
                {
                   Name=category.Name,
                   Description=category.Description,
                };
                _db.AddCategory(Category);
                return RedirectToAction("Dashboard", "Admin");
            }

            return View("Views/Shared/AddNewCategoryPartialView.cshtml", category);
        }
        public IActionResult deleteCategory(int id)
        {
            _db.DeleteCategory(id);
            return RedirectToAction("Dashboard", "Admin");
        }

        //house rules
        public IActionResult AllHouseRules()
        {
            return PartialView("Views/Shared/HouseRulesPartialView.cshtml", _db.HouseRules());
        }
        [HttpGet]
        public IActionResult AddHouseRule()
        {
            return View("Views/Shared/AddNewHouseRulePartialView.cshtml");
        }
        [HttpPost]
        public IActionResult AddHouseRule(Category category)
        {
            if (ModelState.IsValid)
            {
                var HouseRule = new HouseRule
                {
                    Name = category.Name,
                };
                _db.AddHouseRules(HouseRule);
                return RedirectToAction("Dashboard", "Admin");
            }
            return View("Views/Shared/AddNewHouseRulePartialView.cshtml", category);
        }
        public IActionResult deleteHouseRule(int id)
        {
            _db.DeleteHouseRules(id);
            return RedirectToAction("Dashboard", "Admin");
        }

        //place type
        public IActionResult AllPlaceTypes()
        {
            return PartialView("Views/Shared/PlaceTypePartialView.cshtml", _db.GuestPlaceTypes());
        }
        [HttpGet]
        public IActionResult AddPlaceType()
        {
            return View("Views/Shared/AddNewPlaceTypePartialView.cshtml");
        }
        [HttpPost]
        public IActionResult AddPlaceType(GuestPlaceType guestPlaceType)
        {
            if (ModelState.IsValid)
            {
                var GuestPlaceType = new GuestPlaceType
                {
                    Name = guestPlaceType.Name,
                    Description = guestPlaceType.Description
                };
                _db.AddGuestPlaceType(GuestPlaceType);
                return RedirectToAction("Dashboard", "Admin");
            }
            return View("Views/Shared/AddNewPlaceTypePartialView.cshtml", guestPlaceType);
        }
        public IActionResult deletePlaceType(int id)
        {
            _db.DeleteGuestPlaceType(id);
            return RedirectToAction("Dashboard", "Admin");
        }

        // Spaces
        public IActionResult AllSpaces()
        {
            return PartialView("Views/Shared/SpacesPartialView.cshtml", _db.Spaces());
        }
        [HttpGet]
        public IActionResult AddSpace()
        {
            return View("Views/Shared/AddNewSpacePartialView.cshtml");
        }
        [HttpPost]
        public IActionResult AddSpace(Space space)
        {
            if (ModelState.IsValid)
            {
                var Space = new Space
                {
                    Name = space.Name,
                };
                _db.AddSpace(Space);
                return RedirectToAction("Dashboard", "Admin");
            }
            return View("Views/Shared/AddNewSpacePartialView.cshtml", space);
        }
        public IActionResult deleteSpace(int id)
        {
            _db.DeleteSpace(id);
            return RedirectToAction("Dashboard", "Admin");
        }




        //[HttpGet]
        //public IActionResult ResetPassword()
        //{
        //   var userId = userManager.GetUserId(HttpContext.User);
        //   var user = _db.AllUsers().Single(u => u.Id == userId);
        //    ViewBag.userToken = user.PasswordHash;
        //    ViewBag.emial = user.Email;
        //    return PartialView("Views/Shared/ResetPasswordPartialView.cshtml");

        //}
        //[HttpPost]
        //public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await userManager.FindByEmailAsync(model.Email);
        //        if (user != null)
        //        {
        //            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
        //            if (result.Succeeded)
        //            {
        //                return View("Home", "Index");
        //            }
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError("some thing error", error.Description);
        //            }
        //            return View(model);
        //        }
        //        return View("Home", "Index");
        //    }
        //    return View(model);
        //}






        public IActionResult Dashboard()
        {
            ViewBag.allUser = _db.AllUsers().Count;
            ViewBag.UserLastDay = _db.UserInLast24Hours().Count;
            ViewBag.UserLastMonth = _db.UserInLastMonth().Count;
            ViewBag.allProperty = _db.Allproperties().Count;//accepted property
            ViewBag.PropertyLastDay = _db.Newproperties().Count;//waiting property
            ViewBag.PropertyLastMonth = _db.PropertiesInLast30Days().Count;

            ViewBag.AcceptedReservations = _db.AcceptedReservations().Count;//accepted property
            ViewBag.WaitingReservations = _db.WaitingReservations().Count;//waiting property
            ViewBag.ReservationsLastMonth = _db.ReservationsInLast30Days().Count;
            return View();
        }
        public IActionResult AllUsers()
        {
            return PartialView("Views/Shared/AllUserPartialView.cshtml", _db.AllUsers());
        }
        public IActionResult UserDetails(string id)
        {
            var user = _db.GetUser(id);
            return PartialView("Views/Shared/UserDetailsPartialView.cshtml",user);
        }

        public IActionResult AllProperties()
        {
            return PartialView("Views/Shared/AllPropertiesPartialView.cshtml", _db.Allproperties());
        }
        public IActionResult NewProperties()
        {
            return PartialView("Views/Shared/NewPropertiesPartialView.cshtml", _db.Newproperties());
        }
        public IActionResult AllResevations()
        {
            return PartialView("Views/Shared/AllReservationsPartialView.cshtml", _db.Allreservations());
        }
        public IActionResult AcceptNewProperty(int id)
        {
            _db.AcceptNewProperty(id);
            return RedirectToAction("Dashboard", "Admin");
        }
        public IActionResult UsersExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var users = _db.AllUsers();
                var worksheet = workbook.Worksheets.Add("Users");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "FirstName";
                worksheet.Cell(currentRow, 3).Value = "LastName";
                worksheet.Cell(currentRow, 4).Value = "Email";
                worksheet.Cell(currentRow, 5).Value = "PhoneNumber";
                worksheet.Cell(currentRow, 6).Value = "JoinDate";

                foreach (var user in users)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = user.Id;
                    worksheet.Cell(currentRow, 2).Value = user.FirstName;
                    worksheet.Cell(currentRow, 3).Value = user.LastName;
                    worksheet.Cell(currentRow, 4).Value = user.Email;
                    worksheet.Cell(currentRow, 5).Value = user.PhoneNumber;
                    worksheet.Cell(currentRow, 6).Value = user.JoinDate.ToString("MM/dd/yyyy");

                }


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "users.xlsx");
                }
            }
        }
        public IActionResult PropertyExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var props = _db.Allproperties();
                var worksheet = workbook.Worksheets.Add("Users");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "Title";
                worksheet.Cell(currentRow, 3).Value = "City";
                worksheet.Cell(currentRow, 4).Value = "Price";
                worksheet.Cell(currentRow, 5).Value = "MaxStay";
                worksheet.Cell(currentRow, 6).Value = "MinStay";
                worksheet.Cell(currentRow, 7).Value = "JoinDate";
                worksheet.Cell(currentRow, 7).Value = "EndDate";


                foreach (var prop in props)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = prop.Id;
                    worksheet.Cell(currentRow, 2).Value = prop.Title;
                    worksheet.Cell(currentRow, 3).Value = prop.City;
                    worksheet.Cell(currentRow, 4).Value = prop.Price;
                    worksheet.Cell(currentRow, 5).Value = prop.MaxStay;
                    worksheet.Cell(currentRow, 6).Value = prop.MinStay;
                    worksheet.Cell(currentRow, 7).Value = prop.Date.ToString("MM/dd/yyyy");
                }


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "properties.xlsx");
                }
            }
        }
        [HttpPost]
        public IActionResult FindUser(string name)
        {
            return PartialView("Views/Shared/AllUserPartialView.cshtml", _db.FindUserByName(name));
        }
        [HttpPost]
        public IActionResult FindProp(string Title)//by city name
        {
            return PartialView("Views/Shared/AllPropertiesPartialView.cshtml", _db.FindPeopByTitle(Title));
        }
        public IActionResult DeleteUser(string id)
        {
            _db.DeleteUser(id);
            return RedirectToAction("Dashboard", "Admin");
        }
        public IActionResult DeleteProperty(int id)
        {
            _db.DeleteProp(id);
            return RedirectToAction("Dashboard", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string id)
        {
            ViewBag.roleId = id;
            var role = await roleManager.FindByIdAsync(id);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with id = {id} cannot be found";
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();

            foreach(var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
                var x = model;

            }
            return PartialView("Views/Shared/UsersRolePartialView.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> model,string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErroeMessage = $"Role with id = {id} cannot be found";
            }
            for(int i=0;i<model.Count;i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if(model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }else if(!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i<(model.Count-1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                }
               
            }
            return RedirectToAction("Dashboard", "Admin");
        }



        [HttpGet]
        public IActionResult ChangeLogo()
        {
            var logo = _db.getLogo();
            ViewBag.logo = logo?.LogoUrl;

            return PartialView("Views/Shared/ChangeLogoPartialView.cshtml");
        }
        [HttpPost]
        public IActionResult ChangeLogo(LogoViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);
                var logo = new Logo
                {
                    LogoUrl = uniqueFileName,
                };
                _db.AddLogo(logo);
            }
            return RedirectToAction("Dashboard", "Admin");
        }

        private string UploadedFile(LogoViewModel model)
        {
            string uniqueFileName = null;

            if (model.LogoUrl != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.LogoUrl.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.LogoUrl.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }



    }
}
