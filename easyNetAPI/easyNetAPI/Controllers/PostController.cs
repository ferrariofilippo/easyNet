﻿using Microsoft.AspNetCore.Mvc;
using System;
using easyNetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using easyNetAPI.Utility;
using easyNetAPI.Data.Repository.IRepository;
using easyNetAPI.Data;
using easyNetAPI.Models.UpsertModels;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using easyNetAPI.Models.ModelVM;
using Microsoft.AspNetCore.Identity;

namespace easyNetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private const int DEFAULT_RANDOM_POST_AMOUNT = 7;

    private readonly ILogger<PostController> _logger;
    private readonly IWebHostEnvironment _hostEnvironment;
    public IUnitOfWork _unitOfWork;
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public PostController(ILogger<PostController> logger, IUnitOfWork unitOfWork, AppDbContext db, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _db = db;
        _hostEnvironment = hostEnvironment;
        _userManager = userManager;
    }

    [HttpGet("GetPostsOfUser"), Authorize(Roles = $"{SD.ROLE_EMPLOYEE},{SD.ROLE_COMPANY_ADMIN},{SD.ROLE_USER},{SD.ROLE_MODERATOR}")]
    public async Task<IEnumerable<Post>> GetAsync(string username)
    {
        var posts = (await _unitOfWork.Post.GetAllAsync()).Where(post => post.Username == username);
        return posts;
    }

    //get di tutti i post
    [HttpGet("GetAllPosts"), Authorize(Roles = $"{SD.ROLE_EMPLOYEE},{SD.ROLE_COMPANY_ADMIN},{SD.ROLE_USER},{SD.ROLE_MODERATOR}")]
    public async Task<IEnumerable<Post>?> GetAllAsync()
    {
        var posts = await _unitOfWork.Post.GetAllAsync();
        return posts;
    }

    //get di un post in base all id
    [HttpGet("GetPostById"), Authorize(Roles = $"{SD.ROLE_EMPLOYEE},{SD.ROLE_COMPANY_ADMIN},{SD.ROLE_USER},{SD.ROLE_MODERATOR}")]
    public async Task<Post?> GetByIdAsync(int postId)
    {
        var post = await _unitOfWork.Post.GetFirstOrDefault(postId);
        return post;
    }

    [HttpGet("GetPostsOfFollowed"), Authorize(Roles = $"{SD.ROLE_EMPLOYEE},{SD.ROLE_COMPANY_ADMIN},{SD.ROLE_USER}")]
    public async Task<IEnumerable<PostVM>?> GetFollowedAsync(int numeroDiPost)
    {
        var token = Request.Headers["Authorization"].ToString();
        var userId = await AuthControllerUtility.GetUserIdFromTokenAsync(token);
        if (userId is null)
            return null;
        var user = await _unitOfWork.UserBehavior.GetFirstOrDefault(userId);
        if (user is null || user.FollowedUsers is null || user.FollowedUsers.Count == 0)
            return null;
        var posts = await _unitOfWork.Post.GetAllAsync();
        var followedPost = new List<PostVM>();
        foreach (var post in posts)
        {
            if (user.FollowedUsers.Contains(post.UserId))
            {
                var profilepic = _db.Users.Where(u => u.Id == post.UserId).FirstOrDefault().ProfilePicture;
                followedPost.Add(new PostVM {
                    Username = post.Username,
                    PostId = post.PostId,
                    Comments = post.Comments,
                    Content = post.Content,
                    DataDiCreazione = post.DataDiCreazione,
                    Hashtags = post.Hashtags,
                    ImgUrl = profilepic,
                    Likes = post.Likes,
                    Tags = post.Tags
                });
            }
        }
        if(numeroDiPost == null)
            return followedPost.OrderBy(p => p.DataDiCreazione).Take(numeroDiPost);
        return followedPost.OrderBy(p => p.DataDiCreazione);
    }
    [HttpGet("GetNextFollowed"), Authorize(Roles = $"{SD.ROLE_EMPLOYEE},{SD.ROLE_COMPANY_ADMIN},{SD.ROLE_USER}")]
    public async Task<PostVM?> GetNextFollowedAsync(int index)
    {
        if(index == null)
            return null;
        var token = Request.Headers["Authorization"].ToString();
        var userId = await AuthControllerUtility.GetUserIdFromTokenAsync(token);
        if (userId is null)
            return null;
        var user = await _unitOfWork.UserBehavior.GetFirstOrDefault(userId);
        if (user is null || user.FollowedUsers is null || user.FollowedUsers.Count == 0)
            return null;
        var posts = await _unitOfWork.Post.GetAllAsync();
        var followedPost = new List<PostVM>();
        foreach (var post in posts)
        {
            if (user.FollowedUsers.Contains(post.UserId))
            {
                var profilepic = _db.Users.Where(u => u.Id == post.UserId).FirstOrDefault().ProfilePicture;
                followedPost.Add(new PostVM
                {
                    Username = post.Username,
                    PostId = post.PostId,
                    Comments = post.Comments,
                    Content = post.Content,
                    DataDiCreazione = post.DataDiCreazione,
                    Hashtags = post.Hashtags,
                    ImgUrl = profilepic,
                    Likes = post.Likes,
                    Tags = post.Tags
                });
            }
        }
        return followedPost.OrderBy(p => p.DataDiCreazione).Take(index).Last();
    }
    [HttpGet("GetPostsOfRandom"), AllowAnonymous]
    public async Task<IEnumerable<PostVM>?> GetRandomAsync(int? numeroDiPost)
    {
        var token = Request.Headers["Authorization"].ToString();
        if (numeroDiPost is null || numeroDiPost < 0 || numeroDiPost > 30)
        {
            return Enumerable.Empty<PostVM>();
        }
        
        if (string.IsNullOrWhiteSpace(token))
        {
            numeroDiPost = DEFAULT_RANDOM_POST_AMOUNT;
        }
        else
        {
            try
            {
                var userId = await AuthControllerUtility.GetUserIdFromTokenAsync(token);
                if (userId is null)
                {
                    return Enumerable.Empty<PostVM>();
                }

                var user = await _unitOfWork.UserBehavior.GetFirstOrDefault(userId);
                if (user is null)
                {
                    return Enumerable.Empty<PostVM>();
                }
            }
            catch (Exception)
            {
                numeroDiPost = DEFAULT_RANDOM_POST_AMOUNT;
            }
        }
      
        var posts = await _unitOfWork.Post.GetAllAsync();
        var postsList = new List<PostVM>();
        foreach (var post in posts)
        {
            var profilepic = _db.Users.Where(u => u.Id == post.UserId).FirstOrDefault().ProfilePicture;
            postsList.Add(new PostVM
            {
                Username = post.Username,
                PostId = post.PostId,
                Comments = post.Comments,
                Content = post.Content,
                DataDiCreazione = post.DataDiCreazione,
                Hashtags = post.Hashtags,
                ImgUrl = profilepic,
                Likes = post.Likes,
                Tags = post.Tags
            });
        }
        return postsList.OrderBy(p => p.DataDiCreazione).Take((int)numeroDiPost);
    }
    [HttpGet("GetNextRandom"), Authorize(Roles = $"{SD.ROLE_EMPLOYEE},{SD.ROLE_COMPANY_ADMIN},{SD.ROLE_USER}")]
    public async Task<PostVM?> GetNextRandomAsync(int index)
    {
        if (index == null)
            return null;
        var token = Request.Headers["Authorization"].ToString();
        var userId = await AuthControllerUtility.GetUserIdFromTokenAsync(token);
        if (userId is null)
            return null;
        var user = await _unitOfWork.UserBehavior.GetFirstOrDefault(userId);
        if (user is null)
            return null;
        var posts = await _unitOfWork.Post.GetAllAsync();
        var postsList = new List<PostVM>();
        foreach (var post in posts)
        {
            var profilepic = _db.Users.Where(u => u.Id == post.UserId).FirstOrDefault().ProfilePicture;
            postsList.Add(new PostVM
            {
                Username = post.Username,
                PostId = post.PostId,
                Comments = post.Comments,
                Content = post.Content,
                DataDiCreazione = post.DataDiCreazione,
                Hashtags = post.Hashtags,
                ImgUrl = profilepic,
                Likes = post.Likes,
                Tags = post.Tags
            });
        }
        return postsList.OrderBy(p => p.DataDiCreazione).Skip(index).FirstOrDefault();
    }

    [HttpDelete("DeletePost"), Authorize(Roles = $"{SD.ROLE_EMPLOYEE},{SD.ROLE_COMPANY_ADMIN}, {SD.ROLE_USER}")]
    public async Task<ActionResult<string>> Delete(int postId)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString();
            var userId = await AuthControllerUtility.GetUserIdFromTokenAsync(token);
            if (userId is null)
                return BadRequest("User not found");
            var admin = _db.Users.Find(userId);
            if (!await _userManager.IsInRoleAsync(admin, SD.ROLE_COMPANY_ADMIN) && !await _userManager.IsInRoleAsync(admin, SD.ROLE_EMPLOYEE))
                return Forbid();
            var post = await _unitOfWork.Post.GetFirstOrDefault(postId);
            if (post == null)
                return BadRequest("Post not found");
            if (!post.UserId.Equals(userId))
                return BadRequest("User is not Authorized to remove post");
            var result = await _unitOfWork.Post.RemoveAsync(postId, userId);
            if (result)
            {
                var usersFromDb = await _unitOfWork.UserBehavior.GetAllAsync();
                if (usersFromDb.Count() != 0)
                {
                    foreach (var user in usersFromDb)
                    {
                        await MongoDbAlignment.RemovePostDataAsync(postId, user, _unitOfWork);
                    }
                }
                return Ok("Post removed succesfully");
            }
            return BadRequest("Something went wrong");
        }
        catch (Exception ex)
        {
            return BadRequest("Unandled exeption: " + ex.Message);
        }
    }

    [HttpPost("UploadImage"), Authorize(Roles = $"{SD.ROLE_EMPLOYEE},{SD.ROLE_COMPANY_ADMIN}, {SD.ROLE_USER}")]
    public async Task<object> PostImage(IFormFile? file)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString();
            var userId = await AuthControllerUtility.GetUserIdFromTokenAsync(token);
            var admin = _db.Users.Find(userId);
            if (!await _userManager.IsInRoleAsync(admin, SD.ROLE_COMPANY_ADMIN) && !await _userManager.IsInRoleAsync(admin, SD.ROLE_EMPLOYEE))
                return Forbid();
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images");
                var extension = Path.GetExtension(file.FileName);
                var link = Path.Combine(uploads, fileName + extension);
                string url = "https://progettoeasynet.azurewebsites.net/images/" + fileName + extension; // da modificare con il link futuro del sito
                using (var fileStreams = new FileStream(link, FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                return Ok(url);
            }
            return BadRequest("File is null");
        }
        catch(Exception ex)
        {
            return BadRequest("Error " + ex.Message);
        }
    }
    [HttpDelete("DeleteImage"), Authorize(Roles = $"{SD.ROLE_EMPLOYEE},{SD.ROLE_COMPANY_ADMIN}, {SD.ROLE_USER}")]
    public async Task<object> DeleteImage(string link)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString();
            var userId = await AuthControllerUtility.GetUserIdFromTokenAsync(token);
            var admin = _db.Users.Find(userId);
            if (!await _userManager.IsInRoleAsync(admin, SD.ROLE_COMPANY_ADMIN) && !await _userManager.IsInRoleAsync(admin, SD.ROLE_EMPLOYEE))
                return Forbid();
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string location = (new Uri(link)).PathAndQuery;
            System.IO.File.Delete(Path.Combine(wwwRootPath, location));
            return Ok("Deleted " + link);
        }
        catch (Exception ex)
        {
            return BadRequest("Error " + ex.Message);
        }
    }

    [HttpPost("UpsertPost"), Authorize(Roles = $"{SD.ROLE_EMPLOYEE},{SD.ROLE_COMPANY_ADMIN}, {SD.ROLE_USER}")]
    public async Task<ActionResult<string>> UpsertAsync([FromBody]UpsertPost post)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Model is not valid");
        }

        try
        {
            var token = Request.Headers["Authorization"].ToString();
            var userId = await AuthControllerUtility.GetUserIdFromTokenAsync(token);
            var admin = _db.Users.Find(userId);
            if (!await _userManager.IsInRoleAsync(admin, SD.ROLE_COMPANY_ADMIN) && !await _userManager.IsInRoleAsync(admin, SD.ROLE_EMPLOYEE))
                return Forbid();
            if (post.PostId == 0)
            {
                var newPost = new Post()
                {
                    PostId = await IdAutoincrementService.GetPostAutoincrementId(_unitOfWork),
                    Comments = new List<Comment>(),
                    UserId = userId,
                    Username = _db.Users.Where(u => u.Id == userId).Select(u => u.UserName).FirstOrDefault(),
                    Content = post.Content,
                    Likes = new List<string>(),
                    Hashtags = new List<string>(),
                    Tags = new List<string>(),
                    DataDiCreazione = DateTime.Now
                };
                await _unitOfWork.Post.AddAsync(newPost);
                return Ok("Post created succesfully");
            }
            else
            {
                var result = await _unitOfWork.Post.UpdatePostContentAsync(post, userId);
                if (result)
                {
                    return Ok("Post modified succesfully");
                }
                else
                {
                    return BadRequest("Post not found");
                }
            }

        }
        catch (Exception ex)
        {
            return BadRequest("Unandled exception: " + ex.Message);
        }
    }
}