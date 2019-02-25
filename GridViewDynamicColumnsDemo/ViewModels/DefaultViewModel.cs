using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.Binding;
using DotVVM.Framework.Binding.Expressions;
using DotVVM.Framework.Compilation.ControlTree;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Utils;
using DotVVM.Framework.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace GridViewDynamicColumnsDemo.ViewModels
{
    public class DefaultViewModel : DotvvmViewModelBase
    {
        public ICollection<GridViewData> Data { get; set; }= new List<GridViewData>()
        {
            new GridViewData(){DefaultProp = 1,AdditionalProp = -1},
            new GridViewData(){DefaultProp = 2,AdditionalProp = -2},
            new GridViewData(){DefaultProp = 3,AdditionalProp = -3},
            new GridViewData(){DefaultProp = 4,AdditionalProp = -4},
            new GridViewData(){DefaultProp = 5,AdditionalProp = -5},
            new GridViewData(){DefaultProp = 6,AdditionalProp = -6},
            new GridViewData(){DefaultProp = 7,AdditionalProp = -7},
            new GridViewData(){DefaultProp = 8,AdditionalProp = -8},
            new GridViewData(){DefaultProp = 9,AdditionalProp = -9},
        };
        
        

        public override Task Load()
        {
            AddColumn("Load");
            return base.Load();
        }

        public void AddColumn(string name)
        {
            var bindingService = Context.Configuration.ServiceProvider.GetRequiredService<BindingCompilationService>();

            //gets element with ID attribute set to GridView
            var gridView = (GridView)Context.View.FindControlByClientId("GridView");

            //get DataContext from other column to ensure that the new column will have the same dataContext
            //we could also use DataContextStack.Create(typeof(GridViewData)); if we cannot copy DataContext
            var dataContextStack = gridView.Columns[0].GetDataContextType();

            // ((GridViewData)objects[0]).AdditionalProp translates into _parent0.AdditionalProp which is equal to _this.AdditionalProp
            Expression<Func<object[], int>> expression = objects => ((GridViewData)objects[0]).AdditionalProp;


            //this is quite expensive call
            //but soon there should be available cached version
            //usage will be bindingService.Cache.CreateCachedBinding("ID", new [] { dataContext }, () => CreateBinding<T>(service, o => (T)o[0], dataContext).CreateBinding(bindingService,expression,dataContextStack));
            //see https://github.com/riganti/dotvvm/pull/672 for more info
            var valueBindingExpression = ValueBindingExpression.CreateBinding(bindingService,expression,dataContextStack);
            
            var gridViewTextColumn = new GridViewTextColumn()
            {
                HeaderText = name,
                ValueBinding = valueBindingExpression
            };

            gridView.Columns.Add(gridViewTextColumn);
        }
    }



    public class GridViewData
    {
        public int DefaultProp { get; set; }
        public int AdditionalProp { get; set; }
        
    }
}

