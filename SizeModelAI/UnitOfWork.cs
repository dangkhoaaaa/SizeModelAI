using SizeModelAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeModelAI
{
    public class UnitOfWork : IDisposable
    {
        private ClothingAssigmentContext context = new ClothingAssigmentContext();
        private GenericRepository<Clothing> _ClothingRepository;
        private GenericRepository<ClothingColor> _ClothingColorRepository;
        private GenericRepository<ClothingSize> _ClothingSizeRepository;


        //public UnitOfWork(SqldataContext context)
        //{
        //    this.context = context;
        //}

        public GenericRepository<Clothing> ClothingRepository
        {
            get
            {

                if (this._ClothingRepository == null)
                {
                    this._ClothingRepository = new GenericRepository<Clothing>(context);
                }
                return _ClothingRepository;
            }
        }
        public GenericRepository<ClothingColor> ClothingColorRepository

        {
            get
            {

                if (this._ClothingColorRepository == null)
                {
                    this._ClothingColorRepository = new GenericRepository<ClothingColor>(context);
                }
                return _ClothingColorRepository;
            }
        }
        public GenericRepository<ClothingSize> ClothingSizeRepository
        {
            get
            {

                if (this._ClothingSizeRepository == null)
                {
                    this._ClothingSizeRepository = new GenericRepository<ClothingSize>(context);
                }
                return _ClothingSizeRepository;
            }
        }
    
        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
