using System;
using Xunit;
using DocumentDBGettingStarted.Controllers;
using DocumentDBGettingStarted.Models;
using DocumentDBGettingStarted.Repository;
using GenFu;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
		private IRepository<Subscriber, string> dbUser;
		private IRepository<Tag, string> dbTag;
		private IRepository<Article, string> dbArticle;
		private List<Subscriber> users;

		public UnitTest1(
			IRepository<Subscriber,string> SubscriberDocumentDBRepository,
			IRepository<Tag, string> TagDocumentDBRepository,
			IRepository<Article, string> ArticleDocumentDBRepository
			) {
			dbUser = SubscriberDocumentDBRepository;
			dbTag = TagDocumentDBRepository;
			dbArticle = ArticleDocumentDBRepository;
			users = CreateUser();
		}

		[Fact]
		public void UserCreate()
		{
	
			var controller = new SubscriberController();
			foreach (Subscriber user in users)
			{
				{
					Subscriber checkUser = dbUser.GetAllListWhere(f => f.Nickname == user.Nickname).Result.SingleOrDefault();
					if (checkUser == null)
					{
						ActionResult okResult = controller.Create(user);
						Assert.IsType(typeof(OkResult), okResult);
					}
				}

			}
		}

		[Fact]
        public void UniqUser()
        {
			var controller = new SubscriberController();
			foreach(Subscriber user in users)
			{
				Subscriber checkUser = dbUser.GetAllListWhere(f => f.Nickname == user.Nickname).Result.SingleOrDefault();
				if (checkUser != null)
				{
					Exception ex = Assert.Throws<Exception>(() => controller.Create(user));
					Assert.Equal(String.Format("User with nickname {0} already exist", user.Nickname), ex.Message);
				}

			}
		}
		public List<Article> CreateArticle()
		{
			A.Configure<Article>().Fill(a => a.Content).AsLoremIpsumSentences(10);
			A.Configure<Article>().Fill(a => a.Title).AsArticleTitle();
			A.Configure<Article>().Fill(a => a.DateUpdated).WithRandom(new List<string>() { null });
			A.Configure<Article>().Fill(a => a.DatePublished).WithRandom(new List<string>() { null });
			List <Article> Articles = A.ListOf<Article>(20);
			return Articles;
		}

		public List<Subscriber> CreateUser()
		{
			A.Configure<Subscriber>().Fill(a => a.Id).WithRandom(new List<string>() { null });
			List<Subscriber> users = A.ListOf<Subscriber>(1);
			return users;
		}

		public Tag CreateTag()
		{
			Tag tag = A.New<Tag>();
			return tag;
		}
	}
}
