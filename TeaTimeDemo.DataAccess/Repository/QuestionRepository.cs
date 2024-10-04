using TeaTimeDemo.DataAccess.Data;
using TeaTimeDemo.DataAccess.Repository.IRepository;
using TeaTimeDemo.Models;

namespace TeaTimeDemo.DataAccess.Repository
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        private readonly ApplicationDbContext _db;

        public QuestionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Question obj)
        {
            var objFromDb = _db.Questions.FirstOrDefault(q => q.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.QuestionText = obj.QuestionText;
                objFromDb.AnswerType = obj.AnswerType;
                objFromDb.MceHtml = obj.MceHtml;
                // 其他屬性更新邏輯...
            }
        }
    }
}
