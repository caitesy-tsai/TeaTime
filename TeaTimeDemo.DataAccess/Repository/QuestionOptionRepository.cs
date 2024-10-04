using TeaTimeDemo.DataAccess.Data;
using TeaTimeDemo.DataAccess.Repository.IRepository;
using TeaTimeDemo.Models;

namespace TeaTimeDemo.DataAccess.Repository
{
    public class QuestionOptionRepository : Repository<QuestionOption>, IQuestionOptionRepository
    {
        private readonly ApplicationDbContext _db;

        public QuestionOptionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(QuestionOption obj)
        {
            var objFromDb = _db.QuestionOptions.FirstOrDefault(qo => qo.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.OptionText = obj.OptionText;
                objFromDb.SortOrder = obj.SortOrder;
                // 其他屬性更新邏輯...
            }
        }
    }
}
