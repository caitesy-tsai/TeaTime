using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using TeaTimeDemo.DataAccess.Data;
using TeaTimeDemo.DataAccess.Repository.IRepository;

namespace TeaTimeDemo.DataAccess.Repository
{
    // Repository 類別實作 IRepository 介面，對資料庫進行基本的 CRUD 操作
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db; // 資料庫上下文
        internal DbSet<T> dbSet; // 代表資料庫中的某一表

        // 建構子，傳入資料庫上下文並設定 dbSet
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>(); // 設定對應的 DbSet
            _db.Products.Include(u => u.Category).Include(u => u.CategoryId);
        }

        // 取得所有資料，支持條件篩選和關聯屬性
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet; // 建立基本查詢

            if (filter != null) // 如果有篩選條件
            {
                query = query.Where(filter); // 套用篩選條件
            }

            if (!string.IsNullOrEmpty(includeProperties)) // 如果指定了關聯屬性
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp); // 套用關聯屬性查詢
                }
            }

            return query.ToList(); // 返回查詢結果
        }

        // 根據條件取得單一資料，支持關聯屬性
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter); // 根據篩選條件取得資料
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp); // 套用關聯屬性查詢
                }
            }
            return query.FirstOrDefault(); // 返回符合條件的第一筆資料
        }

        // 根據條件取得單一資料，支持關聯屬性
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter); // 根據篩選條件取得資料
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp); // 套用關聯屬性查詢
                }
            }
            return query.FirstOrDefault(); // 返回符合條件的第一筆資料
        }

        // 新增單筆資料
        public void Add(T entity)
        {
            dbSet.Add(entity); // 將實體加入到資料庫上下文中
        }

        // 批次新增多筆資料
        public void AddRange(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities); // 批次將多個實體加入到資料庫上下文中
        }

        // 更新現有的資料
        public void Update(T entity)
        {
            dbSet.Update(entity); // 更新現有的實體資料
        }

        // 刪除單筆資料
        public void Delete(T entity)
        {
            dbSet.Remove(entity); // 從資料庫上下文中移除該實體
        }

        // 刪除多筆資料 (批次刪除)
        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities); // 批次移除多筆資料
        }

        // 刪除表中的所有資料
        public void DeleteAll()
        {
            var entities = dbSet.ToList(); // 先取得所有實體
            dbSet.RemoveRange(entities); // 批次移除所有實體
        }

        // 根據 ID 取得單一資料
        public T GetById(int id)
        {
            return dbSet.Find(id); // 根據主鍵查找實體
        }

        // 根據 ID 刪除資料
        public void DeleteById(int id)
        {
            var entity = GetById(id); // 先查詢該 ID 的實體
            if (entity != null)
            {
                Delete(entity); // 如果找到實體，則移除
            }
        }

        // 刪除資料透過實體類別
        public void Remove(T entity)
        {
            dbSet.Remove(entity); // 將實體從資料庫上下文中移除
        }

        // 儲存變更，將資料庫上下文的變更保存到實際資料庫
        public void SaveChanges()
        {
            _db.SaveChanges(); // 保存所有變更
        }
    }
}


