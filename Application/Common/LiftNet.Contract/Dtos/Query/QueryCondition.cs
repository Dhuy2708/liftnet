using LiftNet.Contract.Enums;
using LiftNet.Domain.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Dtos.Query
{
    public class QueryCondition
    {
        public List<ConditionItem> ConditionItems
        {
            get; set;
        } = [];
            
        public string? Search
        {
            get; set;
        }

        public int PageNumber
        {
            get; set;
        } = 1;

        public int PageSize
        {
            get; set;
        } = 20;
        public string? NextPageToken
        {
            get; set;
        } = null;
        public SortCondition? Sort
        {
            get; set;
        }

        public QueryCondition(List<ConditionItem> conditionItems)
        {
            ConditionItems = conditionItems;
        }

        public QueryCondition() { }

        public void AddCondition(ConditionItem conditionItem)
        {
            ConditionItems.Add(conditionItem);
        }

        public void UpdateCondition(ConditionItem condToUpdate, bool addIfExist = false)
        {
            var cond = ConditionItems.Find(x => x.Property.Equals(condToUpdate.Property));
            if (cond != null)
            {
                ConditionItems.Remove(cond);
                ConditionItems.Add(condToUpdate);
            }
            else if (addIfExist)
            {
                AddCondition(condToUpdate);
            }
        }

        public List<ConditionItem>? FindConditions(string propName)
        {
            var result = ConditionItems.Where(x => x.Property.Equals(propName)).ToList();
            if (result != null)
            {
                return result.ToList();
            }
            return null;
        }

        public ConditionItem? FindCondition(string propName)
        {
            return ConditionItems.Find(x => x.Property.Equals(propName));
        }

        public string? GetValue(string propName)
        {
            var cond = ConditionItems.Find(x => x.Property.Equals(propName));
            if (cond != null && cond.Values.Count > 0)
            {
                return cond.Values[0];
            }
            return null;
        }

        public T? GetValue<T>(string propName) where T : struct
        {
            var cond = ConditionItems.Find(x => x.Property.Equals(propName));
            if (cond != null && cond.Values.Count > 0)
            {
                return (T)Convert.ChangeType(cond.Values[0], typeof(T));
            }
            return null;
        }

        public List<T>? GetValues<T>(string propName) where T : struct
        {
            var cond = ConditionItems.Find(x => x.Property.Equals(propName));
            if (cond != null && cond.Values.Count > 0)
            {
                return cond.Values.Select(x => (T)Convert.ChangeType(x, typeof(T))).ToList();
            }
            return null;
        }
    }

    public class ConditionItem
    {
        public string Property { get; set; }
        public QueryOperator Operator { get; set; }
        public List<string> Values { get; set; } = [];
        public FilterType Type { get; set; }
        public QueryLogic Logic { get; set; } // And, Or, None

        public ConditionItem()
        {

        }

        public ConditionItem(string property, List<string> values, FilterType filterType = FilterType.String, QueryOperator queryOperator = QueryOperator.Equal, QueryLogic logic = QueryLogic.None)
        {
            Property = property;
            Values = values;
            Operator = queryOperator;
            Logic = logic;
            Type = filterType;
        }

        public ConditionItem(string property, string value, QueryOperator queryOperator = QueryOperator.Equal, QueryLogic logic = QueryLogic.None)
        {
            Property = property;
            Values = [value];
            Operator = queryOperator;
            Logic = logic;
            Type = FilterType.String;
        }

        public ConditionItem(string property, int value, QueryOperator queryOperator = QueryOperator.Equal, QueryLogic logic = QueryLogic.None)
        {
            Property = property;
            Values = [value.ToString()];
            Operator = queryOperator;
            Logic = logic;
            Type = FilterType.Integer;
        }

        public ConditionItem(string property, bool value, QueryOperator queryOperator = QueryOperator.Equal, QueryLogic logic = QueryLogic.None)
        {
            Property = property;
            Values = [value.ToString()];
            Operator = queryOperator;
            Logic = logic;
            Type = FilterType.Boolean;
        }

        public ConditionItem(DataSchema schema, QueryOperator queryOperator = QueryOperator.Equal, QueryLogic logic = QueryLogic.None)
        {
            Property = "schema";
            Values = [$"{(int)schema}"];
            Operator = queryOperator;
            Logic = logic;
            Type = FilterType.Integer;
        }
    }

    public class SortCondition
    {
        public string? Name
        {
            get; set;
        }

        public SortType Type
        {
            get; set;
        }
        public void Clear()
        {
            Name = null;
            Type = SortType.None;
        }
    }
}
