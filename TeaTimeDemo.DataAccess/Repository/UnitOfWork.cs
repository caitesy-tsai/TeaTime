using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaTimeDemo.DataAccess.Data;
using TeaTimeDemo.DataAccess.Repository.IRepository;
using TeaTimeDemo.Models;

namespace TeaTimeDemo.DataAccess.Repository
{
    public class UnitOfWork:IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IStoreRepository Store { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }

        public IOrderHeaderRepository OrderHeader { get; private set; }

        public IOrderDetailRepository OrderDetail { get; private set; }

        public IStationRepository Station { get; private set; }

        // 問卷相關的 Repository 實例
        public ISurveyRepository Survey { get; private set; }
        public IQuestionRepository Question { get; private set; }
        public IAnswerRepository Answer { get; private set; }
        public IAnswerOptionRepository AnswerOption { get; private set; }
        public IQuestionImageRepository QuestionImage { get; private set; }
        public IQuestionOptionRepository QuestionOption { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Store = new StoreRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            Station = new StationRepository(_db);

            // 初始化問卷相關的 Repository
            Survey = new SurveyRepository(_db);
            Question = new QuestionRepository(_db);
            Answer = new AnswerRepository(_db);
            AnswerOption = new AnswerOptionRepository(_db);
            QuestionImage = new QuestionImageRepository(_db);
            QuestionOption = new QuestionOptionRepository(_db);

        }
        public void Save()
        {
            _db.SaveChanges();
        }

        // Dispose 方法，當不再需要 UnitOfWork 時釋放資源
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
