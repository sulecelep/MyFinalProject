using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Interceptors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Aspects.Autofac.Validation
{
    public class ValidationAspect : MethodInterception
    {
        private Type _validatorType;
        public ValidationAspect(Type validatorType)
        {
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new System.Exception("Bu bir validator sınıfı değildir.");
            }

            _validatorType = validatorType;
        }
        protected override void OnBefore(IInvocation invocation)
        {
            var validator = (IValidator)Activator.CreateInstance(_validatorType); //reflection : run time'da new'leme yapar
            var entityType = _validatorType.BaseType.GetGenericArguments()[0]; //validator sınıfını bul ve generic parametrelerinden ilkini getir yani ProductValidator ise Product'ı getir diyor
            var entities = invocation.Arguments.Where(t => t.GetType() == entityType); //bu validator sınıfının parametrelerinden entity olanları getir
            foreach (var entity in entities) //her bir entity için validate ediyor // mesela iki tane entity gönderdik Product ve Category o zaman ikisini de validate ediyor.
            {
                ValidationTool.Validate(validator, entity); 
            }
        }
    }
}
