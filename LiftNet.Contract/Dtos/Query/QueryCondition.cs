using LiftNet.Contract.Enums;
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
            } = new List<ConditionItem>();

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
        }

        public class ConditionItem
        {
            public string Property { get; set; }
            public QueryOperator Operator { get; set; }
            public List<string> Values { get; set; }
            public FilterType Type { get; set; }
            public QueryLogic Logic { get; set; } // And, Or, None

            public ConditionItem()
            {

            }

            public ConditionItem(string property, List<string> values, QueryOperator queryOperator, QueryLogic logic = QueryLogic.None)
            {
                Property = property;
                Values = values;
                Operator = queryOperator;
                Logic = logic;
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
