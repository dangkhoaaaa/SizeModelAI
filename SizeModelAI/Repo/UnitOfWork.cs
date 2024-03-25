using SizeModelAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeModelAI.Repo
{
    public class UnitOfWork : IDisposable
    {
        private ClothingAssigmentContext context = new ClothingAssigmentContext();
        private GenericRepository<Clothing> _ClothingRepository;
        private GenericRepository<ClothingSize> _ClothingSizeRepository;


        public GenericRepository<Clothing> ClothingRepository
        {
            get
            {

                if (_ClothingRepository == null)
                {
                    _ClothingRepository = new GenericRepository<Clothing>(context);
                }
                return _ClothingRepository;
            }
        }

        public GenericRepository<ClothingSize> ClothingSizeRepository
        {
            get
            {

                if (_ClothingSizeRepository == null)
                {
                    _ClothingSizeRepository = new GenericRepository<ClothingSize>(context);
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
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
