using TeaTimeDemo.DataAccess.Data;
using TeaTimeDemo.DataAccess.Repository.IRepository;
using TeaTimeDemo.Models;

namespace TeaTimeDemo.DataAccess.Repository
{
    public class SurveyRepository : Repository<Survey>, ISurveyRepository
    {
        private readonly ApplicationDbContext _db;

        public SurveyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Survey obj)
        {
            var objFromDb = _db.Surveys.FirstOrDefault(s => s.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.CategoryName = obj.CategoryName;  // 更新類別名稱
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                objFromDb.StationName = obj.StationName;  // 更新站別名稱
                objFromDb.QuestionNum = obj.QuestionNum;  // 更新頁數或問題順序
                objFromDb.IsPublished = obj.IsPublished;
                objFromDb.MceHtml = obj.MceHtml;

                if (obj.IsPublished && objFromDb.CompleteTime == null)
                {
                    objFromDb.CompleteTime = DateTime.Now;
                }
                else if (!obj.IsPublished)
                {
                    objFromDb.CompleteTime = null; // 如果取消發佈，清空完成時間
                }
            }
        }
    }
}
