using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Script.Serialization;
using MyLife.Validation;
using MyLife.Web.DynamicData;
using xVal.ServerSide;

namespace MyLife.Web
{
    [Serializable]
    public abstract class BizObject<T, K> : IEquatable<BizObject<T, K>>, IDisposable where T : BizObject<T, K>
    {
        protected static readonly object syncRoot = new object();
        private string modifiedBy;
        private DateTime modifiedDate;

        protected BizObject()
        {
        }

        protected BizObject(K id)
        {
            Id = id;
        }

        protected BizObject(K id, DateTime createdDate, string createdBy, DateTime modifiedDate, string modifiedBy)
            : this()
        {
            Id = id;
            CreatedDate = createdDate;
            CreatedBy = createdBy;
            ModifiedDate = modifiedDate;
            ModifiedBy = modifiedBy;
        }

        [ScriptIgnore]
        public IPrincipal User
        {
            get { return HttpContext.Current.User; }
        }

        [DynamicIdentityField]
        public K Id { get; protected set; }

        [ScriptIgnore]
        public DateTime CreatedDate { get; protected set; }

        [ScriptIgnore]
        public string CreatedBy { get; protected set; }

        [ScriptIgnore]
        public DateTime ModifiedDate
        {
            get { return modifiedDate == DateTime.MinValue ? CreatedDate : modifiedDate; }
            protected set { modifiedDate = value; }
        }

        [ScriptIgnore]
        public string ModifiedBy
        {
            get { return string.IsNullOrEmpty(modifiedBy) ? CreatedBy : modifiedBy; }
            protected set { modifiedBy = value; }
        }

        #region Data Access

        /// <summary>
        /// Inserts a new object to the data store.
        /// </summary>
        protected abstract K DataInsert();

        /// <summary>
        /// Updates the object in its data store.
        /// </summary>
        protected abstract void DataUpdate();

        /// <summary>
        /// Deletes the object from the data store.
        /// </summary>
        protected abstract void DataDelete();

        protected virtual void OnDataInserting()
        {
            CreatedBy = MyLifeContext.Current.User.Identity.Name;
            if (CreatedDate == DateTime.MinValue)
            {
                CreatedDate = DateTime.UtcNow;
            }
        }

        protected virtual void OnDataInserted()
        {
        }

        protected virtual void OnDataUpdating()
        {
            ModifiedBy = MyLifeContext.Current.User.Identity.Name;
            ModifiedDate = DateTime.UtcNow;
        }

        protected virtual void OnDataUpdated()
        {
        }

        protected virtual void OnDataDeleting()
        {
        }

        protected virtual void OnDataDeleted()
        {
        }

        /// <summary>
        /// Save object to the data store.
        /// </summary>
        public virtual void Save()
        {
            VerifyAuthorization();
            OnSaving();

            if (IsNew)
            {
                OnDataInserting();
                Validate();
                Id = DataInsert();
                OnDataInserted();
            }
            else
            {
                OnDataUpdating();
                Validate();
                DataUpdate();
                OnDataUpdated();
            }
        }

        protected virtual void OnSaving()
        {
        }

        /// <summary>
        /// Delete object from the data store
        /// </summary>
        public virtual void Delete()
        {
            IsDeleted = true;
            VerifyAuthorization();
            OnDataDeleting();
            DataDelete();
            OnDataDeleted();
            Dispose();
        }

        #endregion

        #region Object State

        [ScriptIgnore]
        public bool IsNew
        {
            get { return Id.Equals(default(K)); }
        }

        protected bool IsDeleted { get; set; }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IEquatable<BizObject<T,K>> Members

        public bool Equals(BizObject<T, K> obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Id, Id);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BizObject<T, K>)) return false;
            return Equals((BizObject<T, K>) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(BizObject<T, K> left, BizObject<T, K> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BizObject<T, K> left, BizObject<T, K> right)
        {
            return !Equals(left, right);
        }

        #region Validate

        public virtual void Validate()
        {
            var errors = DataAnnotationsValidationRunner.GetErrors(this);
            if (errors.Any())
                throw new RulesException(errors);
        }

        #endregion

        #region Authorization

        protected abstract void VerifyAuthorization();

        #endregion

        #region Clone

        public static void CopyFromObject(T obj, object source)
        {
            var propertiesOfSource = TypeDescriptor.GetProperties(source);
            var propertiesOfObject = TypeDescriptor.GetProperties(obj);
            foreach (PropertyDescriptor propertyOfSource in propertiesOfSource)
            {
                if (propertyOfSource.PropertyType.IsGenericType) continue;

                var value = propertyOfSource.GetValue(source);
                if (value == null) continue;
                var propertyOfObject = propertiesOfObject.Find(propertyOfSource.Name, false);
                if (propertyOfObject != null)
                {
                    propertyOfObject.SetValue(obj, value);
                }
            }
        }

        public void CopyFromObject(object source)
        {
            CopyFromObject(this as T, source);
        }

        public static void CopyToObject(T source, object target)
        {
            var propertiesOfSource = TypeDescriptor.GetProperties(source);
            var propertiesOfTarget = TypeDescriptor.GetProperties(target);

            foreach (PropertyDescriptor propertyOfSource in propertiesOfSource)
            {
                if (propertyOfSource.Name == "Id" || propertyOfSource.PropertyType.IsGenericType) continue;
                var value = propertyOfSource.GetValue(source);
                if (value == null) continue;
                var propertyOfTarget = propertiesOfTarget.Find(propertyOfSource.Name, false);
                if (propertyOfTarget != null)
                {
                    propertyOfTarget.SetValue(target, value);
                }
            }
        }

        public void CopyToObject(object target)
        {
            CopyToObject(this as T, target);
        }

        #endregion
    }
}