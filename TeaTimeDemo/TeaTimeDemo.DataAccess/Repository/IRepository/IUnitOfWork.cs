using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaTimeDemo.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }

        IProductRepository Product { get; }

        IStoreRepository Store { get; }

        IShoppingCartRepository ShoppingCart { get; }

        IApplicationUserRepository ApplicationUser { get; }

        IOrderHeaderRepository OrderHeader { get; }

        IOrderDetailRepository OrderDetail { get; }

        IStationRepository Station { get; }

        // 新增問卷相關的 Repository
        ISurveyRepository Survey { get; }
        IQuestionRepository Question { get; }
        IAnswerRepository Answer { get; }
        IAnswerOptionRepository AnswerOption { get; }
        IQuestionImageRepository QuestionImage { get; }
        IQuestionOptionRepository QuestionOption { get; }

        void Save();

        void SaveChanges();
    }
}
