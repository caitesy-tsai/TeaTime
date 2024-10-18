using TeaTimeDemo.DataAccess.Data;
using TeaTimeDemo.DataAccess.Repository.IRepository;
using TeaTimeDemo.Models;
using System.Linq;

namespace TeaTimeDemo.DataAccess.Repository
{
    public class QuestionImageRepository : Repository<QuestionImage>, IQuestionImageRepository
    {
        private readonly ApplicationDbContext _db;

        public QuestionImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(QuestionImage obj)
        {
            var objFromDb = _db.QuestionImages.FirstOrDefault(qi => qi.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.ImageUrl = obj.ImageUrl;
                objFromDb.AltText = obj.AltText;

                // 更新 SurveyId 和 QuestionOptionId 的邏輯（新增）
                objFromDb.SurveyId = obj.SurveyId;
                objFromDb.QuestionId = obj.QuestionId;
                objFromDb.QuestionOptionId = obj.QuestionOptionId;

                // 更新其他屬性...
            }
        }
    }
}
